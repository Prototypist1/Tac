using System;
using System.Collections.Generic;
using System.Text;
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
    internal class AssignSymbols : ISymbols
    {
        public string Symbols => "=:";
    }

    internal class WeakAssignOperation : BinaryOperation<IConvertableFrontendCodeElement<ICodeElement>, IConvertableFrontendCodeElement<ICodeElement>, IAssignOperation>
    {
        
        public WeakAssignOperation(IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>> left, IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IConvertableFrontendType<IVerifiableType>> Returns()
        {
            return Left.IfIs(x=>x.Returns());
        }

        public override IBuildIntention<IAssignOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = AssignOperation.Create();
            return new BuildIntention<IAssignOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().Convert(context), Right.GetOrThrow().Convert(context));
            });
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
                    BinaryOperationMaker<WeakAssignOperation, IAssignOperation>.PopulateScope(left, right, (l,r) => 
                        Possibly.Is(
                            new WeakAssignOperation(l,r))));
            }

            return TokenMatching<IPopulateScope<WeakAssignOperation>>.MakeNotMatch(
                    matching.Context);
        }
        

    }

}
