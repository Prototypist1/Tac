using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;

namespace Tac.SemanticModel.CodeStuff
{

    public static class StaticSymbolsRegistry {
        private static readonly Prototypist.TaskChain.JumpBallConcurrent<List<string>> listLocker = new Prototypist.TaskChain.JumpBallConcurrent<List<string>>(new List<string>());
        public static string AddOrThrow(string s)
        {
            listLocker.Run(x =>
            {
                if (x.Contains(s))
                {
                    throw new Exception("Already added");
                }
                x.Add(s);
                return x;
            });
            return s;
        }
        // playing some crazy games here
        // I want each symbol to register itself instead of having a centeral list
        // unfo, ensuring static init is hard in C#

        // my solution:
        // 1. you get the list of symbols from StaticSymbolsRegistry.SymbolsRegistry
        // 2. SymbolsRegistry is partail each symbol creates a member
        //    when that member is initialized the symbol is added to StaticSymbolsRegistry
        // 
        // when you use StaticSymbolsRegistry.SymbolsRegistry
        // we create a SymbolsRegistry so all the symbols are added to listLocker
        // then we set SymbolsRegistry.Symbols to listLocker
        public static SymbolsRegistry SymbolsRegistry { get; set; } = CreateSymbolsRegistry();

        private static SymbolsRegistry CreateSymbolsRegistry() {
            var res = new SymbolsRegistry(listLocker.Run(x => x));
            return res;
        }
    }

    // this is not actually used
    // is it worth maintaining?
    // if it is not used can I trust it?
    public partial class SymbolsRegistry
    {
        public SymbolsRegistry(IEnumerable<string> symbols)
        {
            Symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
        }

        public IEnumerable<string> Symbols { get; }

    }

    internal class BinaryOperation
    {
        public delegate OrType<Tpn.IValue, IError> GetReturnedValue(
            Tpn.IStaticScope scope, 
            ISetUpContext context, 
            IOrType< ISetUpResult<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> left, 
            IOrType<ISetUpResult<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> right);
        public delegate IBox<T> Make<out T>(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right);

        public delegate Tpn.TypeProblem2.TypeReference ToTypeProblemThings(
            Tpn.IStaticScope scope, 
            ISetUpContext context, 
            ISetUpResult<IBox<IFrontendType<IVerifiableType>>,  Tpn.ITypeProblemNode> left,
            ISetUpResult<IBox<IFrontendType<IVerifiableType>>,  Tpn.ITypeProblemNode> right);
        public delegate T MakeBinaryType<out T>(IBox<IOrType<IFrontendType<IVerifiableType>,IError>> left, IBox<IOrType<IFrontendType<IVerifiableType>,IError>> right);

    }

