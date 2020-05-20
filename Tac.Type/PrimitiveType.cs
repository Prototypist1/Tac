using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.SyntaxModel.Elements.AtomicTypes
{

    // these types are shared by front and back

    // the back does not want to see the errors
    // anything that gets to it should be squeeky clean

    // the frontend wants these to have validate
    // and it wants to conver them to model type

    // and model has types too! ahh!

    public struct Yes { }
    public struct No { }

    //internal interface ITacType
    //{
    //    bool TheyAreUsThrowOnError(ITacType they, List<(ITacType, ITacType)> assumeTrue);
    //    IOrType<ITacType, No> TryGetMemberThrowOnError(IKey key);
    //    IOrType<ITacType, No> TryGetReturnThrowOnError();
    //    IOrType<ITacType, No> TryGetInputThrowOnError();
    //}

    public interface IErroryTacType 
    {
        // we need to pass around a list of assumed true
        // TheyAreUs is often called inside TheyAreUs
        // and the parameters to the inner can be the same as the outer
        // this happens in cases like: A { A x }  TheyAreUs B { B x }
        // this are actaully the same
        // another case A { B x },  B { C x },  C { A x }
        // these are the same as well
        IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue);
        IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key);
        IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn();
        IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput();
    }

    public static class OrTypeLibrary {


        public delegate bool IsOrType(IErroryTacType them, out IOrType<IErroryTacType, IError> left, out IOrType<IErroryTacType, IError> right);

        public static IOrType<bool, IError> CanAssign(
            IErroryTacType from,
            IsOrType check,
            IErroryTacType to,
            IOrType<IErroryTacType, IError> toLeft,
            IOrType<IErroryTacType, IError> toRight,
            List<(IErroryTacType, IErroryTacType)> assumeTrue)
        {
            if (assumeTrue.Contains((to, from)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((to, from));



            if (check(from, out var fromLeft, out var fromRight))
            {

                // both of their type must be assignable to one of our types...

                var leftRes = fromLeft.TransformInner(x =>
                {
                    return to.TheyAreUs(x, assumeTrue);
                });
                var rightRes = fromRight.TransformInner(x =>
                {
                    return to.TheyAreUs(x, assumeTrue);
                });


                return leftRes.SwitchReturns(x => rightRes.SwitchReturns(
                        y => OrType.Make<bool, IError>(x && y),
                        y => OrType.Make<bool, IError>(y)),
                    x => rightRes.SwitchReturns(
                        y => OrType.Make<bool, IError>(x),
                        y => OrType.Make<bool, IError>(Error.Cascaded("", new[] { x, y }))));

            }
            else
            {
                // they must be assignable to one of our types

                var leftRes = toLeft.TransformInner(x =>
                {
                    return x.TheyAreUs(from, assumeTrue);
                });
                var rightRes = toRight.TransformInner(x =>
                {
                    return x.TheyAreUs(from, assumeTrue);
                });


                return leftRes.SwitchReturns(x => rightRes.SwitchReturns(
                        y => OrType.Make<bool, IError>(x || y),
                        y => x ? OrType.Make<bool, IError>(true) : OrType.Make<bool, IError>(y)),
                    x => rightRes.SwitchReturns(
                        y => y ? OrType.Make<bool, IError>(true) : OrType.Make<bool, IError>(x),
                        y => OrType.Make<bool, IError>(Error.Cascaded("", new[] { x, y }))));
            }
        }

        public static IOrType<IOrType<IErroryTacType, IError>, No, IError> GetMember(IKey key, IOrType<IErroryTacType, IError> left, IOrType<IErroryTacType, IError> right) {
            return left.SwitchReturns(
                    leftType => right.SwitchReturns(
                        rightType =>
                        {
                            var leftMember = leftType.TryGetMember(key);
                            var rightMember = rightType.TryGetMember(key);

                            if (leftMember is IIsDefinately<IOrType<IErroryTacType, IError>> definateLeft && rightMember is IIsDefinately<IOrType<IErroryTacType, IError>> definateRight)
                            {
                                return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(OrType.Make<IErroryTacType, IError>(new FrontEndOrType(definateLeft.Value, definateRight.Value)));
                            }
                            return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
                        },
                        rightError => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(rightError)),
                    leftError => right.SwitchReturns(
                        rightType => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(leftError),
                        rightError => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(Error.Cascaded("", new[] { leftError, rightError }))));
        }

        public static IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn(IOrType<IErroryTacType, IError> left, IOrType<IErroryTacType, IError> right)
        {
            return left.SwitchReturns(
                leftReturns => right.SwitchReturns(
                    rightReturns => {

                        var leftReturn = leftReturns.TryGetReturn();
                        var rightReturn = rightReturns.TryGetReturn();

                        if (leftReturn is IIsDefinately<IOrType<IErroryTacType, IError>> definateLeft && rightReturn is IIsDefinately<IOrType<IErroryTacType, IError>> definateRight)
                        {
                            return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(OrType.Make<IErroryTacType, IError>(new FrontEndOrType(definateLeft.Value, definateRight.Value)));
                        }
                        return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());

                    },
                    rightError => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(rightError)),
                leftError => right.SwitchReturns(
                    rightReturns => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(leftError),
                    rightError => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(Error.Cascaded("", new[] { leftError, rightError }))));

        }

        public static IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput(IOrType<IErroryTacType, IError> left, IOrType<IErroryTacType, IError> right)
        {
            return left.SwitchReturns(
               leftReturns => right.SwitchReturns(
                   rightReturns => {

                       var leftReturn = leftReturns.TryGetInput();
                       var rightReturn = rightReturns.TryGetInput();

                       if (leftReturn is IIsDefinately<IOrType<IErroryTacType, IError>> definateLeft && rightReturn is IIsDefinately<IOrType<IErroryTacType, IError>> definateRight)
                       {
                           return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(OrType.Make<IErroryTacType, IError>(new FrontEndOrType(definateLeft.Value, definateRight.Value)));
                       }
                       return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());

                   },
                   rightError => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(rightError)),
               leftError => right.SwitchReturns(
                   rightReturns => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(leftError),
                   rightError => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(Error.Cascaded("", new[] { leftError, rightError }))));
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
        // I probably need another work for: this never changes

        // if B is { readonly Animal thing; } then
        // B := A
        // is ok, humnas are always animals and you can't set on be to breaak things
        // A := B is still not since B.thing might not be a Human  List<(IFrontendType, IFrontendType)>
        public static IOrType<bool, IError> CanAssign(IErroryTacType from, IErroryTacType to, IReadOnlyList<(IKey key, IOrType<IErroryTacType, IError> type)> ourMembers,  List<(IErroryTacType, IErroryTacType)> assumeTrue)
        {

            if (assumeTrue.Contains((to, from)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((to, from));

            var list = new List<IOrType<IError[], No, Yes>>();

            foreach (var member in ourMembers)
            {
                var theirMember = from.TryGetMember(member.key);

                var ourMemberType = member.type;

                // this is horrifying
                // I think I need an extension
                list.Add(theirMember.SwitchReturns(
                    or => or.SwitchReturns(
                        theirType => ourMemberType.SwitchReturns(
                            ourType => ourType.TheyAreUs(theirType, assumeTrue).SwitchReturns(
                                boolean1 => theirType.TheyAreUs(ourType, assumeTrue).SwitchReturns(
                                    boolean2 => boolean1 && boolean2 ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()), // if they are us and we are them the types are the same
                                    error2 => OrType.Make<IError[], No, Yes>(new[] { error2 })),
                                error1 => theirType.TheyAreUs(ourType, assumeTrue).SwitchReturns(
                                    boolean2 => OrType.Make<IError[], No, Yes>(new[] { error1 }),
                                    error2 => OrType.Make<IError[], No, Yes>(new[] { error1, error2 }))),
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

        public static IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key, IReadOnlyList<(IKey key, IOrType<IErroryTacType, IError> type)> ourMembers)
        {
            var matches = ourMembers.Where(x =>
                x.key.Equals(key));
            if (matches.Count() == 1)
            {
                return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(matches.First().type);
            }
            else
            {
                return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
            }
        }

    }

    public static class MethodLibrary {

        public static IOrType<bool, IError> CanAssign(
            IErroryTacType from, 
            IErroryTacType to, 
            IOrType<IErroryTacType, IError> inputType,
            IOrType<IErroryTacType, IError> outputType, List<(IErroryTacType, IErroryTacType)> assumeTrue)
        {
            if (assumeTrue.Contains((to, from)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((to, from));

            var list = new List<IOrType<IError[], No, Yes>>();

            list.Add(from.TryGetInput().SwitchReturns(
                or => or.SwitchReturns(
                    theirType => inputType.SwitchReturns(
                        ourType => ourType.TheyAreUs(theirType, assumeTrue).SwitchReturns(
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

            list.Add(from.TryGetReturn().SwitchReturns(
                or => or.SwitchReturns(
                    theirType => outputType.SwitchReturns(
                        ourType => theirType.TheyAreUs(ourType, assumeTrue).SwitchReturns(
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
