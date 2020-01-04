using System;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Operations;
using Tac.Frontend._3_Syntax_Model.Operations;
using Prototypist.Toolbox;
using Tac.Frontend.Parser;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticMemberMaker = AddElementMakers(
            () => new MemberMaker());
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> MemberMaker = StaticMemberMaker;
    }
}


namespace Tac.Semantic_Model
{
    internal class MemberMaker : IMaker<ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>>
    {
        public MemberMaker()
        {
        }
        
        public ITokenMatching<ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new MemberPopulateScope(first.Item)); ;
            }
            return TokenMatching<ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>>.MakeNotMatch(
                    matching.Context);
        }

        public static ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member> PopulateScope(string item)
        {
            return new MemberPopulateScope(item);
        }

        private class MemberPopulateScope : ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>
        {
            private readonly string memberName;

            public MemberPopulateScope(string item)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
            }

            public ISetUpResult<WeakMemberReference, Tpn.TypeProblem2.Member> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var nameKey = new NameKey(memberName);
                var member = context.TypeProblem.CreateMemberPossiblyOnParent(scope, nameKey,new WeakMemberDefinitionConverter(false,nameKey));

                return new SetUpResult<WeakMemberReference, Tpn.TypeProblem2.Member>(new MemberResolveReferance(member),member);
            }

        }

        private class MemberResolveReferance : IResolve<WeakMemberReference>
        {
            private readonly Tpn.TypeProblem2.Member member;

            public MemberResolveReferance(Tpn.TypeProblem2.Member member)
            {
                this.member = member ?? throw new ArgumentNullException(nameof(member));
            }

            public IBox<WeakMemberReference> Run(Tpn.ITypeSolution context)
            {
                return new Box<WeakMemberReference>(new WeakMemberReference(context.GetMember(member)));
            }
        }
    }

    
}