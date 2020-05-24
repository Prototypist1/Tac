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
        public delegate OrType<Tpn.IValue, IError> GetReturnedValue(Tpn.IStaticScope scope, ISetUpContext context, IOrType< ISetUpResult<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> left, IOrType<ISetUpResult<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> right);
        public delegate IBox<T> Make<out T>(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right);

        public delegate Tpn.TypeProblem2.TypeReference ToTypeProblemThings(Tpn.IStaticScope scope, ISetUpContext context, ISetUpResult<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode> left, ISetUpResult<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode> right);
        public delegate T MakeBinaryType<out T>(IOrType<IBox<IFrontendType>,IError> left, IOrType<IBox<IFrontendType>,IError> right);


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
        where TLeft : IFrontendType
        where TRight : IFrontendType
        where TType: IVerifiableType 
    { 
        public IOrType<IBox<TLeft>, IError> Left { get; }
        public IOrType<IBox<TRight>, IError> Right { get; }
        public IEnumerable<IBox<IConvertableFrontendType<IVerifiableType>>> Operands { get {
                // this make me sad,
                // if we could mark TLeft, TRight as classes and I would need these ugly casts
                // but a lot of types are structs
                // so I have casts
                // I assume this has something to do with boxing
                // the cast boxes
                yield return (IBox<IConvertableFrontendType<IVerifiableType>>)Left;
                yield return (IBox<IConvertableFrontendType<IVerifiableType>>)Right;
            }
        }

        public BinaryTypeOperation(IOrType<IBox<TLeft>,IError> left, IOrType<IBox<TRight>, IError> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }


        public abstract IBuildIntention<TType> GetBuildIntention(IConversionContext context);

        public virtual IEnumerable<IError> Validate()
        {
            foreach (var error in Left.SwitchReturns(x=>x.GetValue().Validate(),x=>new[] { x}))
            {
                yield return error;
            }
            foreach (var error in Right.SwitchReturns(x => x.GetValue().Validate(), x => new[] { x }))
            {
                yield return error;
            }
        }
    }



    internal class BinaryOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>>
        where TFrontendCodeElement : class, IConvertableFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        private readonly BinaryOperation.GetReturnedValue keyMaker;

        public BinaryOperationMaker(string symbol, BinaryOperation.Make<TFrontendCodeElement> make,
                BinaryOperation.GetReturnedValue keyMaker
            )
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.keyMaker = keyMaker ?? throw new ArgumentNullException(nameof(keyMaker));
        }

        public string Symbol { get; }
        private BinaryOperation.Make<TFrontendCodeElement> Make { get; }

        public ITokenMatching<ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var matching = tokenMatching
                .HasStruct(new BinaryOperationMatcher(Symbol), out var _);

            return matching.ConvertIfMatched(match => new BinaryPopulateScope(matching.Context.ParseLine(match.perface), matching.Context.ParseParenthesisOrElement(match.rhs), Make, keyMaker));
        }


        public static ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue> PopulateScope(
                IOrType< ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> left,
                IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right,
                BinaryOperation.Make<TFrontendCodeElement> make,
                BinaryOperation.GetReturnedValue key)
        {
            return new BinaryPopulateScope(left,
                 right,
                 make,
                 key);
        }
        //public static IResolve<TFrontendCodeElement> PopulateBoxes(
        //        IOrType<IResolve<IConvertableFrontendCodeElement<ICodeElement>>, IError> resolveReferance1,
        //        IOrType<IResolve<IConvertableFrontendCodeElement<ICodeElement>>, IError> resolveReferance2,
        //        BinaryOperation.Make<TFrontendCodeElement> make)
        //{
        //    return new BinaryResolveReferance(
        //        resolveReferance1,
        //        resolveReferance2,
        //        make);
        //}



        private class BinaryPopulateScope : ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>
        {
            private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left;
            private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;
            private readonly BinaryOperation.GetReturnedValue keyMaker;

            public BinaryPopulateScope(
                IOrType< ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> left,
                IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> right,
                BinaryOperation.Make<TFrontendCodeElement> make,
                BinaryOperation.GetReturnedValue key)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.keyMaker = key ?? throw new ArgumentNullException(nameof(key));
            }

            public ISetUpResult<IBox<TFrontendCodeElement>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
            {
                var nextLeft = left.TransformInner(x=>x.Run(scope, context));
                var nextRight = right.TransformInner(x => x.Run(scope, context));
                var value = keyMaker(scope, context,nextLeft, nextRight);

                return new SetUpResult<IBox<TFrontendCodeElement>, Tpn.IValue>(new BinaryResolveReferance(
                    nextLeft.TransformInner(x=>x.Resolve),
                    nextRight.TransformInner(x => x.Resolve),
                    make), value);
            }
        }


        private class BinaryResolveReferance : IResolve<IBox<TFrontendCodeElement>>
        {
            public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> left;
            public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;

            public BinaryResolveReferance(
                IOrType< IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance1,
                IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance2,
                BinaryOperation.Make<TFrontendCodeElement> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }


            public IBox<TFrontendCodeElement> Run(Tpn.ITypeSolution context)
            {
                var res = make(
                    left.TransformInner(x=>x.Run(context)),
                    right.TransformInner(x => x.Run( context)));
                return res;
            }
        }
    }


    internal class BinaryTypeMaker : IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>
    {
        private readonly BinaryOperation.ToTypeProblemThings toTypeProblemThings;

        public BinaryTypeMaker(string symbol, BinaryOperation.MakeBinaryType<IOrType<IBox<IFrontendType>, IError>> make,
            BinaryOperation.ToTypeProblemThings toTypeProblemThings
            )
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.toTypeProblemThings = toTypeProblemThings ?? throw new ArgumentNullException(nameof(toTypeProblemThings));
        }

        public string Symbol { get; }
        private BinaryOperation.MakeBinaryType<IOrType<IBox<IFrontendType>, IError>> Make { get; }

        public ITokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .HasStruct(new BinaryOperationMatcher(Symbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseTypeLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElementType(match.rhs);

                return TokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new BinaryPopulateScope(left, right, Make, toTypeProblemThings));
            }

            return TokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    matching.Context);
        }


        //public static ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> PopulateScope(
        //    ISetUp<IConvertableFrontendType<IVerifiableType>, Tpn.ITypeProblemNode> left,
        //    ISetUp<IConvertableFrontendType<IVerifiableType>, Tpn.ITypeProblemNode> right,
        //    BinaryOperation.MakeBinaryType<IFrontendType> make,
        //    BinaryOperation.ToTypeProblemThings toTypeProblemThings)
        //{
        //    return new BinaryPopulateScope( left,
        //         right,
        //         make,
        //         toTypeProblemThings);
        //}
        //public static IResolve<IFrontendType> PopulateBoxes(IResolve<IConvertableFrontendType<IVerifiableType>> resolveReferance1,
        //        IResolve<IConvertableFrontendType<IVerifiableType>> resolveReferance2,
        //        BinaryOperation.MakeBinaryType<IFrontendType> make)
        //{
        //    return new BinaryResolveReferance(resolveReferance1,
        //        resolveReferance2,
        //        make);
        //}



        private class BinaryPopulateScope : ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>
        {
            private readonly ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode> left;
            private readonly ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode> right;
            private readonly BinaryOperation.MakeBinaryType<IOrType<IBox<IFrontendType>, IError>> make;
            private readonly BinaryOperation.ToTypeProblemThings toTypeProblemThings;

            public BinaryPopulateScope(
                ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode> left,
                ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode> right,
                BinaryOperation.MakeBinaryType<IOrType<IBox<IFrontendType>, IError>> make,
                BinaryOperation.ToTypeProblemThings toTypeProblemThings)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.toTypeProblemThings = toTypeProblemThings ?? throw new ArgumentNullException(nameof(toTypeProblemThings));
            }


            public ISetUpResult<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> Run(Tpn.IStaticScope scope, ISetUpContext context)
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
                var nextLeft = left.Run(scope, context);
                var nextRight = right.Run(scope, context);
                var type = toTypeProblemThings(scope, context, nextLeft, nextRight);

                return new SetUpResult<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>(new BinaryResolveReferance(
                    nextLeft.Resolve,
                    nextRight.Resolve,
                    make
                    ), OrType.Make<Tpn.TypeProblem2.TypeReference, IError>( type));
            }
        }


        private class BinaryResolveReferance : IResolve<IOrType<IBox<IFrontendType>, IError>>
        {
            public readonly IResolve<IOrType<IBox< IFrontendType>, IError>> left;
            public readonly IResolve<IOrType<IBox<IFrontendType>, IError>> right;
            private readonly BinaryOperation.MakeBinaryType<IOrType<IBox<IFrontendType>, IError>> make;

            public BinaryResolveReferance(
                IResolve<IOrType<IBox<IFrontendType>, IError>> resolveReferance1,
                IResolve<IOrType<IBox<IFrontendType>, IError>> resolveReferance2,
                BinaryOperation.MakeBinaryType<IOrType<IBox<IFrontendType>, IError>> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            // I think IResolve<TCodeElement> should return TCodeElement instead of IBox<TCodeElement>
            // that will be expensive but I think it gives me more control
            public IOrType<IBox<IFrontendType>, IError> Run(Tpn.ITypeSolution context)
            {
                var res = make(
                    left.Run(context),
                    right.Run( context));

                return res;
            }
        }
    }

}
