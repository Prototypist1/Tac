using System.Collections.Generic;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;

namespace Tac.SemanticModel.Operations
{
    internal class WeakTryAssignOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ITryAssignOperation>
    {

        public WeakTryAssignOperation(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right) : base(left, right)
        {
        }

        public override IBuildIntention<ITryAssignOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = TryAssignOperation.Create();
            return new BuildIntention<ITryAssignOperation>(toBuild, () =>
            {
                maker.Build(Left.GetValue().ConvertElementOrThrow(context), Right.GetValue().ConvertElementOrThrow(context));
            });
        }
    }

    internal class TryAssignOperationMaker : IMaker<ISetUp<WeakTryAssignOperation, Tpn.IValue>>
    {
        public TryAssignOperationMaker()
        {
        }

        public ITokenMatching<ISetUp<WeakTryAssignOperation, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var matching = tokenMatching
            .HasStruct(new BinaryOperationMatcher(SymbolsRegistry.TryAssignSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) res);

            if (matching
                 is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                var right = matching.Context.ParseParenthesisOrElement(res.rhs);

                return TokenMatching<ISetUp<WeakTryAssignOperation, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    BinaryOperationMaker<WeakTryAssignOperation, ITryAssignOperation>.PopulateScope(left, right, (l, r) =>
                       new Box<WeakTryAssignOperation>(
                            new WeakTryAssignOperation(l, r)),
                    (s,c,l,r)=> c.TypeProblem.CreateValue(s,new NameKey("bool"),new PlaceholderValueConverter())));
            }

            return TokenMatching<ISetUp<WeakTryAssignOperation, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


    }


}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticTryAssignMaker = AddOperationMatcher(() => new TryAssignOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> TryAssignMaker = StaticTryAssignMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.CodeStuff
{
    // maybe some registaration in this page
    // like at the bottum we tell something this is here
    // like wanderer modules 
    public partial class SymbolsRegistry
    {
        public static readonly string TryAssignSymbol = StaticSymbolsRegistry.AddOrThrow("?=:");
    }
}
