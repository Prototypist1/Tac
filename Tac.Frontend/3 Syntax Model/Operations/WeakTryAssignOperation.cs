using System.Collections.Generic;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.Operations
{
    internal class WeakTryAssignOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ITryAssignOperation>
    {

        public WeakTryAssignOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }

        public override IIsPossibly<IFrontendType> Returns()
        {
            return Left.IfIs(x => x.Returns());
        }

        public override IBuildIntention<ITryAssignOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = TryAssignOperation.Create();
            return new BuildIntention<ITryAssignOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertElementOrThrow(context), Right.GetOrThrow().ConvertElementOrThrow(context));
            });
        }
    }

    internal class TryAssignOperationMaker : IMaker<IPopulateScope<WeakTryAssignOperation>>
    {
        public TryAssignOperationMaker()
        {
        }

        public ITokenMatching<IPopulateScope<WeakTryAssignOperation>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var matching = tokenMatching
            .Has(new BinaryOperationMatcher(SymbolsRegistry.TryAssignSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) res);

            if (matching
                 is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                var right = matching.Context.AcceptImplicit(left.GetReturnType()).ParseParenthesisOrElement(res.rhs);

                return TokenMatching<IPopulateScope<WeakTryAssignOperation>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    BinaryOperationMaker<WeakTryAssignOperation, ITryAssignOperation>.PopulateScope(left, right, (l, r) =>
                        Possibly.Is(
                            new WeakTryAssignOperation(l, r))));
            }

            return TokenMatching<IPopulateScope<WeakTryAssignOperation>>.MakeNotMatch(
                    matching.Context);
        }


    }


}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement>> StaticTryAssignMaker = AddOperationMatcher(() => new TryAssignOperationMaker());
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement>> TryAssignMaker = StaticTryAssignMaker;
    }
}


namespace Tac.Semantic_Model.CodeStuff
{
    // maybe some registaration in this page
    // like at the bottum we tell something this is here
    // like wanderer modules 
    public partial class SymbolsRegistry
    {
        public static readonly string TryAssignSymbol = StaticSymbolsRegistry.AddOrThrow("?=:");
    }
}
