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
using Tac.SemanticModel;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;

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
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticAssertAssignMaker = AddOperationMatcher(() => new AssertAssignOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> AssertAssignMaker = StaticAssertAssignMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}




namespace Tac.SemanticModel.Operations
{



    internal class WeakAssignOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IAssignOperation>, IReturn
    {
        // right is really a member reference or a member definition
        // I think a member defintion return a member reference
        // so always a member referece
        public WeakAssignOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }

        public override IBuildIntention<IAssignOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = AssignOperation.Create();
            return new BuildIntention<IAssignOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public IOrType<IFrontendType, IError> Returns() => Left.TransformAndFlatten(x => {
            if (x is IReturn @return) {
                return @return.Returns();
            }
            return OrType.Make<IFrontendType, IError>(Error.Other("left needs to return"));
        } );

        public override IEnumerable<IError> Validate()
        {
            foreach (var error in base.Validate())
            {
                yield return error;
            }

            var leftTypeOrErrors = Left.ReturnsTypeOrErrors();
            var rightTypeOrErrors = Right.ReturnsTypeOrErrors();

            foreach (var error in leftTypeOrErrors.SwitchReturns(l =>
               rightTypeOrErrors.SwitchReturns<IEnumerable<IError>>(r => {
                   if (!r.TheyAreUs(l, new List<(IFrontendType, IFrontendType)>()).SwitchReturns(x=>x,x=>false))
                   {
                       return new[] { Error.Other($"can not assign {l} to {r}") };
                   }
                   return Array.Empty<IError>();
               }, r => {
                   return new IError[] { r };
               })
            , l => rightTypeOrErrors.SwitchReturns<IEnumerable<IError>>(r => new IError[] { l }, r => new IError[] { l, r })))
            {
                yield return error;
            }
        }
    }

    //I think in theory you could assing to things that return references
    // like Method<int,ref int> t
    // 3 =: ( 5 > t )
    internal class AssertAssignOperationMaker : IMaker<ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>>
    {

        public AssertAssignOperationMaker()
        {
        }
        
        public ITokenMatching<ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticAssertAssignSymbol), out (ISetUp lhs, ISetUp rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(match.lhs.SafeCastTo(out ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode> _));
                var right = OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(match.rhs.SafeCastTo(out ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode> _));

                var res = new WeakAssignOperationPopulateScope(left, right);

                //if (left.Is1(out var leftValue))
                //{
                //    tokenMatching.Context.Map.SetElementParent(leftValue, res);
                //}
                //if (right.Is1(out var rightValue))
                //{
                //    tokenMatching.Context.Map.SetElementParent(rightValue, res);
                //}

                return TokenMatching<ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>>.MakeMatch(
                    tokenMatching,
                    res,
                    matched.EndIndex); ;
            }

            return TokenMatching<ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }



    }

    internal class WeakAssignOperationPopulateScope : ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>
    {
        private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left;
        private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right;

        public WeakAssignOperationPopulateScope(
            IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left,
            IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right)); ;
        }

        public ISetUpResult<IBox<WeakAssignOperation>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            var nextLeft = left.TransformInner(x => x.Run(scope, context.CreateChildContext(this)));
            var nextRight = right.TransformInner(x => x.Run(scope, context.CreateChildContext(this)));

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

            return new SetUpResult<IBox<WeakAssignOperation>, Tpn.IValue>(new WeakAssignOperationResolveReferance(
                nextLeft.TransformInner(x => x.Resolve),
                nextRight.TransformInner(x => x.Resolve)),
                nextLeft.TransformAndFlatten(x => x.SetUpSideNode).OrCastToOr<Tpn.ITypeProblemNode, Tpn.IValue>(Error.Other("")));
        }
    }

    internal class WeakAssignOperationResolveReferance : IResolve<IBox<WeakAssignOperation>>
    {
        public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> left;
        public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> right;

        public WeakAssignOperationResolveReferance(
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance1,
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance2)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
        }


        public IBox<WeakAssignOperation> Run(Tpn.TypeSolution context)
        {
            var res = new Box<WeakAssignOperation>(new WeakAssignOperation(
                left.TransformInner(x => x.Run(context)),
                right.TransformInner(x => x.Run(context))));
            return res;
        }
    }


    //internal class AssertAssignInObjectOperationMaker : IMaker<ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>>
    //{

    //    public AssertAssignInObjectOperationMaker()
    //    {
    //    }

    //    public ITokenMatching<ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
    //    {
    //        var matching = tokenMatching
    //            .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticAssertAssignSymbol), out (IToken perface, AtomicToken token, IToken rhs) match);
    //        if (matching is IMatchedTokenMatching matched)
    //        {
    //            var left = matching.Context.Map.GetGreatestParent(match.perface);
    //            var right = matching.Context.Map.GetGreatestParent(match.rhs);

    //            return TokenMatching<ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>>.MakeMatch(
    //                matched.AllTokens,
    //                matched.Context,
    //                new WeakAssignOperationPopulateScope(left, right),
    //                matched.StartIndex,
    //                matched.EndIndex);
    //        }

    //        return TokenMatching<ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>>.MakeNotMatch(
    //                matching.Context);
    //    }


    //}


    //internal class WeakAssignOperationPopulateScope : ISetUp<IBox<WeakAssignOperation>, Tpn.IValue>
    //{
    //    private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left;
    //    private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right;

    //    public WeakAssignOperationPopulateScope(
    //        IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left,
    //        IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> right)
    //    {
    //        this.left = left ?? throw new ArgumentNullException(nameof(left));
    //        this.right = right ?? throw new ArgumentNullException(nameof(right)); ;
    //    }

    //    public ISetUpResult<IBox<WeakAssignOperation>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
    //    {
    //        if (!(scope is Tpn.TypeProblem2.Object @object))
    //        {
    //            throw new NotImplementedException("This should probably be an ierror");
    //        }

    //        var nextLeft = left.TransformInner(x => x.Run(@object.InitizationScope, context.CreateChild(this)));
    //        var nextRight = right.TransformInner(x => x.Run(scope, context.CreateChild(this)));

    //        if (nextLeft.Is1(out var nextLeft1) && nextLeft1.SetUpSideNode.Is1(out var node1) && nextRight.Is1(out var nextRight1) && nextRight1.SetUpSideNode.Is1(out var node2))
    //        {
    //            if (!(node1 is Tpn.ICanAssignFromMe canAssignFromMe))
    //            {
    //                // todo I need real error handling
    //                // probably I need somewhere to stuff additional errors
    //                throw new NotImplementedException($"can not assign from {nextLeft1.SetUpSideNode}");
    //            }

    //            if (!(node2 is Tpn.ICanBeAssignedTo canBeAssignedTo))
    //            {
    //                // todo I need real error handling
    //                throw new NotImplementedException($"can not assign to {nextRight1.SetUpSideNode}");
    //            }

    //            canAssignFromMe.AssignTo(canBeAssignedTo);

    //        }
    //        else
    //        {
    //            // left or right are errors 
    //            throw new NotImplementedException();
    //        }

    //        return new SetUpResult<IBox<WeakAssignOperation>, Tpn.IValue>(new WeakAssignOperationResolveReferance(
    //            nextLeft.TransformInner(x => x.Resolve),
    //            nextRight.TransformInner(x => x.Resolve)),
    //            nextLeft.TransformAndFlatten(x => x.SetUpSideNode).OrCastToOr<Tpn.ITypeProblemNode, Tpn.IValue>(Error.Other("")));
    //    }
    //}

    //internal class WeakAssignOperationResolveReferance : IResolve<IBox<WeakAssignOperation>>
    //{
    //    public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> left;
    //    public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> right;

    //    public WeakAssignOperationResolveReferance(
    //        IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance1,
    //        IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReferance2)
    //    {
    //        left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
    //        right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
    //    }


    //    public IBox<WeakAssignOperation> Run(Tpn.TypeSolution context)
    //    {
    //        var res = new Box<WeakAssignOperation>(new WeakAssignOperation(
    //            left.TransformInner(x => x.Run(context)),
    //            right.TransformInner(x => x.Run(context))));
    //        return res;
    //    }
    //}
}
