﻿using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Type
{

    // these types are shared by front and back

    // the back does not want to see the errors
    // anything that gets to it should be squeeky clean

    // the frontend wants these to have validate
    // and it wants to conver them to model type

    // and model has types too! ahh!

    public struct Yes { }
    public struct No { }

    public static class OrTypeLibrary {


        public delegate IIsPossibly<(IOrType<T, IError> fromLeft , IOrType<T, IError> fromRight)> IsOrType<T>(T them);

        public static IOrType<bool, IError> CanAssign<T>(
            T from,
            IsOrType<T> check,
            T to,
            IOrType<T, IError> toLeft,
            IOrType<T, IError> toRight,
            Func<T,T, List<(T, T)>, IOrType<bool, IError>> theyAreUs,
            List<(T, T)> assumeTrue)
        {
            if (assumeTrue.Contains((to, from)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((to, from));



            return check(from).IfElseReturn(x =>
            {
                // both of their type must be assignable to one of our types...

                var leftRes = x.fromLeft.TransformInner(x =>
                {
                    return theyAreUs(to, x, assumeTrue);
                });
                var rightRes = x.fromRight.TransformInner(x =>
                {
                    return theyAreUs(to, x, assumeTrue);
                });

                return leftRes.SwitchReturns(x => rightRes.SwitchReturns(
                        y => OrType.Make<bool, IError>(x && y),
                        y => OrType.Make<bool, IError>(y)),
                    x => rightRes.SwitchReturns(
                        y => OrType.Make<bool, IError>(x),
                        y => OrType.Make<bool, IError>(Error.Cascaded("", new[] { x, y }))));
            }, () =>
            {
                // they must be assignable to one of our types

                var leftRes = toLeft.TransformInner(x =>
                {
                    return theyAreUs(x, from, assumeTrue);
                });
                var rightRes = toRight.TransformInner(x =>
                {
                    return theyAreUs(x, from, assumeTrue);
                });


                return leftRes.SwitchReturns(x => rightRes.SwitchReturns(
                        y => OrType.Make<bool, IError>(x || y),
                        y => x ? OrType.Make<bool, IError>(true) : OrType.Make<bool, IError>(y)),
                    x => rightRes.SwitchReturns(
                        y => y ? OrType.Make<bool, IError>(true) : OrType.Make<bool, IError>(x),
                        y => OrType.Make<bool, IError>(Error.Cascaded("", new[] { x, y }))));
            });
        }

        public static IOrType<IOrType<(T,Access), IError>, No, IError> GetMember<T>(
            IKey key, 
            IOrType<T, IError> left, 
            IOrType<T, IError> right,
            Func<T, IKey, List<(T, T)>, IOrType<IOrType<(T,Access), IError>, No, IError>> tryGetMember,
            Func<T, T, T> make,
            Func<T,T, List<(T, T)>, IOrType<bool, IError>> theyAreUs, 
            List<(T,T)> assumeTrue
            )
        {
            return left.SwitchReturns(
                    leftType => right.SwitchReturns(
                        rightType =>
                        {
                            var leftMember = tryGetMember(leftType,key, assumeTrue);
                            var rightMember = tryGetMember(rightType,key, assumeTrue);

                            if (leftMember.Is1(out IOrType<(T,Access), IError> definateLeft) && rightMember.Is1(out IOrType <(T, Access), IError> definateRight))
                            {
                                return definateLeft.SwitchReturns(
                                    valueLeft => definateRight.SwitchReturns(
                                        valueRight => {
                                            if (valueLeft.Item2 == Access.ReadOnly && valueRight.Item2 == Access.WriteOnly)
                                            {
                                                return OrType.Make<IOrType<(T, Access), IError>, No, IError>(new No());
                                            }
                                            if (valueLeft.Item2 == Access.ReadOnly && valueRight.Item2 == Access.ReadWrite)
                                            {
                                                return OrType.Make<IOrType<(T, Access), IError>, No, IError>(OrType.Make<(T, Access), IError>((make(valueLeft.Item1, valueRight.Item1), Access.ReadOnly)));
                                            }
                                            if (valueLeft.Item2 == Access.ReadOnly && valueRight.Item2 == Access.ReadOnly)
                                            {
                                                return OrType.Make<IOrType<(T, Access), IError>, No, IError>(OrType.Make<(T, Access), IError>((make(valueLeft.Item1, valueRight.Item1), Access.ReadOnly)));
                                            }

                                            if (valueLeft.Item2 == Access.WriteOnly && valueRight.Item2 == Access.ReadOnly)
                                            {
                                                return OrType.Make<IOrType<(T, Access), IError>, No, IError>(new No());
                                            }
                                            if (valueLeft.Item2 == Access.WriteOnly && valueRight.Item2 == Access.ReadWrite)
                                            {
                                                return theyAreUs(valueLeft.Item1, valueRight.Item1, assumeTrue).TransformInner(_ => theyAreUs(valueRight.Item1, valueLeft.Item1, assumeTrue)).SwitchReturns(same =>
                                                {
                                                    if (same)
                                                    {
                                                        return OrType.Make<IOrType<(T, Access), IError>, No, IError>(OrType.Make<(T, Access), IError>((make(valueLeft.Item1, valueRight.Item1), Access.WriteOnly)));
                                                    }
                                                    else
                                                    {
                                                        return OrType.Make<IOrType<(T, Access), IError>, No, IError>(new No());
                                                    }
                                                },
                                                error => OrType.Make<IOrType<(T, Access), IError>, No, IError>(error));
                                            }
                                            if (valueLeft.Item2 == Access.WriteOnly && valueRight.Item2 == Access.WriteOnly)
                                            {
                                                return theyAreUs(valueLeft.Item1, valueRight.Item1, assumeTrue).TransformInner(_ => theyAreUs(valueRight.Item1, valueLeft.Item1, assumeTrue)).SwitchReturns(same =>
                                                {
                                                    if (same)
                                                    {
                                                        return OrType.Make<IOrType<(T, Access), IError>, No, IError>(OrType.Make<(T, Access), IError>((make(valueLeft.Item1, valueRight.Item1), Access.WriteOnly)));
                                                    }
                                                    else
                                                    {
                                                        return OrType.Make<IOrType<(T, Access), IError>, No, IError>(new No());
                                                    }
                                                },
                                                error => OrType.Make<IOrType<(T, Access), IError>, No, IError>(error));
                                            }

                                            if (valueLeft.Item2 == Access.ReadWrite && valueRight.Item2 == Access.ReadOnly)
                                            {
                                                return OrType.Make<IOrType<(T, Access), IError>, No, IError>(OrType.Make<(T, Access), IError>((make(valueLeft.Item1, valueRight.Item1), Access.ReadOnly)));
                                            }
                                            if (valueLeft.Item2 == Access.ReadWrite && valueRight.Item2 == Access.ReadWrite)
                                            {
                                                return theyAreUs(valueLeft.Item1, valueRight.Item1, assumeTrue).TransformInner(_ => theyAreUs(valueRight.Item1, valueLeft.Item1, assumeTrue)).SwitchReturns(same =>
                                                {
                                                    if (same)
                                                    {
                                                        return OrType.Make<IOrType<(T, Access), IError>, No, IError>(OrType.Make<(T, Access), IError>((make(valueLeft.Item1, valueRight.Item1), Access.ReadWrite)));
                                                    }
                                                    else
                                                    {
                                                        return OrType.Make<IOrType<(T, Access), IError>, No, IError>(OrType.Make<(T, Access), IError>((make(valueLeft.Item1, valueRight.Item1), Access.ReadOnly)));
                                                    }
                                                },
                                                error => OrType.Make<IOrType<(T, Access), IError>, No, IError>(error));
                                            }
                                            if (valueLeft.Item2 == Access.ReadWrite && valueRight.Item2 == Access.WriteOnly)
                                            {
                                                return theyAreUs(valueLeft.Item1, valueRight.Item1, assumeTrue).TransformInner(_ => theyAreUs(valueRight.Item1, valueLeft.Item1, assumeTrue)).SwitchReturns(same =>
                                                {
                                                    if (same)
                                                    {
                                                        return OrType.Make<IOrType<(T, Access), IError>, No, IError>(OrType.Make<(T, Access), IError>((make(valueLeft.Item1, valueRight.Item1), Access.WriteOnly)));
                                                    }
                                                    else
                                                    {
                                                        return OrType.Make<IOrType<(T, Access), IError>, No, IError>(new No());
                                                    }

                                                },
                                                error => OrType.Make<IOrType<(T, Access), IError>, No, IError>(error));
                                            }
                                            throw new NotImplementedException("what cast is this?!");




                                        },
                                        errorRight => OrType.Make<IOrType<(T, Access), IError>, No, IError>(errorRight)
                                        ),
                                    errorLeft => definateRight.SwitchReturns(
                                        valueRight => OrType.Make<IOrType<(T, Access), IError>, No, IError>(errorLeft),
                                        errorRight => OrType.Make<IOrType<(T, Access), IError>, No, IError>(Error.Cascaded("",new[] { errorLeft, errorRight }))
                                        )
                                    );
                            }
                            return OrType.Make<IOrType<(T, Access), IError>, No, IError>(new No());
                        },
                        rightError => OrType.Make<IOrType<(T, Access), IError>, No, IError>(rightError)),
                    leftError => right.SwitchReturns(
                        rightType => OrType.Make<IOrType<(T, Access), IError>, No, IError>(leftError),
                        rightError => OrType.Make<IOrType<(T, Access), IError>, No, IError>(Error.Cascaded("", new[] { leftError, rightError }))));
        }

        public static IOrType<IOrType<T, IError>, No, IError> TryGetReturn<T>(
            IOrType<T, IError> left, 
            IOrType<T, IError> right,
            Func<T, IOrType<IOrType<T, IError>, No, IError>> tryGetReturn,
            Func<IOrType<T, IError>, IOrType<T, IError>, T> make)
        {
            return left.SwitchReturns(
                leftReturns => right.SwitchReturns(
                    rightReturns => {

                        var leftReturn = tryGetReturn(leftReturns);
                        var rightReturn = tryGetReturn(rightReturns);

                        if (leftReturn is IIsDefinately<IOrType<T, IError>> definateLeft && rightReturn is IIsDefinately<IOrType<T, IError>> definateRight)
                        {
                            return OrType.Make<IOrType<T, IError>, No, IError>(OrType.Make<T, IError>(make(definateLeft.Value, definateRight.Value)));
                        }
                        return OrType.Make<IOrType<T, IError>, No, IError>(new No());

                    },
                    rightError => OrType.Make<IOrType<T, IError>, No, IError>(rightError)),
                leftError => right.SwitchReturns(
                    rightReturns => OrType.Make<IOrType<T, IError>, No, IError>(leftError),
                    rightError => OrType.Make<IOrType<T, IError>, No, IError>(Error.Cascaded("", new[] { leftError, rightError }))));

        }

        public static IOrType<IOrType<T, IError>, No, IError> TryGetInput<T>(
            IOrType<T, IError> left, 
            IOrType<T, IError> right,
            Func<T, IOrType<IOrType<T, IError>, No, IError>> tryGetInput,
            Func<IOrType<T, IError>, IOrType<T, IError>, T> make)
        {
            return left.SwitchReturns(
               leftReturns => right.SwitchReturns(
                   rightReturns => {

                       var leftReturn = tryGetInput(leftReturns);
                       var rightReturn = tryGetInput(rightReturns);

                       if (leftReturn is IIsDefinately<IOrType<T, IError>> definateLeft && rightReturn is IIsDefinately<IOrType<T, IError>> definateRight)
                       {
                           return OrType.Make<IOrType<T, IError>, No, IError>(OrType.Make<T, IError>(make(definateLeft.Value, definateRight.Value)));
                       }
                       return OrType.Make<IOrType<T, IError>, No, IError>(new No());

                   },
                   rightError => OrType.Make<IOrType<T, IError>, No, IError>(rightError)),
               leftError => right.SwitchReturns(
                   rightReturns => OrType.Make<IOrType<T, IError>, No, IError>(leftError),
                   rightError => OrType.Make<IOrType<T, IError>, No, IError>(Error.Cascaded("", new[] { leftError, rightError }))));
        }

    }

    public static class HasMembersLibrary {
        // for now the members all need to be the same type
        // say A is { Human thing; } and B is { Animal thing; }
        // B := A
        // B.thing := () > Method [Empty, Chicken] { .... }
        // breaks A, A.thing is a chicken
        // 
        // now 
        // A := B
        // B.thing := () > Method [Empty, Chicken] { .... }
        // agains breaks A, A.thing is a chicken

        // this is going to be more interesting when things can be read-only 
        // I think readonly means: you never get to changes this but it can change
        // I probably need another word for: this never changes

        // if B is { readonly Animal thing; } then
        // B := A
        // is ok, humnas are always animals and you can't set on be to breaak things
        // A := B is still not since B.thing might not be a Human  List<(IFrontendType, IFrontendType)>
        public static IOrType<bool, IError> CanAssign<T>(
            T from, 
            T to, 
            IReadOnlyList<(IKey key, IOrType<(T,Access), IError> type)> ourMembers,
            Func<T, IKey, IOrType<IOrType<(T,Access), IError>, No, IError>> tryGetMember,
            Func<T, T, List<(T, T)>, IOrType<bool, IError>> theyAreUs, // us, them, output
            List<(T, T)> assumeTrue)
        {

            if (assumeTrue.Contains((to, from)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((to, from));

            var list = new List<IOrType<IError[], No, Yes>>();

            foreach (var member in ourMembers)
            {
                var theirMember = tryGetMember(from,member.key);

                var ourMemberType = member.type;

                // this is horrifying
                // I think I need an extension
                list.Add(theirMember.SwitchReturns(
                    or => or.SwitchReturns(
                        theirType => ourMemberType.SwitchReturns(
                            ourType => {
                                if (ourType.Item2 == Access.ReadOnly && theirType.Item2 != Access.ReadOnly) {
                                    return OrType.Make<IError[], No, Yes>(new No());
                                }
                                if (ourType.Item2 == Access.ReadOnly && theirType.Item2 == Access.ReadOnly) {
                                    return theyAreUs(theirType.Item1, ourType.Item1, assumeTrue).SwitchReturns(
                                        boolean => boolean ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()),
                                        error => OrType.Make<IError[], No, Yes>(new[] { error }));
                                }
                                if (ourType.Item2 == Access.WriteOnly && theirType.Item2 != Access.WriteOnly)
                                {
                                    return OrType.Make<IError[], No, Yes>(new No());
                                }
                                if (ourType.Item2 == Access.WriteOnly && theirType.Item2 == Access.WriteOnly)
                                {
                                    return theyAreUs(ourType.Item1, theirType.Item1, assumeTrue).SwitchReturns(
                                        boolean => boolean ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()),
                                        error => OrType.Make<IError[], No, Yes>(new[] { error }));
                                }
                                if (ourType.Item2 == Access.ReadWrite && theirType.Item2 == Access.ReadWrite) {
                                    return theyAreUs(ourType.Item1, theirType.Item1, assumeTrue).SwitchReturns(
                                         boolean1 => theyAreUs(theirType.Item1, ourType.Item1, assumeTrue).SwitchReturns(
                                             boolean2 => boolean1 && boolean2 ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()), // if they are us and we are them the types are the same
                                             error2 => OrType.Make<IError[], No, Yes>(new[] { error2 })),
                                         error1 => theyAreUs(theirType.Item1, ourType.Item1, assumeTrue).SwitchReturns(
                                             boolean2 => OrType.Make<IError[], No, Yes>(new[] { error1 }),
                                             error2 => OrType.Make<IError[], No, Yes>(new[] { error1, error2 })));
                                }
                                if (ourType.Item2 == Access.ReadWrite && theirType.Item2 == Access.WriteOnly) {
                                    return theyAreUs(ourType.Item1, theirType.Item1, assumeTrue).SwitchReturns(
                                        boolean => boolean ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()),
                                        error => OrType.Make<IError[], No, Yes>(new[] { error }));
                                }
                                if (ourType.Item2 == Access.ReadWrite && theirType.Item2 == Access.ReadOnly)
                                {
                                    return theyAreUs(theirType.Item1, ourType.Item1, assumeTrue).SwitchReturns(
                                        boolean => boolean ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()),
                                        error => OrType.Make<IError[], No, Yes>(new[] { error }));
                                }
                                throw new NotImplementedException("what case is this");
                            },
                            ourError => OrType.Make<IError[], No, Yes>(new[] { ourError })),
                        error => ourMemberType.SwitchReturns(
                            ourType => OrType.Make<IError[], No, Yes>(new[] { error }),
                            ourError => OrType.Make<IError[], No, Yes>(new[] { error, ourError }))),
                    no => OrType.Make<IError[], No, Yes>(no),
                    error => ourMemberType.SwitchReturns(
                        ourType => OrType.Make<IError[], No, Yes>(new[] { error }),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { error, ourError }))));
            }

            if (list.All(x => x.Is3(out var _)))
            {
                return OrType.Make<bool, IError>(true);
            }

            if (list.Any(x => x.Is2(out var _)))
            {
                return OrType.Make<bool, IError>(false);
            }

            return OrType.Make<bool, IError>(Error.Cascaded("", list.Select(x => x.Possibly1()).OfType<IIsDefinately<IError[]>>().SelectMany(x => x.Value).ToArray()));
        }



        public static IOrType<IOrType<T, IError>, No, IError> TryGetMember<T>(
            IKey key, 
            IReadOnlyList<(IKey key, IOrType<T, IError> type)> ourMembers)
        {
            var matches = ourMembers.Where(x =>
                x.key.Equals(key));
            if (matches.Count() == 1)
            {
                return OrType.Make<IOrType<T, IError>, No, IError>(matches.First().type);
            }
            else
            {
                return OrType.Make<IOrType<T, IError>, No, IError>(new No());
            }
        }

    }

    public static class MethodLibrary {

        public static IOrType<bool, IError> CanAssign<T>(
            T from, 
            T to, 
            IOrType<T, IError> inputType,
            IOrType<T, IError> outputType,
            Func<T, IOrType<IOrType<T, IError>, No, IError>> tryGetInput,
            Func<T, IOrType<IOrType<T, IError>, No, IError>> tryGetReturn,
            Func<T, T, List<(T, T)>, IOrType<bool, IError>> theyAreUs,
            List<(T, T)> assumeTrue)
        {
            if (assumeTrue.Contains((to, from)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((to, from));

            var list = new List<IOrType<IError[], No, Yes>>();

            list.Add(tryGetInput(from).SwitchReturns(
                or => or.SwitchReturns(
                    theirType => inputType.SwitchReturns(
                        ourType => theyAreUs(ourType,theirType, assumeTrue).SwitchReturns(
                            boolean => boolean ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()),
                            error => OrType.Make<IError[], No, Yes>(new[] { error })),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { ourError })),
                    theirError => inputType.SwitchReturns(
                        ourType => OrType.Make<IError[], No, Yes>(new[] { theirError }),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { theirError, ourError }))),
                no => OrType.Make<IError[], No, Yes>(new No()),
                error => inputType.SwitchReturns(
                        ourType => OrType.Make<IError[], No, Yes>(new[] { error }),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { error, ourError }))));

            list.Add(tryGetReturn(from).SwitchReturns(
                or => or.SwitchReturns(
                    theirType => outputType.SwitchReturns(
                        ourType => theyAreUs(theirType,ourType, assumeTrue).SwitchReturns(
                            boolean => boolean ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()),
                            error => OrType.Make<IError[], No, Yes>(new[] { error })),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { ourError })),
                    theirError => outputType.SwitchReturns(
                        ourType => OrType.Make<IError[], No, Yes>(new[] { theirError }),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { theirError, ourError }))),
                no => OrType.Make<IError[], No, Yes>(new No()),
                error => outputType.SwitchReturns(
                        ourType => OrType.Make<IError[], No, Yes>(new[] { error }),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { error, ourError }))));

            if (list.All(x => x.Is3(out var _)))
            {
                return OrType.Make<bool, IError>(true);
            }

            if (list.Any(x => x.Is2(out var _)))
            {
                return OrType.Make<bool, IError>(false);
            }

            return OrType.Make<bool, IError>(Error.Cascaded("", list.Select(x => x.Possibly1()).OfType<IIsDefinately<IError[]>>().SelectMany(x => x.Value).ToArray()));

        }
    }
}
