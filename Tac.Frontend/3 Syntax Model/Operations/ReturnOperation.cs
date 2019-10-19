using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticReturnSymbol = StaticSymbolsRegistry.AddOrThrow("return");
        public readonly string ReturnSymbol = StaticReturnSymbol;
    }
}


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> StaticReturnMaker = AddOperationMatcher(() => new ReturnOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> ReturnMaker = StaticReturnMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model.Operations
{
    internal class WeakReturnOperation : TrailingOperation, IConvertableFrontendCodeElement<IReturnOperation>
    {
        public WeakReturnOperation(IIsPossibly<IFrontendCodeElement> result)
        {
            Result = result;
        }
        
        public IIsPossibly<IFrontendCodeElement> Result { get; }
        
        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IConvertableFrontendType<IVerifiableType>>(PrimitiveTypes.CreateEmptyType());
        }

        public IBuildIntention<IReturnOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ReturnOperation.Create();
            return new BuildIntention<IReturnOperation>(toBuild, () =>
            {
                maker.Build(Result.GetOrThrow().ConvertElementOrThrow(context));
            });
        }
    }

    internal abstract class TrailingOperion<T> 
    {
        public abstract IConvertableFrontendCodeElement<ICodeElement>[] Operands { get; }
        public abstract T1 Convert<T1,TBacking>(IOpenBoxesContext<T1,TBacking> context) where TBacking:IBacking;
        public abstract IVerifiableType Returns();
    }

    internal class TrailingOperation {
        public delegate IIsPossibly<T> Make<out T>(IIsPossibly<IFrontendCodeElement> codeElement);
    }

    internal class TrailingOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<IPopulateScope<TFrontendCodeElement>>
        where TFrontendCodeElement : class, IConvertableFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        public TrailingOperationMaker(string symbol, TrailingOperation.Make<TFrontendCodeElement> make)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public string Symbol { get; }
        private TrailingOperation.Make<TFrontendCodeElement> Make { get; }

        public ITokenMatching<IPopulateScope<TFrontendCodeElement>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            

            var matching = tokenMatching
                .Has(new TrailingOperationMatcher(Symbol), out (IEnumerable<IToken> perface, AtomicToken _) res);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                
                return TokenMatching<IPopulateScope<TFrontendCodeElement>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TrailingPopulateScope(left,Make));
            }
            return TokenMatching<IPopulateScope<TFrontendCodeElement>>.MakeNotMatch(
                    matching.Context);
        }
        
        public static IPopulateScope<TFrontendCodeElement> PopulateScope(IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>> left,
                TrailingOperation.Make<TFrontendCodeElement> make)
        {
            return new TrailingPopulateScope(left, make);
        }
        public static IPopulateBoxes<TFrontendCodeElement> PopulateBoxes(IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>> left,
                TrailingOperation.Make<TFrontendCodeElement> make)
        {
            return new TrailingResolveReferance(left,
                make);
        }


        private class TrailingPopulateScope : IPopulateScope<TFrontendCodeElement>
        {
            private readonly IPopulateScope<IFrontendCodeElement> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;

            public TrailingPopulateScope(IPopulateScope<IFrontendCodeElement> left, TrailingOperation.Make<TFrontendCodeElement> make)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IResolvelizeScope<TFrontendCodeElement> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {
                return new TrailingFinalizeScope(left.Run(scope,context), make);
            }
        }


        private class TrailingFinalizeScope : IResolvelizeScope<TFrontendCodeElement>
        {
            public readonly IResolvelizeScope<IFrontendCodeElement> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;

            public TrailingFinalizeScope(IResolvelizeScope<IFrontendCodeElement> resolveReferance1, TrailingOperation.Make<TFrontendCodeElement> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IPopulateBoxes<TFrontendCodeElement> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new TrailingResolveReferance(left.Run(parent,context), make);
            }
        }

        private class TrailingResolveReferance: IPopulateBoxes<TFrontendCodeElement>
        {
            public readonly IPopulateBoxes<IFrontendCodeElement> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;

            public TrailingResolveReferance(IPopulateBoxes<IFrontendCodeElement> resolveReferance1, TrailingOperation.Make<TFrontendCodeElement> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IIsPossibly<TFrontendCodeElement> Run(IResolvableScope scope ,IResolveReferenceContext context)
            {
                var res = make(left.Run(scope,context));
                return res;
            }
        }
    }


    internal class ReturnOperationMaker : TrailingOperationMaker<WeakReturnOperation, IReturnOperation>
    {
        public ReturnOperationMaker() : base(SymbolsRegistry.StaticReturnSymbol, x=>Possibly.Is(new WeakReturnOperation(x)))
        {
        }
    }
}
