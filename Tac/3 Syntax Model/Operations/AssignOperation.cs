using System;
using System.Collections.Generic;
using System.Text;
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
    internal class AssignSymbols : ISymbols
    {
        public string Symbols => "=:";
    }

    internal class WeakAssignOperation : BinaryOperation<ICodeElement, ICodeElement>, IAssignOperation
    {
        
        public WeakAssignOperation(IIsPossibly<ICodeElement> left, IIsPossibly<ICodeElement> right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.AssignOperation(this);
        }

        public override IVarifiableType Returns()
        {
            return Left.Returns();
        }
    }

    internal class AssignOperationMaker : IMaker<IPopulateScope<WeakAssignOperation>>
    {
        public AssignOperationMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakAssignOperation>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var matching = tokenMatching
            .Has(new BinaryOperationMatcher(new AssignSymbols().Symbols), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) res);

            if (matching
                 is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                var right = matching.Context.AcceptImplicit(left.GetReturnType()).ParseParenthesisOrElement(res.rhs);

                return TokenMatching<IPopulateScope<WeakAssignOperation>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new BinaryPopulateScope<WeakAssignOperation>(left, right, (l,r) => 
                        Possibly.Is(
                            new WeakAssignOperation(l,r))));
            }

            return TokenMatching<IPopulateScope<WeakAssignOperation>>.MakeNotMatch(
                    matching.Context);
        }
        

    }

}
