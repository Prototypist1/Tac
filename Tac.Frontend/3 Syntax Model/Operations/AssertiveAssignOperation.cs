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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticAssertAssignMaker = AddOperationMatcher(() => new AssertAssignOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> AssertAssignMaker = StaticAssertAssignMaker;
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

    internal class AssertAssignOperationMaker : IMaker<ISetUp<WeakAssignOperation, LocalTpn.IValue>>
    {

        public AssertAssignOperationMaker()
        {
        }
        
        public ITokenMatching<ISetUp<WeakAssignOperation, LocalTpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticAssertAssignSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElement(match.rhs);

                return TokenMatching<ISetUp<WeakAssignOperation, LocalTpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new WeakAssignOperationPopulateScope(left, right));
            }

            return TokenMatching<ISetUp<WeakAssignOperation, LocalTpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakAssignOperationPopulateScope : ISetUp<WeakAssignOperation, LocalTpn.IValue>
        {
            private readonly ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode> left;
            private readonly ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode> right;

            public WeakAssignOperationPopulateScope(ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode> left,
                ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode> right)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));;
            }

            public ISetUpResult<WeakAssignOperation, LocalTpn.IValue> Run(LocalTpn.IScope scope, ISetUpContext context)
            {

                var nextLeft = left.Run(scope, context);
                var nextRight = right.Run(scope, context);

                if (left is LocalTpn.ICanAssignFromMe from && right is LocalTpn.ICanBeAssignedTo to) {
                    from.AssignTo(to);
                }
                else {
                    throw new Exception("I need real error handling");
                }

                return new SetUpResult<WeakAssignOperation, LocalTpn.IValue>(new WeakAssignOperationResolveReferance(
                    nextLeft.Resolve,
                    nextRight.Resolve),
                    nextLeft.SetUpSideNode as LocalTpn.IValue);
            }
        }

        private class WeakAssignOperationResolveReferance : IResolve<WeakAssignOperation>
        {
            public readonly IResolve<IFrontendCodeElement> left;
            public readonly IResolve<IFrontendCodeElement> right;

            public WeakAssignOperationResolveReferance(
                IResolve<IFrontendCodeElement> resolveReferance1,
                IResolve<IFrontendCodeElement> resolveReferance2)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            }


            public IIsPossibly<WeakAssignOperation> Run(IResolvableScope scope, IResolveContext context)
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
