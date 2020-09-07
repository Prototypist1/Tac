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
using Prototypist.Toolbox;
using Tac.SemanticModel;
using System;
using Prototypist.Toolbox.Object;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.SemanticModel.Operations
{

    // the syntax for this...
    // really needs to be
    // 5 is number n { ... } 
    // n only exists in the { ... }
    // otherwise you could write 
    // 5 is Cat cat { ... } ; cat.age > some-method
    // and that will error out

    // really I might not need the name need the name
    // 5 is Cat { }
    //

    internal class WeakTryAssignOperation : IConvertableFrontendCodeElement<ITryAssignOperation> 
    {

        public WeakTryAssignOperation(
            IOrType<IBox<IFrontendCodeElement>,IError> left, 
            IOrType<IBox<IFrontendCodeElement>,IError> right, 
            IOrType<IBox<IFrontendCodeElement>, IError> body,
            IOrType<IBox<WeakScope>, IError> scope)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IOrType<IBox<IFrontendCodeElement>, IError> Left { get; }
        public IOrType<IBox<IFrontendCodeElement>, IError> Right { get; }
        public IOrType<IBox<IFrontendCodeElement>, IError> Body { get; }
        public IOrType<IBox<WeakScope>, IError> Scope { get; }

        public IBuildIntention<ITryAssignOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = TryAssignOperation.Create();
            return new BuildIntention<ITryAssignOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context),
                    Body.Is1OrThrow().GetValue().ConvertElementOrThrow(context),
                    Scope.Is1OrThrow().GetValue().Convert(context));
            });
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var item in Left.SwitchReturns(x => x.GetValue().Validate(), x => new List<IError>() { x }))
            {
                yield return item;
            }
            foreach (var item in Right.SwitchReturns(x => x.GetValue().Validate(), x => new List<IError>() { x }))
            {
                yield return item;
            }
            foreach (var item in Body.SwitchReturns(x => x.GetValue().Validate(), x => new List<IError>() { x }))
            {
                yield return item;
            }

            // it must be possible for left to be right
            // this come down to them being the same C# type
            // mostly they are both HasMembersType but they could both be methods
            // they could as be the same primitive but that would not be very interesting
            if (Left.ReturnsTypeOrErrors().Is1(out var leftType) && Right.ReturnsTypeOrErrors().Is1(out var rightType)){
                if (leftType.GetType() != rightType.GetType()) {
                    yield return Error.AssignmentMustBePossible("it must be possible for the left to be the right");
                }
            }

            if (!Body.Possibly1().AsEnumerable().Select(x=>x.GetValue()).OfType<WeakBlockDefinition>().Any())
            {
                yield return Error.Other($"body must be a block");
            }
        }
    }

    internal class TryAssignOperationMaker : IMaker<ISetUp<IBox< WeakTryAssignOperation>, Tpn.IValue>>
    {
        public TryAssignOperationMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakTryAssignOperation>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var index = tokenMatching.EndIndex;

            if (tokenMatching.AllTokens.Count - 3 > index &&
                    tokenMatching.AllTokens[index+1].Is1(out var token) && token.SafeIs(out AtomicToken op) &&
                    op.Item == SymbolsRegistry.TryAssignSymbol &&
                    tokenMatching.AllTokens[index].Is2(out var lhs) &&
                    tokenMatching.AllTokens[index + 2].Is2(out var rhs) &&
                    tokenMatching.AllTokens[index + 3].Is2(out var rrhs))
            {
                var left = OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(lhs.SafeCastTo(out ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode> _));
                var right = OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(rhs.SafeCastTo(out ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode> _));
                var block = OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(rrhs.SafeCastTo(out ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode> _));
                

                var res = new TryAssignOperationPopulateScope(left, right, block);

                return TokenMatching<ISetUp<IBox<WeakTryAssignOperation>, Tpn.IValue>>.MakeMatch(
                    tokenMatching,
                    res,
                    tokenMatching.EndIndex + 4
                );
            }

            return TokenMatching<ISetUp<IBox<WeakTryAssignOperation>, Tpn.IValue>>.MakeNotMatch(
                    tokenMatching.Context);
        }


    }


    internal class TryAssignOperationPopulateScope : ISetUp<IBox<WeakTryAssignOperation>, Tpn.IValue>
    {
        private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left;
        private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right;
        private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> block;

        public TryAssignOperationPopulateScope(
            IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left,
            IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right,
            IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> block)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.block = block ?? throw new ArgumentNullException(nameof(block));
        }

        public ISetUpResult<IBox<WeakTryAssignOperation>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            var box = new Box<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[]>();

            // we create a new scope because the member we are assigning in to only exists in that scope
            // 5 is Cat c { ... }
            // is really 
            // { 5 is Cat c { ... }}

            var myScope = context.TypeProblem.CreateScope(scope, new WeakBlockDefinitionConverter(box));

            var nextLeft = left.TransformInner(x => x.Run(myScope, context.CreateChildContext(this)));
            var nextRight = right.TransformInner(x => x.Run(myScope, context.CreateChildContext(this)));
            var nextblock = block.TransformInner(x => x.Run(myScope, context.CreateChildContext(this)));

            if (nextLeft.Is1(out var nextLeft1) && nextLeft1.SetUpSideNode.Is1(out var node1) && nextRight.Is1(out var nextRight1) && nextRight1.SetUpSideNode.Is1(out var node2))
            {
                if (!(node1 is Tpn.ICanAssignFromMe canAssignFromMe))
                {
                    // todo I need real error handling
                    // probably I need somewhere to stuff additional errors
                    throw new NotImplementedException($"can not assign from {nextLeft1.SetUpSideNode}");
                }

                if (!(node2 is Tpn.ICanBeAssignedTo canBeAssignedTo))
                {
                    // todo I need real error handling
                    throw new NotImplementedException($"can not assign to {nextRight1.SetUpSideNode}");
                }
            }
            else
            {
                // left or right are errors 
                throw new NotImplementedException();
            }

            var res = new TryAssignOperationResolveReferance(
                nextLeft.TransformInner(x => x.Resolve),
                nextRight.TransformInner(x => x.Resolve),
                nextblock.TransformInner(x => x.Resolve),
                myScope);

            box.Fill(new[] { OrType.Make<IResolve<IBox<IFrontendCodeElement>>, IError>(res) });

            return new SetUpResult<IBox<WeakTryAssignOperation>, Tpn.IValue>(res,
                nextLeft.TransformAndFlatten(x => x.SetUpSideNode).OrCastToOr<Tpn.ITypeProblemNode, Tpn.IValue>(Error.Other("")));
        }
    }

    internal class TryAssignOperationResolveReferance : IResolve<IBox<WeakTryAssignOperation>>
    {
        public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> left;
        public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> right;
        private readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> block;
        private readonly Tpn.IHavePrivateMembers haveMembers;

        public TryAssignOperationResolveReferance(
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> value,
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> member,
           IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> block,
           Tpn.IHavePrivateMembers haveMembers)
        {
            left = value ?? throw new ArgumentNullException(nameof(value));
            right = member ?? throw new ArgumentNullException(nameof(member));
            this.block = block ?? throw new ArgumentNullException(nameof(block));
            this.haveMembers = haveMembers ?? throw new ArgumentNullException(nameof(haveMembers));
        }


        public IBox<WeakTryAssignOperation> Run(Tpn.TypeSolution context)
        {
            var res = new Box<WeakTryAssignOperation>(new WeakTryAssignOperation(
                left.TransformInner(x => x.Run(context)),
                right.TransformInner(x => x.Run(context)),
                block.TransformInner(x=>x.Run(context)),
                OrType.Make<IBox<WeakScope>, IError>(new Box<WeakScope>(new WeakScope(context.GetPrivateMembers(haveMembers).Select(x => context.GetMember(x)).ToList())))));
            return res;
        }
    }


}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticTryAssignMaker = AddOperationMatcher(() => new TryAssignOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> TryAssignMaker = StaticTryAssignMaker;
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
        public static readonly string TryAssignSymbol = StaticSymbolsRegistry.AddOrThrow("is");
    }
}
