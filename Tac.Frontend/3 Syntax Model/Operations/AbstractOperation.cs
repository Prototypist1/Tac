using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
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
        // playing some fucking games here
        // I want each symbol to register itself in stead of having a centeral list
        // unfo, ensuring static init is hard in C#

        // my solution:
        // 1. you get the list of symbols from StaticSymbolsRegistry.SymbolsRegistry
        // 2. SymbolsRegistry is partail each symbol creates a member
        //    when that member is initialized the symbol is added to StaticSymbolsRegistry
        // 
        // when you use StaticSymbolsRegistry.SymbolsRegistry
        // we create a SymbolsRegistry so all the symbols are added to listLocker
        // then we set SymbolsRegistry.Symbols to listLocker
        public static SymbolsRegistry SymbolsRegistry = CreateSymbolsRegistry();

        private static SymbolsRegistry CreateSymbolsRegistry() {
            var res = new SymbolsRegistry
            {
                Symbols = listLocker.Run(x => x)
            };
            return res;
        }
    }

    public partial class SymbolsRegistry
    {
        public IEnumerable<string> Symbols { get; set; }

    }

    internal class BinaryOperation
    {
        public delegate Tpn.IValue GetReturnedValue(Tpn.IScope scope, IPopulateScopeContext context, IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> left, IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> right);
        public delegate IIsPossibly<T> Make<out T>(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right);

        public delegate Tpn.ITypeReference ToTypeProblemThings(Tpn.IScope scope, IPopulateScopeContext context, IResolvelizeScope<IFrontendType, ITypeProblemNode> left, IResolvelizeScope<IFrontendType, ITypeProblemNode> right);
        public delegate IIsPossibly<T> MakeBinaryType<out T>(IIsPossibly<IFrontendType> left, IIsPossibly<IFrontendType> right);


    }

    internal abstract class BinaryOperation<TLeft, TRight,TCodeElement> :  IConvertableFrontendCodeElement<TCodeElement>
        where TLeft : class, IFrontendCodeElement
        where TRight : class, IFrontendCodeElement
        where TCodeElement: class, ICodeElement
    {
        public IIsPossibly<TLeft> Left { get; }
        public IIsPossibly<TRight> Right { get; }
        public IIsPossibly<IFrontendCodeElement>[] Operands
        {
            get
            {
                return new IIsPossibly<IFrontendCodeElement>[] { Left, Right };
            }
        }
        
        public BinaryOperation(IIsPossibly<TLeft> left, IIsPossibly<TRight> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        
        public abstract IIsPossibly<IFrontendType> Returns();

        public abstract IBuildIntention<TCodeElement> GetBuildIntention(IConversionContext context);
    }



    internal abstract class BinaryTypeOperation<TLeft, TRight, TType> : IConvertableFrontendType<TType>
        where TLeft : IFrontendType
        where TRight : IFrontendType
        where TType: IVerifiableType 
    { 
        public IIsPossibly<TLeft> Left { get; }
        public IIsPossibly<TRight> Right { get; }
        public IEnumerable<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> Operands { get {
                // this make me sad,
                // if we could mark TLeft, TRight as classes and I would need these ugly casts
                // but a lot of types are structs
                // so I have casts
                // I assume this has something to do with boxing
                // the cast boxes
                yield return (IIsPossibly<IConvertableFrontendType<IVerifiableType>>)Left;
                yield return (IIsPossibly<IConvertableFrontendType<IVerifiableType >>)Right;
            }
        }

        public BinaryTypeOperation(IIsPossibly<TLeft> left, IIsPossibly<TRight> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }


        public abstract IBuildIntention<TType> GetBuildIntention(IConversionContext context);
    }



    internal class BinaryOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<IPopulateScope<TFrontendCodeElement, Tpn.IValue>>
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

        public ITokenMatching<IPopulateScope<TFrontendCodeElement, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(Symbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElement(match.rhs);

                return TokenMatching<IPopulateScope<TFrontendCodeElement, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new BinaryPopulateScope(left, right, Make, keyMaker));
            }

            return TokenMatching<IPopulateScope<TFrontendCodeElement, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


        public static IPopulateScope<TFrontendCodeElement, Tpn.IValue> PopulateScope(
                IPopulateScope<IFrontendCodeElement, ITypeProblemNode> left,
                IPopulateScope<IFrontendCodeElement, ITypeProblemNode> right,
                BinaryOperation.Make<TFrontendCodeElement> make,
                BinaryOperation.GetReturnedValue key)
        {
            return new BinaryPopulateScope(left,
                 right,
                 make,
                 key);
        }
        public static IPopulateBoxes<TFrontendCodeElement> PopulateBoxes(IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>> resolveReferance1,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>> resolveReferance2,
                BinaryOperation.Make<TFrontendCodeElement> make)
        {
            return new BinaryResolveReferance(resolveReferance1,
                resolveReferance2,
                make);
        }



        private class BinaryPopulateScope : IPopulateScope<TFrontendCodeElement, Tpn.IValue>
        {
            private readonly IPopulateScope<IFrontendCodeElement, ITypeProblemNode> left;
            private readonly IPopulateScope<IFrontendCodeElement, ITypeProblemNode> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;
            private readonly BinaryOperation.GetReturnedValue keyMaker;

            public BinaryPopulateScope(IPopulateScope<IFrontendCodeElement, ITypeProblemNode> left,
                IPopulateScope<IFrontendCodeElement, ITypeProblemNode> right,
                BinaryOperation.Make<TFrontendCodeElement> make,
                BinaryOperation.GetReturnedValue key)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.keyMaker = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IResolvelizeScope<TFrontendCodeElement, Tpn.IValue> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {
                var nextLeft = left.Run(scope, context);
                var nextRight = right.Run(scope, context);
                var value = keyMaker(scope, context,nextLeft, nextRight);

                return new BinaryFinalizeScope(value,
                    nextLeft,
                    nextRight,
                    make);
            }
        }


        private class BinaryFinalizeScope : IResolvelizeScope<TFrontendCodeElement, Tpn.IValue>
        {
            public readonly IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> left;
            public readonly IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;

            public BinaryFinalizeScope(
                Tpn.IValue setUpSideNode,
                IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> resolveReferance1,
                IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> resolveReferance2,
                BinaryOperation.Make<TFrontendCodeElement> make)
            {
                SetUpSideNode = setUpSideNode ?? throw new ArgumentNullException(nameof(setUpSideNode));
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public Tpn.IValue SetUpSideNode
            {
                get;
            }

            public IPopulateBoxes<TFrontendCodeElement> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new BinaryResolveReferance(left.Run(parent,context), right.Run(parent,context), make);
            }
        }

        private class BinaryResolveReferance : IPopulateBoxes<TFrontendCodeElement>
        {
            public readonly IPopulateBoxes<IFrontendCodeElement> left;
            public readonly IPopulateBoxes<IFrontendCodeElement> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;

            public BinaryResolveReferance(
                IPopulateBoxes<IFrontendCodeElement> resolveReferance1,
                IPopulateBoxes<IFrontendCodeElement> resolveReferance2,
                BinaryOperation.Make<TFrontendCodeElement> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }


            public IIsPossibly<TFrontendCodeElement> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                var res = make(
                    left.Run(scope,context),
                    right.Run(scope, context));
                return res;
            }
        }
    }


    internal class BinaryTypeMaker : IMaker<IPopulateScope<IWeakTypeReference,Tpn.ITypeReference>>
    {
        private readonly BinaryOperation.ToTypeProblemThings toTypeProblemThings;

        public BinaryTypeMaker(string symbol, BinaryOperation.MakeBinaryType<IWeakTypeReference> make,
            BinaryOperation.ToTypeProblemThings toTypeProblemThings
            )
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.toTypeProblemThings = toTypeProblemThings ?? throw new ArgumentNullException(nameof(toTypeProblemThings));
        }

        public string Symbol { get; }
        private BinaryOperation.MakeBinaryType<IWeakTypeReference> Make { get; }

        public ITokenMatching<IPopulateScope<IWeakTypeReference, Tpn.ITypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(Symbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseTypeLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElementType(match.rhs);

                return TokenMatching<IPopulateScope<IWeakTypeReference, Tpn.ITypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new BinaryPopulateScope(left, right, Make, toTypeProblemThings));
            }

            return TokenMatching<IPopulateScope<IWeakTypeReference, Tpn.ITypeReference>>.MakeNotMatch(
                    matching.Context);
        }


        public static IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> PopulateScope(
            IPopulateScope<IConvertableFrontendType<IVerifiableType>, ITypeProblemNode> left,
            IPopulateScope<IConvertableFrontendType<IVerifiableType>, ITypeProblemNode> right,
            BinaryOperation.MakeBinaryType<IWeakTypeReference> make,
            BinaryOperation.ToTypeProblemThings toTypeProblemThings)
        {
            return new BinaryPopulateScope( left,
                 right,
                 make,
                 toTypeProblemThings);
        }
        public static IPopulateBoxes<IWeakTypeReference> PopulateBoxes(IPopulateBoxes<IConvertableFrontendType<IVerifiableType>> resolveReferance1,
                IPopulateBoxes<IConvertableFrontendType<IVerifiableType>> resolveReferance2,
                BinaryOperation.MakeBinaryType<IWeakTypeReference> make)
        {
            return new BinaryResolveReferance(resolveReferance1,
                resolveReferance2,
                make);
        }



        private class BinaryPopulateScope : IPopulateScope<IWeakTypeReference, Tpn.ITypeReference>
        {
            private readonly IPopulateScope<IFrontendType, ITypeProblemNode> left;
            private readonly IPopulateScope<IFrontendType, ITypeProblemNode> right;
            private readonly BinaryOperation.MakeBinaryType<IWeakTypeReference> make;
            private readonly BinaryOperation.ToTypeProblemThings toTypeProblemThings;

            public BinaryPopulateScope(IPopulateScope<IFrontendType, ITypeProblemNode> left,
                IPopulateScope<IFrontendType, ITypeProblemNode> right,
                BinaryOperation.MakeBinaryType<IWeakTypeReference> make,
                BinaryOperation.ToTypeProblemThings toTypeProblemThings)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.toTypeProblemThings = toTypeProblemThings ?? throw new ArgumentNullException(nameof(toTypeProblemThings));
            }


            public IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> Run(Tpn.IScope scope, IPopulateScopeContext context)
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

                return new BinaryFinalizeScope(
                    nextLeft,
                    nextRight,
                    make,
                    type);
            }
        }


        private class BinaryFinalizeScope : IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference>
        {
            public readonly IResolvelizeScope<IFrontendType, ITypeProblemNode> left;
            public readonly IResolvelizeScope<IFrontendType, ITypeProblemNode> right;
            private readonly BinaryOperation.MakeBinaryType<IWeakTypeReference> make;

            public BinaryFinalizeScope(
                IResolvelizeScope<IFrontendType, ITypeProblemNode> resolveReferance1,
                IResolvelizeScope<IFrontendType, ITypeProblemNode> resolveReferance2,
                BinaryOperation.MakeBinaryType<IWeakTypeReference> make,
                Tpn.ITypeReference typeReference)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.SetUpSideNode = typeReference ?? throw new ArgumentNullException(nameof(typeReference));
            }

            public Tpn.ITypeReference SetUpSideNode
            {
                get;
            }

            public IPopulateBoxes<IWeakTypeReference> Run(IResolvableScope parent,IFinalizeScopeContext context)
            {
                return new BinaryResolveReferance(left.Run(parent,context), right.Run(parent,context), make);
            }
        }

        private class BinaryResolveReferance : IPopulateBoxes<IWeakTypeReference>
        {
            public readonly IPopulateBoxes<IFrontendType> left;
            public readonly IPopulateBoxes<IFrontendType> right;
            private readonly BinaryOperation.MakeBinaryType<IWeakTypeReference> make;

            public BinaryResolveReferance(
                IPopulateBoxes<IFrontendType> resolveReferance1,
                IPopulateBoxes<IFrontendType> resolveReferance2,
                BinaryOperation.MakeBinaryType<IWeakTypeReference> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }


            public IIsPossibly<IWeakTypeReference> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                var res = make(
                    left.Run(scope,context),
                    right.Run(scope, context));

                return res;
            }
        }
    }

}
