using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    internal class PathSymbols : ISymbols
    {
        public string Symbols => ".";
    }

    internal class WeakPathOperation : BinaryOperation<ICodeElement, ICodeElement>, IPathOperation
    {
        public WeakPathOperation(IIsPossibly<ICodeElement> left, IIsPossibly<ICodeElement> right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.PathOperation(this);
        }
        
        public override IVarifiableType Returns()
        {
            // should this check to see if the left contains the member defined on the rhs?
            return Right.Cast<WeakMemberReference>();
        }
    }
    
    internal class PathOperationMaker : IMaker<IPopulateScope<WeakPathOperation>>
    {
        public ITokenMatching<IPopulateScope<WeakPathOperation>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(new PathSymbols().Symbols), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) res);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                var right = matching.Context.ExpectPathPart(left.GetReturnType()).ParseParenthesisOrElement(res.rhs);

                return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new BinaryPopulateScope<WeakPathOperation>(left, right, (l,r)=> Possibly.Is(new WeakPathOperation(l,r))));
            }

            return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeNotMatch(
                    matching.Context);
        }
    }
}
