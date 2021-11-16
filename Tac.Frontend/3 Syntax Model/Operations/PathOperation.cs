using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Tac.SemanticModel;

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticPathSymbol = StaticSymbolsRegistry.AddOrThrow(".");
        public readonly string PathSymbol = StaticPathSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticPathMaker = AddOperationMatcher(() => new PathOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> PathMaker = StaticPathMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.SemanticModel.Operations
{
    internal class WeakPathOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IPathOperation>, IReturn
    {
        public WeakPathOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }

        public override IBuildIntention<IPathOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = PathOperation.Create();
            return new BuildIntention<IPathOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context),
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns()
        {
            return Right.TransformAndFlatten(x =>
            {
                if (x.GetValue() is IReturn @return) {
                    return @return.Returns();
                }
                return OrType.Make<IFrontendType<IVerifiableType>, IError>(Error.Other($"{Right} should return"));
            });
        }

        public override IEnumerable<IError> Validate()
        {
            foreach (var error in base.Validate())
            {
                yield return error;
            }

            // really not sure how to validate this
            // this will do for now
        }

    }


    internal class PathOperationMaker : IMaker<ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>>
    {

        public PathOperationMaker()
        {
        }


        public ITokenMatching<ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticPathSymbol), out (ISetUp lhs, ISetUp rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {


                    var left = OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(match.lhs.SafeCastTo(out ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode> _));
                    var right = OrType.Make<MemberPopulateScope, IError>(match.rhs.SafeCastTo(out MemberPopulateScope _));


                    var res = new WeakPathOperationPopulateScope(left, right.TransformInner(x=>x.memberName));

                    //if (left.Is1(out var leftValue))
                    //{
                    //    tokenMatching.Context.Map.SetElementParent(leftValue, res);
                    //}
                    //if (right.Is1(out var rightValue))
                    //{
                    //    tokenMatching.Context.Map.SetElementParent(rightValue, res);
                    //}

                    return TokenMatching<ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>>.MakeMatch(
                        tokenMatching,
                        res,
                        matched.EndIndex);
                
            }

            return TokenMatching<ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>>.MakeNotMatch(
                    matching.Context);
        }


    }

    internal class WeakPathOperationPopulateScope : ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>
    {
        private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left;
        private readonly IOrType<string,IError> name;

        public WeakPathOperationPopulateScope(IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left,
             IOrType<string, IError> name)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public ISetUpResult<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            scope = scope.EnterInitizaionScopeIfNessisary();

            var nextLeft = left.TransformInner(x => x.Run(scope, context.CreateChildContext(this)));

            var member = nextLeft.SwitchReturns(
                good =>
                name.SwitchReturns( 
                    actualName =>
                    {
                        if (good.SetUpSideNode.Is1(out var nodeLeft) && nodeLeft is Tpn.IValue value)
                        {
                            return OrType.Make<Tpn.TypeProblem2.Member, IError>(context.TypeProblem.CreateHopefulMember(
                                value,
                                new NameKey(actualName)));
                        }
                        else
                        {
                            return OrType.Make<Tpn.TypeProblem2.Member, IError>(Error.Other(""));
                                // todo better error handling 
                                throw new NotImplementedException($"can not . off {good.SetUpSideNode}");
                        }
                    }, 
                    error => OrType.Make<Tpn.TypeProblem2.Member, IError>(Error.Cascaded("We needed ", error))),
                error => OrType.Make<Tpn.TypeProblem2.Member, IError>(Error.Cascaded("We needed ", error)));

            return new SetUpResult<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>(new WeakPathOperationResolveReference(
                nextLeft.TransformInner(x => x.Resolve),
                member,
                new NameKey(name.Is1OrThrow())),// Is1OrThrow is very sloppy 
                member);
        }
    }

    internal class WeakPathOperationResolveReference : IResolve<IBox<WeakPathOperation>>
    {
        readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> left;
        readonly IOrType<Tpn.TypeProblem2.Member, IError> member;
        readonly IKey key;

        public WeakPathOperationResolveReference(
            IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReference,
            IOrType<Tpn.TypeProblem2.Member, IError> member,
            IKey key)
        {
            left = resolveReference ?? throw new ArgumentNullException(nameof(resolveReference));
            this.member = member ?? throw new ArgumentNullException(nameof(member));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<WeakPathOperation> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {
            var leftRes = left.TransformInner(x => x.Run(context, stack));

            var res = new Box<WeakPathOperation>(new WeakPathOperation(
                leftRes,
                member.SwitchReturns(
                    x => OrType.Make<IBox<IFrontendCodeElement>, IError>(new FuncBox<IFrontendCodeElement>(()=> {


                        // TODO 
                        // I don't I should need to ask the solution for the member def, the left's return type should give me that

                        // why doesn't TryGetMember return a member definition!
                        // probably because of Ortypes

                        // I thinkg TypeSolution TryGetMember is really just for TypeProblem2.Method and TypeProblem2.Scope
                        // maybe I am thinking about this wrong and scopeCache only need to exist during TypeSolution's constructor
                        var returns = leftRes.Is1OrThrow().GetValue().CastTo<IReturn>().Returns().Is1OrThrow();

                        // smells
                        if (returns.SafeIs(out Tac.SyntaxModel.Elements.AtomicTypes.RefType refType)) {
                            returns = refType.inner.Is1OrThrow();
                        }

                        var memberDef=  returns.TryGetMember(key, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>());

                        //context.TryGetMember(,key)

                        return new WeakMemberReference(new Box<IOrType<WeakMemberDefinition,IError>>(memberDef.Is1OrThrow()));


                        //WeakMemberReference memberRef = leftRes.Is1OrThrow().GetValue().CastTo<WeakMemberReference>();
                        //var memberDef = memberRef.MemberDefinition.GetValue().Type.GetValue().Is1OrThrow().TryGetMember(key, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>());

                        ////context.TryGetMember(,key)

                        //return new WeakMemberReference(new Box<WeakMemberDefinition>(memberDef.Is1OrThrow().Is1OrThrow()));
                    })),
                    y => OrType.Make<IBox<IFrontendCodeElement>, IError>(Error.Cascaded("", y)))));
            return res;
        }
    }
}
