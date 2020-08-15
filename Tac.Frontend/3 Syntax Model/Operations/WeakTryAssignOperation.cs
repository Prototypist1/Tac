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

        public WeakTryAssignOperation(IOrType<IBox<IFrontendCodeElement>,IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right, IOrType<IBox<IFrontendCodeElement>, IError> body)
        {
            Left = left;
            Right = right;
            Body = body;
        }

        public IOrType<IBox<IFrontendCodeElement>, IError> Left { get; }
        public IOrType<IBox<IFrontendCodeElement>, IError> Right { get; }
        public IOrType<IBox<IFrontendCodeElement>, IError> Body { get; }

        public IBuildIntention<ITryAssignOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = TryAssignOperation.Create();
            return new BuildIntention<ITryAssignOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
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
            if (!Right.Possibly1().AsEnumerable().OfType<WeakBlockDefinition>().Any())
            {
                yield return Error.Other($"right hand side must be a block");
            }
            foreach (var error in Left.TypeCheck(new Tac.SyntaxModel.Elements.AtomicTypes.BlockType()))
            {
                yield return error;
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

            if (tokenMatching.AllTokens.Count - 2 > index & index != 0 &&
                    tokenMatching.AllTokens[index] is AtomicToken op &&
                    op.Item == SymbolsRegistry.TryAssignSymbol)
            {
                var left = tokenMatching.Context.Map.GetGreatestParent(tokenMatching.AllTokens[index-1]);
                var right = tokenMatching.Context.Map.GetGreatestParent(tokenMatching.AllTokens[index + 1]);
                var block = tokenMatching.Context.Map.GetGreatestParent(tokenMatching.AllTokens[index + 2]);

                var res = new TryAssignOperationPopulateScope(left, right, block);

                if (left.Is1(out var leftValue))
                {
                    tokenMatching.Context.Map.SetElementParent(leftValue, res);
                }
                if (right.Is1(out var rightValue))
                {
                    tokenMatching.Context.Map.SetElementParent(rightValue, res);
                }
                if (block.Is1(out var blockValue))
                {
                    tokenMatching.Context.Map.SetElementParent(blockValue, res);
                }

                return TokenMatching<ISetUp<IBox<WeakTryAssignOperation>, Tpn.IValue>>.MakeMatch(
                    tokenMatching.AllTokens,
                    tokenMatching.Context,
                    res,
                    tokenMatching.StartIndex - 1,
                    tokenMatching.EndIndex + 2
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

            var nextLeft = left.TransformInner(x => x.Run(myScope, context.CreateChild(this)));
            var nextblock = block.TransformInner(x => x.Run(myScope, context.CreateChild(this)));
            var nextRight = right.TransformInner(x => x.Run(myScope, context.CreateChild(this)));

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

                canAssignFromMe.AssignTo(canBeAssignedTo);

            }
            else
            {
                // left or right are errors 
                throw new NotImplementedException();
            }

            var res = new TryAssignOperationResolveReferance(
                nextLeft.TransformInner(x => x.Resolve),
                nextRight.TransformInner(x => x.Resolve),
                nextblock.TransformInner(x => x.Resolve));

            box.Fill(new[] { OrType.Make<IResolve<IBox<IFrontendCodeElement>>, IError>(res) });

            return new SetUpResult<IBox<WeakTryAssignOperation>, Tpn.IValue>(res,
                nextLeft.TransformAndFlatten(x => x.SetUpSideNode).OrCastToOr<Tpn.ITypeProblemNode, Tpn.IValue>(Error.Other(""));
        }
    }

    internal class TryAssignOperationResolveReferance : IResolve<IBox<WeakTryAssignOperation>>
    {
        public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> left;
        public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> right;
        private readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> orType;

        public TryAssignOperationResolveReferance(
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance1,
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance2,
           IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> orType)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.orType = orType ?? throw new ArgumentNullException(nameof(orType));
        }


        public IBox<WeakTryAssignOperation> Run(Tpn.TypeSolution context)
        {
            var res = new Box<WeakTryAssignOperation>(new WeakTryAssignOperation(
                left.TransformInner(x => x.Run(context)),
                right.TransformInner(x => x.Run(context)),
                orType.TransformInner(x=>x.Run(context))));
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
