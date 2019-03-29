using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    internal class RetunrSymbols : ISymbols
    {
        public string Symbols => "return";
    }
    
    internal class WeakReturnOperation : TrailingOperation, IFrontendCodeElement<IReturnOperation>
    {
        public WeakReturnOperation(IIsPossibly<IFrontendCodeElement<ICodeElement>> result)
        {
            Result = result;
        }
        
        public IIsPossibly<IFrontendCodeElement<ICodeElement>> Result { get; }
        
        public IIsPossibly<IFrontendType<IVerifiableType>> Returns()
        {
            return Possibly.Is<IFrontendType<IVerifiableType>>(PrimitiveTypes.CreateEmptyType());
        }


        public IBuildIntention<IReturnOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = ReturnOperation.Create();
            return new BuildIntention<IReturnOperation>(toBuild, () =>
            {
                maker.Build(Result.GetOrThrow().Convert(context));
            });
        }
    }

    internal abstract class TrailingOperion<T> 
    {
        public abstract IFrontendCodeElement<ICodeElement>[] Operands { get; }
        public abstract T1 Convert<T1,TBacking>(IOpenBoxesContext<T1,TBacking> context) where TBacking:IBacking;
        public abstract IVerifiableType Returns();
    }

    internal class TrailingOperation {
        public delegate IIsPossibly<T> Make<out T>(IIsPossibly<IFrontendCodeElement<ICodeElement>> codeElement);
    }

    internal class TrailingOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<IPopulateScope<TFrontendCodeElement>>
        where TFrontendCodeElement : class, IFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        public TrailingOperationMaker(ISymbols name, TrailingOperation.Make<TFrontendCodeElement> make)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public ISymbols Name { get; }
        private TrailingOperation.Make<TFrontendCodeElement> Make { get; }

        public ITokenMatching<IPopulateScope<TFrontendCodeElement>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            

            var matching = tokenMatching
                .Has(new TrailingOperationMatcher(Name.Symbols), out (IEnumerable<IToken> perface, AtomicToken _) res);
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
        
        public static IPopulateScope<TFrontendCodeElement> PopulateScope(IPopulateScope<IFrontendCodeElement<ICodeElement>> left,
                TrailingOperation.Make<TFrontendCodeElement> make)
        {
            return new TrailingPopulateScope(left, make);
        }
        public static IPopulateBoxes<TFrontendCodeElement> PopulateBoxes(IPopulateBoxes<IFrontendCodeElement<ICodeElement>> left,
                TrailingOperation.Make<TFrontendCodeElement> make,
                DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>> box)
        {
            return new TrailingResolveReferance(left,
                make,
                box);
        }


        private class TrailingPopulateScope : IPopulateScope<TFrontendCodeElement>
        {
            private readonly IPopulateScope<IFrontendCodeElement<ICodeElement>> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;
            private readonly DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>> box = new DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>>();

            public TrailingPopulateScope(IPopulateScope<IFrontendCodeElement<ICodeElement>> left, TrailingOperation.Make<TFrontendCodeElement> make)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetReturnType()
            {
                return box;
            }

            public IPopulateBoxes<TFrontendCodeElement> Run(IPopulateScopeContext context)
            {
                return new TrailingResolveReferance(left.Run(context), make, box);
            }
        }



        private class TrailingResolveReferance: IPopulateBoxes<TFrontendCodeElement>
        {
            public readonly IPopulateBoxes<IFrontendCodeElement<ICodeElement>> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;
            private readonly DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>> box;

            public TrailingResolveReferance(IPopulateBoxes<IFrontendCodeElement<ICodeElement>> resolveReferance1, TrailingOperation.Make<TFrontendCodeElement> make, DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>> box)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }

            public IIsPossibly<TFrontendCodeElement> Run(IResolveReferenceContext context)
            {
                var res = make(left.Run(context));
                box.Set(() => {
                    if (res.IsDefinately(out var yes, out var no))
                    {
                        return yes.Value.Returns();
                    }
                    else
                    {
                        return Possibly.IsNot<IFrontendType<IVerifiableType>>(no);
                    }
                });
                return res;
            }
        }


    }


    internal class ReturnOperationMaker : TrailingOperationMaker<WeakReturnOperation, IReturnOperation>
    {
        public ReturnOperationMaker() : base(new RetunrSymbols(), x=>Possibly.Is(new WeakReturnOperation(x)))
        {
        }
    }
}
