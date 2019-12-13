﻿using Prototypist.LeftToRight;
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
        public delegate Tpn.IValue GetReturnedValue(Tpn.IScope scope, ISetUpContext context, ISetUpResult<IFrontendCodeElement, Tpn.ITypeProblemNode> left, ISetUpResult<IFrontendCodeElement, Tpn.ITypeProblemNode> right);
        public delegate IBox<T> Make<out T>(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right);

        public delegate Tpn.TypeProblem2.TypeReference ToTypeProblemThings(Tpn.IScope scope, ISetUpContext context, ISetUpResult<IFrontendType, Tpn.ITypeProblemNode> left, ISetUpResult<IFrontendType, Tpn.ITypeProblemNode> right);
        public delegate IBox<T> MakeBinaryType<out T>(IBox<IFrontendType> left, IBox<IFrontendType> right);


    }

    internal abstract class BinaryOperation<TLeft, TRight,TCodeElement> :  IConvertableFrontendCodeElement<TCodeElement>
        where TLeft : class, IFrontendCodeElement
        where TRight : class, IFrontendCodeElement
        where TCodeElement: class, ICodeElement
    {
        public IBox<TLeft> Left { get; }
        public IBox<TRight> Right { get; }
        public IBox<IFrontendCodeElement>[] Operands
        {
            get
            {
                return new IBox<IFrontendCodeElement>[] { Left, Right };
            }
        }
        
        public BinaryOperation(IBox<TLeft> left, IBox<TRight> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        public abstract IBuildIntention<TCodeElement> GetBuildIntention(IConversionContext context);
    }



    internal abstract class BinaryTypeOperation<TLeft, TRight, TType> : IConvertableFrontendType<TType>
        where TLeft : IFrontendType
        where TRight : IFrontendType
        where TType: IVerifiableType 
    { 
        public IBox<TLeft> Left { get; }
        public IBox<TRight> Right { get; }
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

        public BinaryTypeOperation(IBox<TLeft> left, IBox<TRight> right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }


        public abstract IBuildIntention<TType> GetBuildIntention(IConversionContext context);
    }



    internal class BinaryOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<ISetUp<TFrontendCodeElement, Tpn.IValue>>
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

        public ITokenMatching<ISetUp<TFrontendCodeElement, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(Symbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElement(match.rhs);

                return TokenMatching<ISetUp<TFrontendCodeElement, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new BinaryPopulateScope(left, right, Make, keyMaker));
            }

            return TokenMatching<ISetUp<TFrontendCodeElement, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


        public static ISetUp<TFrontendCodeElement, Tpn.IValue> PopulateScope(
                ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> left,
                ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> right,
                BinaryOperation.Make<TFrontendCodeElement> make,
                BinaryOperation.GetReturnedValue key)
        {
            return new BinaryPopulateScope(left,
                 right,
                 make,
                 key);
        }
        public static IResolve<TFrontendCodeElement> PopulateBoxes(IResolve<IConvertableFrontendCodeElement<ICodeElement>> resolveReferance1,
                IResolve<IConvertableFrontendCodeElement<ICodeElement>> resolveReferance2,
                BinaryOperation.Make<TFrontendCodeElement> make)
        {
            return new BinaryResolveReferance(resolveReferance1,
                resolveReferance2,
                make);
        }



        private class BinaryPopulateScope : ISetUp<TFrontendCodeElement, Tpn.IValue>
        {
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> left;
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;
            private readonly BinaryOperation.GetReturnedValue keyMaker;

            public BinaryPopulateScope(ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> left,
                ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> right,
                BinaryOperation.Make<TFrontendCodeElement> make,
                BinaryOperation.GetReturnedValue key)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.keyMaker = key ?? throw new ArgumentNullException(nameof(key));
            }

            public ISetUpResult<TFrontendCodeElement, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var nextLeft = left.Run(scope, context);
                var nextRight = right.Run(scope, context);
                var value = keyMaker(scope, context,nextLeft, nextRight);

                return new SetUpResult<TFrontendCodeElement, Tpn.IValue>(new BinaryResolveReferance(
                    nextLeft.Resolve,
                    nextRight.Resolve,
                    make), value);
            }
        }


        private class BinaryResolveReferance : IResolve<TFrontendCodeElement>
        {
            public readonly IResolve<IFrontendCodeElement> left;
            public readonly IResolve<IFrontendCodeElement> right;
            private readonly BinaryOperation.Make<TFrontendCodeElement> make;

            public BinaryResolveReferance(
                IResolve<IFrontendCodeElement> resolveReferance1,
                IResolve<IFrontendCodeElement> resolveReferance2,
                BinaryOperation.Make<TFrontendCodeElement> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }


            public IBox<TFrontendCodeElement> Run(Tpn.ITypeSolution context)
            {
                var res = make(
                    left.Run(context),
                    right.Run( context));
                return res;
            }
        }
    }


    internal class BinaryTypeMaker : IMaker<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>
    {
        private readonly BinaryOperation.ToTypeProblemThings toTypeProblemThings;

        public BinaryTypeMaker(string symbol, BinaryOperation.MakeBinaryType<IFrontendType> make,
            BinaryOperation.ToTypeProblemThings toTypeProblemThings
            )
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.toTypeProblemThings = toTypeProblemThings ?? throw new ArgumentNullException(nameof(toTypeProblemThings));
        }

        public string Symbol { get; }
        private BinaryOperation.MakeBinaryType<IFrontendType> Make { get; }

        public ITokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(Symbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseTypeLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElementType(match.rhs);

                return TokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new BinaryPopulateScope(left, right, Make, toTypeProblemThings));
            }

            return TokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    matching.Context);
        }


        public static ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> PopulateScope(
            ISetUp<IConvertableFrontendType<IVerifiableType>, Tpn.ITypeProblemNode> left,
            ISetUp<IConvertableFrontendType<IVerifiableType>, Tpn.ITypeProblemNode> right,
            BinaryOperation.MakeBinaryType<IFrontendType> make,
            BinaryOperation.ToTypeProblemThings toTypeProblemThings)
        {
            return new BinaryPopulateScope( left,
                 right,
                 make,
                 toTypeProblemThings);
        }
        public static IResolve<IFrontendType> PopulateBoxes(IResolve<IConvertableFrontendType<IVerifiableType>> resolveReferance1,
                IResolve<IConvertableFrontendType<IVerifiableType>> resolveReferance2,
                BinaryOperation.MakeBinaryType<IFrontendType> make)
        {
            return new BinaryResolveReferance(resolveReferance1,
                resolveReferance2,
                make);
        }



        private class BinaryPopulateScope : ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>
        {
            private readonly ISetUp<IFrontendType, Tpn.ITypeProblemNode> left;
            private readonly ISetUp<IFrontendType, Tpn.ITypeProblemNode> right;
            private readonly BinaryOperation.MakeBinaryType<IFrontendType> make;
            private readonly BinaryOperation.ToTypeProblemThings toTypeProblemThings;

            public BinaryPopulateScope(ISetUp<IFrontendType, Tpn.ITypeProblemNode> left,
                ISetUp<IFrontendType, Tpn.ITypeProblemNode> right,
                BinaryOperation.MakeBinaryType<IFrontendType> make,
                BinaryOperation.ToTypeProblemThings toTypeProblemThings)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.toTypeProblemThings = toTypeProblemThings ?? throw new ArgumentNullException(nameof(toTypeProblemThings));
            }


            public ISetUpResult<IFrontendType, Tpn.TypeProblem2.TypeReference> Run(Tpn.IScope scope, ISetUpContext context)
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

                return new SetUpResult<IFrontendType, Tpn.TypeProblem2.TypeReference>(new BinaryResolveReferance(
                    nextLeft.Resolve,
                    nextRight.Resolve,
                    make
                    ), type);
            }
        }


        private class BinaryResolveReferance : IResolve<IFrontendType>
        {
            public readonly IResolve<IFrontendType> left;
            public readonly IResolve<IFrontendType> right;
            private readonly BinaryOperation.MakeBinaryType<IFrontendType> make;

            public BinaryResolveReferance(
                IResolve<IFrontendType> resolveReferance1,
                IResolve<IFrontendType> resolveReferance2,
                BinaryOperation.MakeBinaryType<IFrontendType> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }


            public IBox<IFrontendType> Run(Tpn.ITypeSolution context)
            {
                var res = make(
                    left.Run(context),
                    right.Run( context));

                return res;
            }
        }
    }

}
