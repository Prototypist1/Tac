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

namespace Tac.Semantic_Model.CodeStuff
{
    // maybe some registaration in this page
    // like at the bottum we tell something this is here
    // like wanderer modules 
    public partial class SymbolsRegistry
    {
        public static readonly string StaticAssignSymbol = StaticSymbolsRegistry.AddOrThrow("=:");
        public readonly string AssignSymbol = StaticAssignSymbol;
    }
}

namespace Tac.Semantic_Model.Operations
{


    internal class WeakAssignOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IAssignOperation>
    {
        public WeakAssignOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }

        public override IIsPossibly<IFrontendType> Returns()
        {
            return Left.IfIs(x => x.Returns());
        }

        public override IBuildIntention<IAssignOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = AssignOperation.Create();
            return new BuildIntention<IAssignOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertElementOrThrow(context), Right.GetOrThrow().ConvertElementOrThrow(context));
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
            .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticAssignSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) res);

            if (matching
                 is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                var right = matching.Context.AcceptImplicit(left.GetReturnType()).ParseParenthesisOrElement(res.rhs);

                return TokenMatching<IPopulateScope<WeakAssignOperation>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    BinaryOperationMaker<WeakAssignOperation, IAssignOperation>.PopulateScope(left, right, (l, r) =>
                        Possibly.Is(
                            new WeakAssignOperation(l, r))));
            }

            return TokenMatching<IPopulateScope<WeakAssignOperation>>.MakeNotMatch(
                    matching.Context);
        }


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

namespace Tac.Semantic_Model.Operations
{

    internal class WeakTryAssignOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IAssignOperation>
    {

        public WeakTryAssignOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }

        public override IIsPossibly<IFrontendType> Returns()
        {
            return Left.IfIs(x => x.Returns());
        }

        public override IBuildIntention<IAssignOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = AssignOperation.Create();
            return new BuildIntention<IAssignOperation>(toBuild, () =>
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
                    BinaryOperationMaker<WeakTryAssignOperation, IAssignOperation>.PopulateScope(left, right, (l, r) =>
                        Possibly.Is(
                            new WeakTryAssignOperation(l, r))));
            }

            return TokenMatching<IPopulateScope<WeakTryAssignOperation>>.MakeNotMatch(
                    matching.Context);
        }


    }

}
