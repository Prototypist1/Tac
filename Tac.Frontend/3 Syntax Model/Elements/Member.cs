using System;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.Operations;
using Tac.Frontend.SyntaxModel.Operations;
using Prototypist.Toolbox;
using Tac.Frontend.Parser;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticMemberMaker = AddElementMakers(
            () => new MemberMaker());
#pragma warning disable CA1823 
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> MemberMaker = StaticMemberMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel
{
    internal class MemberMaker : IMaker<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>>
    {
        public MemberMaker()
        {
        }
        
        public ITokenMatching<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
           return tokenMatching
                .Has(new NameMaker())
                .ConvertIfMatched(token => new MemberPopulateScope(token.Item));
        }

        public static ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> PopulateScope(string item)
        {
            return new MemberPopulateScope(item);
        }

        private class MemberPopulateScope : ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>
        {
            private readonly string memberName;

            public MemberPopulateScope(string item)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
            }

            public ISetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
            {
                // TODO can you make a something like Type {x;y}
                // i think so
                // so this may not always need a runtimeScope
                // but before I make that work I need to review the whole workflow


                if (!(scope is Tpn.IHavePossibleMembers possibleScope))
                {
                    throw new NotImplementedException("this should be an IError");
                }

                var nameKey = new NameKey(memberName);
                var member = context.TypeProblem.CreateMemberPossiblyOnParent(scope, possibleScope, nameKey,new WeakMemberDefinitionConverter(false,nameKey));

                return new SetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>(new MemberResolveReferance(member), OrType.Make<Tpn.TypeProblem2.Member, IError>(member));
            }

        }

        private class MemberResolveReferance : IResolve<IBox< WeakMemberReference>>
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