    internal abstract class BinaryOperation<TLeft, TRight,TCodeElement> :  IConvertableFrontendCodeElement<TCodeElement>
        where TLeft : class, IFrontendCodeElement
        where TRight : class, IFrontendCodeElement
        where TCodeElement: class, ICodeElement
    {
        public IOrType<IBox<TLeft>,IError> Left { get; }
        public IOrType<IBox<TRight>,IError> Right { get; }
        public IOrType<IBox<IFrontendCodeElement>,IError>[] Operands
        {
            get
            {
                return new IOrType<IBox<IFrontendCodeElement>, IError>[] { Left, Right };
            }
        }
        
        public BinaryOperation(IOrType<IBox<TLeft>, IError> left, IOrType<IBox<TRight>, IError> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        public abstract IBuildIntention<TCodeElement> GetBuildIntention(IConversionContext context);

        public virtual IEnumerable<IError> Validate() {
            foreach (var item in Left.SwitchReturns(x => x.GetValue().Validate(), x => new List<IError>() { x }))
            {
                yield return item;
            }
            foreach (var item in Right.SwitchReturns(x => x.GetValue().Validate(), x => new List<IError>() { x }))
            {
                yield return item;
            }
        }
    }

    // TODO this is not really a type
    // or type is primitive
    // this is not a type
    internal abstract class BinaryTypeOperation<TLeft, TRight, TType>
        where TLeft : IFrontendType<IVerifiableType>
        where TRight : IFrontendType<IVerifiableType>
        where TType: IVerifiableType 
    { 
        public IBox<IOrType<TLeft, IError>> Left { get; }
        public IBox<IOrType<TRight, IError>> Right { get; }
        public IEnumerable<IBox<IFrontendType<IVerifiableType>>> Operands { get {
                // this make me sad,
                // if we could mark TLeft, TRight as classes and I would need these ugly casts
                // but a lot of types are structs
                // so I have casts
                // I assume this has something to do with boxing
                // the cast boxes
                yield return (IBox<IFrontendType<IVerifiableType>>)Left;
                yield return (IBox<IFrontendType<IVerifiableType>>)Right;
            }
        }

        public BinaryTypeOperation(IBox<IOrType<TLeft, IError>> left, IBox<IOrType<TRight, IError>> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }


        //public abstract IBuildIntention<TType> GetBuildIntention(IConversionContext context);

        public virtual IEnumerable<IError> Validate()
        {
            foreach (var error in Left.GetValue().SwitchReturns(x=>x.Validate(),x=>new[] { x}))
            {
                yield return error;
            }
            foreach (var error in Right.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
        }
    }

    internal static class ScopeExtensions {
        public static Tpn.IStaticScope EnterInitizaionScopeIfNessisary(this Tpn.IStaticScope staticScope ) {
            // this is for this:
            // y := 5
            // object { x := y + 1 }
            // x is on the obect
            // but the RHS is run in the initization scope
            // y := 5
            // object { z := x := y + 1 }
            // operations exculding := push us in to the InitizationScope
            if (staticScope is Tpn.TypeProblem2.Object objectScope)
            {
                return objectScope.InitizationScope;
            }
            return staticScope;
        }
    }

    internal class BinaryOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>>
        where TFrontendCodeElement : class, IConvertableFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        private readonly BinaryOperation.GetReturnedValue keyMaker;
        private readonly bool intoInitScope;

        public BinaryOperationMaker(string symbol, BinaryOperation.Make<TFrontendCodeElement> make,
                BinaryOperation.GetReturnedValue keyMaker,
                bool intoInitScope
            )
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.keyMaker = keyMaker ?? throw new ArgumentNullException(nameof(keyMaker));
            this.intoInitScope = intoInitScope;
        }

        public string Symbol { get; }
        private BinaryOperation.Make<TFrontendCodeElement> Make { get; }

        public ITokenMatching<ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(Symbol), out var _);

