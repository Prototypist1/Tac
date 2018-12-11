using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
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
    
    internal class WeakReturnOperation : TrailingOperation, IFrontendCodeElement
    {
        public WeakReturnOperation(IIsPossibly<IFrontendCodeElement> result)
        {
            Result = result;
        }
        
        public IIsPossibly<IFrontendCodeElement> Result { get; }
        
        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is(new EmptyType());
        }
    }

    internal abstract class TrailingOperion<T> 
    {
        public abstract IFrontendCodeElement[] Operands { get; }
        public abstract T1 Convert<T1>(IOpenBoxesContext<T1> context);
        public abstract IVarifiableType Returns();
    }

    internal class TrailingOperation {
        public delegate IIsPossibly<T> Make<out T>(IIsPossibly<IFrontendCodeElement> codeElement);
    }

    internal class TrailingOperationMaker<T> : IMaker<IPopulateScope<T>>
        where T : class, IFrontendCodeElement
    {
        public TrailingOperationMaker(ISymbols name, TrailingOperation.Make<T> make)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public ISymbols Name { get; }
        private TrailingOperation.Make<T> Make { get; }

        public ITokenMatching<IPopulateScope<T>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            

            var matching = tokenMatching
                .Has(new TrailingOperationMatcher(Name.Symbols), out (IEnumerable<IToken> perface, AtomicToken _) res);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                
                return TokenMatching<IPopulateScope<T>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TrailingPopulateScope<T>(left,Make));
            }
            return TokenMatching<IPopulateScope<T>>.MakeNotMatch(
                    matching.Context);
        }
        
    }

    internal class TrailingPopulateScope<T> : IPopulateScope<T>
        where T : class, IFrontendCodeElement
    {
        private readonly IPopulateScope<IFrontendCodeElement> left;
        private readonly TrailingOperation.Make<T> make;
        private readonly DelegateBox<IIsPossibly<IFrontendType>> box = new DelegateBox<IIsPossibly<IFrontendType>>();

        public TrailingPopulateScope(IPopulateScope<IFrontendCodeElement> left, TrailingOperation.Make<T> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IIsPossibly<IFrontendType>> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<T> Run(IPopulateScopeContext context)
        {
            return new TrailingResolveReferance<T>(left.Run(context),  make, box);
        }
    }



    internal class TrailingResolveReferance<T> : IPopulateBoxes<T>
        where T : class, IFrontendCodeElement
    {
        public readonly IPopulateBoxes<IFrontendCodeElement> left;
        private readonly TrailingOperation.Make<T> make;
        private readonly DelegateBox<IIsPossibly<IFrontendType>> box;

        public TrailingResolveReferance(IPopulateBoxes<IFrontendCodeElement> resolveReferance1, TrailingOperation.Make<T> make, DelegateBox<IIsPossibly<IFrontendType>> box)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public IIsPossibly<T> Run(IResolveReferenceContext context)
        {
            var res = make(left.Run(context));
            box.Set(()=> {
                    if (res.IsDefinately(out var yes, out var no))
                    {
                        return yes.Value.Returns();
                    }
                    else {
                        return Possibly.IsNot<IFrontendType>(no);
                    }
                });
            return res;
        }
    }


    internal class ReturnOperationMaker : TrailingOperationMaker<WeakReturnOperation>
    {
        public ReturnOperationMaker() : base(new RetunrSymbols(), x=>Possibly.Is(new WeakReturnOperation(x)))
        {
        }
    }
}
