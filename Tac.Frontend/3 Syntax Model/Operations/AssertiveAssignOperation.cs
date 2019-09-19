using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
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
        public static readonly string StaticAssertAssignSymbol = StaticSymbolsRegistry.AddOrThrow("=:");
        public readonly string AssertAssignSymbol = StaticAssertAssignSymbol;
    }
}


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement>> StaticAssertAssignMaker = AddOperationMatcher(() => new AssertAssignOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement>> AssertAssignMaker = StaticAssertAssignMaker;
#pragma warning restore IDE0052 // Remove unread private members
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

        public override IBuildIntention<IAssignOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = AssignOperation.Create();
            return new BuildIntention<IAssignOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertElementOrThrow(context), Right.GetOrThrow().ConvertElementOrThrow(context));
            });
        }
    }

    internal class AssertAssignOperationMaker : IMaker<IPopulateScope<WeakAssignOperation>>
    {

        public AssertAssignOperationMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakAssignOperation>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticAssertAssignSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElement(match.rhs);

                return TokenMatching<IPopulateScope<WeakAssignOperation>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new WeakAssignOperationPopulateScope(left, right));
            }

            return TokenMatching<IPopulateScope<WeakAssignOperation>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakAssignOperationPopulateScope : IPopulateScope<WeakAssignOperation>
        {
            private readonly IPopulateScope<IFrontendCodeElement> left;
            private readonly IPopulateScope<IFrontendCodeElement> right;

            public WeakAssignOperationPopulateScope(IPopulateScope<IFrontendCodeElement> left,
                IPopulateScope<IFrontendCodeElement> right)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));;
            }

            public IResolvelizeScope<WeakAssignOperation> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {
                left.GetReturnedType().AcceptsType(right.GetReturnedType());
                
                return new WeakAssignOperationFinalizeScope(
                    left.Run(scope, context),
                    right.Run(scope, context));
            }
        }

        private class WeakAssignOperationFinalizeScope : IResolvelizeScope<WeakAssignOperation>
        {
            public readonly IResolvelizeScope<IFrontendCodeElement> left;
            public readonly IResolvelizeScope<IFrontendCodeElement> right;

            public WeakAssignOperationFinalizeScope(
                IResolvelizeScope<IFrontendCodeElement> resolveReferance1,
                IResolvelizeScope<IFrontendCodeElement> resolveReferance2)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            }


            public IPopulateBoxes<WeakAssignOperation> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new WeakAssignOperationResolveReferance(left.Run(parent, context), right.Run(parent, context), box);
            }
        }

        private class WeakAssignOperationResolveReferance : IPopulateBoxes<WeakAssignOperation>
        {
            public readonly IPopulateBoxes<IFrontendCodeElement> left;
            public readonly IPopulateBoxes<IFrontendCodeElement> right;

            public WeakAssignOperationResolveReferance(
                IPopulateBoxes<IFrontendCodeElement> resolveReferance1,
                IPopulateBoxes<IFrontendCodeElement> resolveReferance2)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }


            public IIsPossibly<WeakAssignOperation> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                var leftRes = left.Run(scope, context);
                var res = Possibly.Is(new WeakAssignOperation(
                    leftRes,
                    right.Run(scope, context)));
                return res;
            }
        }
    }
}