            return matching.ConvertIfMatched(match => {

                var left= OrType.Make< ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>( match.lhs.SafeCastTo(out ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode> _));
                var right = OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(match.rhs.SafeCastTo(out ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode> _));

                var res = new BinaryPopulateScope<TFrontendCodeElement, TCodeElement>(left, right, Make, keyMaker, intoInitScope);

                return res;
            }, tokenMatching);
        }


        //public static ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue> PopulateScope(
        //        IOrType< ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> left,
        //        IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right,
        //        BinaryOperation.Make<TFrontendCodeElement> make,
        //        BinaryOperation.GetReturnedValue key)
        //{
        //    return new BinaryPopulateScope(left,
        //         right,
        //         make,
        //         key);
        //}

        
    }

    internal class BinaryPopulateScope<TFrontendCodeElement,TCodeElement> : ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>
        where TFrontendCodeElement : class, IConvertableFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left;
        private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right;
        private readonly BinaryOperation.Make<TFrontendCodeElement> make;
        private readonly BinaryOperation.GetReturnedValue keyMaker;
        private readonly bool intoInitScope;

        public BinaryPopulateScope(
            IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left,
            IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right,
            BinaryOperation.Make<TFrontendCodeElement> make,
            BinaryOperation.GetReturnedValue key,
            bool intoInitScope)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.keyMaker = key ?? throw new ArgumentNullException(nameof(key));
            this.intoInitScope = intoInitScope;
        }

        public ISetUpResult<IBox<TFrontendCodeElement>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            if (intoInitScope)
            {
                scope = scope.EnterInitizaionScopeIfNessisary();
            }

            var nextLeft = left.TransformInner(x => x.Run(scope, context.CreateChildContext(this)));
            var nextRight = right.TransformInner(x => x.Run(scope, context.CreateChildContext(this)));
            var value = keyMaker(scope, context.CreateChildContext(this), nextLeft, nextRight);

            return new SetUpResult<IBox<TFrontendCodeElement>, Tpn.IValue>(new BinaryResolveReferance<TFrontendCodeElement, TCodeElement>(
                nextLeft.TransformInner(x => x.Resolve),
                nextRight.TransformInner(x => x.Resolve),
                make), value);
        }
    }


    internal class BinaryResolveReferance<TFrontendCodeElement, TCodeElement> : IResolve<IBox<TFrontendCodeElement>>
        where TFrontendCodeElement : class, IConvertableFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> left;
        public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> right;
        private readonly BinaryOperation.Make<TFrontendCodeElement> make;

        public BinaryResolveReferance(
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance1,
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance2,
            BinaryOperation.Make<TFrontendCodeElement> make)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }


        public IBox<TFrontendCodeElement> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {
            var res = make(
                left.TransformInner(x => x.Run(context, stack)),
                right.TransformInner(x => x.Run(context, stack)));
            return res;
        }
    }


    internal class BinaryTypeMaker : IMaker<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>>
    {
        private readonly BinaryOperation.ToTypeProblemThings toTypeProblemThings;

        public BinaryTypeMaker(string symbol, 
            //BinaryOperation.MakeBinaryType<IBox<IFrontendType<IVerifiableType>>> make,
            BinaryOperation.ToTypeProblemThings toTypeProblemThings
            )
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            //Make = make ?? throw new ArgumentNullException(nameof(make));
            this.toTypeProblemThings = toTypeProblemThings ?? throw new ArgumentNullException(nameof(toTypeProblemThings));
        }

        public string Symbol { get; }
        //private BinaryOperation.MakeBinaryType<IBox<IFrontendType<IVerifiableType>>> Make { get; }

        public ITokenMatching<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryTypeOperationMatcher(Symbol), out var match);
            if (matching is IMatchedTokenMatching matched)
            {
                //var left = matching.Context.Map.GetGreatestParent(match.lhs);
                //var right = matching.Context.Map.GetGreatestParent(match.rhs);

                //IOrType<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>, IError> leftType = OrType.Make<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>, IError>(Error.Other("Must be a type"));
                //if (left is IOrType<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>,IError> leftTypeMatched ) {
                //    leftType = leftTypeMatched;
                //}

                //IOrType<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>, IError> rightType = OrType.Make<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>, IError>(Error.Other("Must be a type"));
                //if (right is IOrType<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>, IError> rightTypeMatched)
                //{
                //    rightType = rightTypeMatched;
                //}

                var res = new BinaryPopulateScope(match.lhs, match.rhs, /*Make,*/ toTypeProblemThings);

                //matching.Context.Map.SetElementParent(match.lhs, res);
                //matching.Context.Map.SetElementParent(match.rhs, res);

                return TokenMatching<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                    tokenMatching, 
                    res,
                    matched.EndIndex
                    );
            }

            return TokenMatching<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    matching.Context);
        }


        //public static ISetUp<IFrontendType<IVerifiableType>, Tpn.TypeProblem2.TypeReference> PopulateScope(
        //    ISetUp<IConvertableFrontendType<IVerifiableType>, Tpn.ITypeProblemNode> left,
        //    ISetUp<IConvertableFrontendType<IVerifiableType>, Tpn.ITypeProblemNode> right,
        //    BinaryOperation.MakeBinaryType<IFrontendType<IVerifiableType>> make,
        //    BinaryOperation.ToTypeProblemThings toTypeProblemThings)
        //{
        //    return new BinaryPopulateScope( left,
        //         right,
        //         make,
        //         toTypeProblemThings);
        //}
        //public static IResolve<IFrontendType<IVerifiableType>> PopulateBoxes(IResolve<IConvertableFrontendType<IVerifiableType>> resolveReferance1,
        //        IResolve<IConvertableFrontendType<IVerifiableType>> resolveReferance2,
        //        BinaryOperation.MakeBinaryType<IFrontendType<IVerifiableType>> make)
        //{
        //    return new BinaryResolveReferance(resolveReferance1,
        //        resolveReferance2,
        //        make);
        //}



    }



    internal class BinaryPopulateScope : ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>
    {
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode> left;
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode> right;
        //private readonly BinaryOperation.MakeBinaryType<IBox<IFrontendType<IVerifiableType>>> make;
        private readonly BinaryOperation.ToTypeProblemThings toTypeProblemThings;

        public BinaryPopulateScope(
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode> left,
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode> right,
            //BinaryOperation.MakeBinaryType<IBox<IFrontendType<IVerifiableType>>> make,
            BinaryOperation.ToTypeProblemThings toTypeProblemThings)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            //this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.toTypeProblemThings = toTypeProblemThings ?? throw new ArgumentNullException(nameof(toTypeProblemThings));
        }


        public ISetUpResult<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            // TODO
            // this is something I don't much like
            // right runs first because of assign
            // in assign you might have something like
            // method [int;int] input { input < ? 2 if { 1 return; } else { input - 1 > fac * input return; } } =: fac
            // if the left runs first than fac will not be found
            // and so it will add it to the scope
            // but if the right is run first 
            // fac works
            // if I add an assign that goes the other way...
            // this will break

            // part of me just thinks 
            // force 'var' on member definition 
            var nextLeft = left.Run(scope, context.CreateChildContext(this));
            var nextRight = right.Run(scope, context.CreateChildContext(this));
            var type = toTypeProblemThings(scope, context.CreateChildContext(this), nextLeft, nextRight);

            return new SetUpResult<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>(new BinaryResolveReferance(
                nextLeft.Resolve,
                nextRight.Resolve,
                //make,
                type
                ), OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(type));
        }
    }


    internal class BinaryResolveReferance : IResolve<IBox<IFrontendType<IVerifiableType>>>
    {
        public readonly IResolve<IBox<IFrontendType<IVerifiableType>>> left;
        public readonly IResolve<IBox<IFrontendType<IVerifiableType>>> right;
        //private readonly BinaryOperation.MakeBinaryType<IBox<IFrontendType<IVerifiableType>>> make;
        private readonly Tpn.TypeProblem2.TypeReference type;

        public BinaryResolveReferance(
            IResolve<IBox<IFrontendType<IVerifiableType>>> resolveReferance1,
            IResolve<IBox<IFrontendType<IVerifiableType>>> resolveReferance2,
            //BinaryOperation.MakeBinaryType<IBox<IFrontendType<IVerifiableType>>> make,
            Tpn.TypeProblem2.TypeReference type)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            //this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        // I think IResolve<TCodeElement> should return TCodeElement instead of IBox<TCodeElement>
        // that will be expensive but I think it gives me more control
        public IBox<IFrontendType<IVerifiableType>> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {

            //var res = make(
            //    new BoxThenOr(left.Run(context)),
            //    new BoxThenOr(right.Run(context)));

            return new Box<IFrontendType<IVerifiableType>>( context.GetType(type, stack).Is1OrThrow());
        }
    }

    // maybe this is a good standard format for my tetering towers of Boxes and Ors
    internal class BoxThenOr : IBox<IFrontendType<IVerifiableType>>
    {
        private IBox<IOrType<IFrontendType<IVerifiableType>, IError>> orType;

        public BoxThenOr(IBox<IOrType<IFrontendType<IVerifiableType>, IError>> orType)
        {
            this.orType = orType ?? throw new ArgumentNullException(nameof(orType));
        }


        public IFrontendType<IVerifiableType> GetValue()
        {
            return orType.GetValue().Is1OrThrow();
        }
    }
}
