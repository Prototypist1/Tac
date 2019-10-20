using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New.CrzayNamespace;
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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement,ITypeProblemNode>> StaticAssertAssignMaker = AddOperationMatcher(() => new AssertAssignOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> AssertAssignMaker = StaticAssertAssignMaker;
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

    internal class AssertAssignOperationMaker : IMaker<IPopulateScope<WeakAssignOperation, Tpn.IValue>>
    {

        public AssertAssignOperationMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakAssignOperation, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticAssertAssignSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElement(match.rhs);

                return TokenMatching<IPopulateScope<WeakAssignOperation, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new WeakAssignOperationPopulateScope(left, right));
            }

            return TokenMatching<IPopulateScope<WeakAssignOperation, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakAssignOperationPopulateScope : IPopulateScope<WeakAssignOperation, Tpn.IValue>
        {
            private readonly IPopulateScope<IFrontendCodeElement,ITypeProblemNode> left;
            private readonly IPopulateScope<IFrontendCodeElement,ITypeProblemNode> right;

            public WeakAssignOperationPopulateScope(IPopulateScope<IFrontendCodeElement,ITypeProblemNode> left,
                IPopulateScope<IFrontendCodeElement,ITypeProblemNode> right)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));;
            }

            public IResolvelizeScope<WeakAssignOperation,Tpn.IValue> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {

                var nextLeft = left.Run(scope, context);
                var nextRight = right.Run(scope, context);

                if (left is Tpn.ICanAssignFromMe from && right is Tpn.ICanBeAssignedTo to) {
                    from.AssignTo(to);
                }
                else {
                    throw new Exception("I need real error handling");
                }

                return new WeakAssignOperationFinalizeScope(
                    nextLeft,
                    nextRight);
            }
        }

        private class WeakAssignOperationFinalizeScope : IResolvelizeScope<WeakAssignOperation,Tpn.IValue>
        {
            public readonly IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> left;
            public readonly IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> right;

            public WeakAssignOperationFinalizeScope(
                IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> resolveReferance1,
                IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode> resolveReferance2)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            }

            // this will not handle bad code well,
            //need to take care of it when I do a pass to make error handling graceful
            public Tpn.IValue SetUpSideNode => left.SetUpSideNode as Tpn.IValue;

            public IPopulateBoxes<WeakAssignOperation> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new WeakAssignOperationResolveReferance(left.Run(parent, context), right.Run(parent, context));
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
