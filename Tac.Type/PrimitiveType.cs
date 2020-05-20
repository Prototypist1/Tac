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


    //public static class TacTypeLibrary {
    //    public static class OrType { 

    //    }
    //}

    //internal static class TypeExtensions{
    //    public static IOrType<IErroryTacType,IError> UnwrapRefrence(this IErroryTacType frontendType) 
    //    {
    //        if (frontendType is RefType refType) {
    //            return refType.inner;
    //        }
    //        return OrType.Make< IErroryTacType,IError >(frontendType);
    //    }
    //}


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

    internal class FrontEndOrType : IErroryTacType
    {
        private readonly IOrType<IErroryTacType,IError> left, right;

        public FrontEndOrType(IOrType<IErroryTacType, IError> left, IOrType<IErroryTacType, IError> right)
        {
            // TODO this is wrong
            // I think
            // I don't think this should be a validation error
            // it should be an exception at converstion type
            // Idk this design is a little werid
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }

        // ugh
        // this probaly needs to be able to return an erro
        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue)
        {
            if (assumeTrue.Contains((this, they))) {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((this, they));



            if (they.SafeIs(out FrontEndOrType frontEndOrType))
            {

                // both of their type must be assignable to one of our types...

                var leftRes = frontEndOrType.left.TransformInner(x =>
                {
                    return TheyAreUs(x, assumeTrue);
                });
                var rightRes = frontEndOrType.right.TransformInner(x =>
                {
                    return TheyAreUs(x, assumeTrue);
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

                var leftRes = left.TransformInner(x =>
                {
                    return x.TheyAreUs(they, assumeTrue);
                });
                var rightRes = right.TransformInner(x =>
                {
                    return x.TheyAreUs(they, assumeTrue);
                });


                return leftRes.SwitchReturns(x => rightRes.SwitchReturns(
                        y => OrType.Make<bool, IError>(x || y),
                        y => x ? OrType.Make<bool, IError>(true) : OrType.Make<bool, IError>(y)),
                    x => rightRes.SwitchReturns(
                        y => y ? OrType.Make<bool, IError>(true) : OrType.Make<bool, IError>(x),
                        y => OrType.Make<bool, IError>(Error.Cascaded("", new[] { x, y }))));
            }
        }

        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key)
        {

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

        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn()
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
                    rightError => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(Error.Cascaded("",new[] { leftError,rightError}))));

        }

        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput()
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

    internal class HasMembersType : IErroryTacType
    {

        private struct Yes { }


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
        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue) {

            if (assumeTrue.Contains((this, they)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((this, they));

            var list = new List<IOrType<IError[], No, Yes>>();

            foreach (var member in weakScope)
            {
                var theirMember = they.TryGetMember(member.key);

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

            if (list.All(x =>x.Is3(out var _))) {
                return OrType.Make<bool, IError>(true);
            }

            if (list.Any(x => x.Is2(out var _)))
            {
                return OrType.Make<bool, IError>(false);
            }

            return OrType.Make<bool, IError>(Error.Cascaded("", list.Select(x=> x.Possibly1()).OfType<IIsDefinately<IError[]>>().SelectMany(x=>x.Value).ToArray()));
        }
        
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key)
        {
            var matches = weakScope.Where(x => 
                x.key.Equals(key));
            if (matches.Count() == 1)
            {
                return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(matches.First().type);
            }
            else { 
                return OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
            }
        }

        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());

        //public readonly IReadOnlyDictionary<IKey, IOrType<IFrontendType, IError>> members;

        public readonly IReadOnlyList<(IKey key, IOrType<IErroryTacType, IError> type)> weakScope;

        public HasMembersType(IReadOnlyList<(IKey key, IOrType<IErroryTacType, IError> type)> weakScope)
        {
            this.weakScope = weakScope ?? throw new ArgumentNullException(nameof(weakScope));
        }
    }

    internal class MethodType : IErroryTacType
    {

        private struct Yes { }

        // is the meta-data here worth capturing
        public static MethodType ImplementationType(
            IOrType<IErroryTacType, IError> inputType,
            IOrType<IErroryTacType, IError> outputType,
            IOrType<IErroryTacType, IError> contextType)
        {
            return new MethodType(
                contextType,
                OrType.Make<IErroryTacType, IError>(new MethodType(inputType, outputType)));
        }

        public MethodType(
            IOrType<IErroryTacType, IError> inputType,
            IOrType<IErroryTacType, IError> outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IOrType<IErroryTacType, IError> InputType { get; }
        public IOrType<IErroryTacType, IError> OutputType { get; }

        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue)
        {
            if (assumeTrue.Contains((this, they)))
            {
                return OrType.Make<bool, IError>(true);
            }
            assumeTrue.Add((this, they));

            var list = new List<IOrType<IError[], No, Yes>>();

            list.Add(they.TryGetInput().SwitchReturns(
                or => or.SwitchReturns(
                    theirType => InputType.SwitchReturns(
                        ourType => ourType.TheyAreUs(theirType, assumeTrue).SwitchReturns(
                            boolean => boolean ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()),
                            error => OrType.Make<IError[], No, Yes>(new[] { error })),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { ourError })),
                    theirError => InputType.SwitchReturns(
                        ourType => OrType.Make<IError[], No, Yes>(new[] { theirError }),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { theirError, ourError }))),
                no => OrType.Make<IError[], No, Yes>(new No()),
                error => InputType.SwitchReturns(
                        ourType => OrType.Make<IError[], No, Yes>(new[] { error }),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { error, ourError }))));

            list.Add(they.TryGetReturn().SwitchReturns(
                or => or.SwitchReturns(
                    theirType => OutputType.SwitchReturns(
                        ourType => theirType.TheyAreUs(ourType, assumeTrue).SwitchReturns(
                            boolean => boolean ? OrType.Make<IError[], No, Yes>(new Yes()) : OrType.Make<IError[], No, Yes>(new No()),
                            error => OrType.Make<IError[], No, Yes>(new[] { error })),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { ourError })),
                    theirError => OutputType.SwitchReturns(
                        ourType => OrType.Make<IError[], No, Yes>(new[] { theirError }),
                        ourError => OrType.Make<IError[], No, Yes>(new[] { theirError, ourError }))),
                no => OrType.Make<IError[], No, Yes>(new No()),
                error => OutputType.SwitchReturns(
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
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());


        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(OutputType);
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(InputType);
    }
    // still bad at structs is this a struct?
    //internal struct HowTypesThinkOfMembers {
    //    public IOrType<IFrontendType, IError> orType;
    //    public IKey key;
    //}

    // reference is a type!
    // but it probably does not mean what you think it means
    // it really means you can assign to it
    // this is what is returned by member and member reference 
    internal class RefType : IErroryTacType {

        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());

        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue)
        {
            // the method calling this
            // is in charge of unwrapping
            throw new Exception("I don't think this should ever happen");

            // this is a bit of a smell
        }

        public readonly IOrType< IErroryTacType,IError> inner;

        public RefType(IOrType<IErroryTacType, IError> inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
    }

    internal class BlockType : IErroryTacType
    {
        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue) => OrType.Make<bool, IError> (they is BlockType);
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
    }

    internal struct StringType : IErroryTacType
    {

        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue) => OrType.Make<bool, IError>(they is StringType);
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
    }
    internal struct EmptyType : IErroryTacType
    {
        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue) => OrType.Make<bool, IError>(they is EmptyType);
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
    }

    internal struct NumberType : IErroryTacType
    {
        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue) => OrType.Make<bool, IError>(they is NumberType);
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
    }

    // I don't think this is a type....
    // placeholders are effectively defined by constraints really just metadata
    internal interface IGenericTypeParameterPlacholder //: IFrontendType
    {
        IOrType<NameKey, ImplicitKey> Key { get; }
    }

    internal struct GenericTypeParameterPlacholder : IGenericTypeParameterPlacholder, IEquatable<GenericTypeParameterPlacholder>
    {
        public GenericTypeParameterPlacholder(IOrType<NameKey, ImplicitKey> key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IOrType<NameKey, ImplicitKey> Key { get; }

        public override bool Equals(object? obj)
        {
            return obj is GenericTypeParameterPlacholder placholder && Equals(placholder);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        public bool Equals(GenericTypeParameterPlacholder placholder)
        {
            return EqualityComparer<IOrType<NameKey, ImplicitKey>>.Default.Equals(Key, placholder.Key);
        }
    }

    internal struct AnyType : IErroryTacType
    {
        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue) => OrType.Make<bool, IError>(true);
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
    }

    internal struct BooleanType : IErroryTacType
    {
        public IOrType<bool, IError> TheyAreUs(IErroryTacType they, List<(IErroryTacType, IErroryTacType)> assumeTrue) => OrType.Make<bool, IError>(they is BooleanType);

        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
        public IOrType<IOrType<IErroryTacType, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IErroryTacType, IError>, No, IError>(new No());
    }

}
