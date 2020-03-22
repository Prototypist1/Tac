using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Text;
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
using Prototypist.Toolbox;

namespace Tac.SemanticModel.CodeStuff
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticAssertAssignMaker = AddOperationMatcher(() => new AssertAssignOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> AssertAssignMaker = StaticAssertAssignMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}




namespace Tac.SemanticModel.Operations
{



    internal class WeakAssignOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IAssignOperation>
    {
        public WeakAssignOperation(OrType<IBox<IFrontendCodeElement>, IError> left, OrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }

        public override IBuildIntention<IAssignOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = AssignOperation.Create();
            return new BuildIntention<IAssignOperation>(toBuild, () =>
            {
                maker.Build(Left.Convert(x=>x.GetValue().ConvertElementOrThrow(context)), Right.Convert(x => x.GetValue().ConvertElementOrThrow(context)));
            });
        }
    }

    internal class AssertAssignOperationMaker : IMaker<ISetUp<WeakAssignOperation, Tpn.IValue>>
    {

        public AssertAssignOperationMaker()
        {
        }
        
        public ITokenMatching<ISetUp<WeakAssignOperation, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .HasStruct(new BinaryOperationMatcher(SymbolsRegistry.StaticAssertAssignSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(match.perface);
                var right = matching.Context.ParseParenthesisOrElement(match.rhs);

                return TokenMatching<ISetUp<WeakAssignOperation, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new WeakAssignOperationPopulateScope(left, right));
            }

            return TokenMatching<ISetUp<WeakAssignOperation, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakAssignOperationPopulateScope : ISetUp<WeakAssignOperation, Tpn.IValue>
        {
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> left;
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> right;

            public WeakAssignOperationPopulateScope(ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> left,
                ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> right)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));;
            }

            public ISetUpResult<WeakAssignOperation, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {

                var nextLeft = left.Run(scope, context);
                var nextRight = right.Run(scope, context);

                if (nextLeft.SetUpSideNode is Tpn.ICanAssignFromMe from && nextRight.SetUpSideNode is Tpn.ICanBeAssignedTo to) {
                    from.AssignTo(to);
                }
                else {
                    throw new Exception("I need real error handling");
                }

                return new SetUpResult<WeakAssignOperation, Tpn.IValue>(new WeakAssignOperationResolveReferance(
                    nextLeft.Resolve,
                    nextRight.Resolve),
                    nextLeft.SetUpSideNode.CastTo<Tpn.IValue>());
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


            public IBox<WeakAssignOperation> Run(Tpn.ITypeSolution context)
            {
                var leftRes = left.Run( context);
                var res = new Box<WeakAssignOperation>(new WeakAssignOperation(
                    leftRes,
                    right.Run( context)));
                return res;
            }
        }
    }
}
