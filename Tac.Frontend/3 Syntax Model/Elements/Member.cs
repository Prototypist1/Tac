using Prototypist.LeftToRight;
using System;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticMemberMaker = AddElementMakers(
            () => new MemberMaker());
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> MemberMaker = StaticMemberMaker;
    }
}


namespace Tac.Semantic_Model
{
    internal class MemberMaker : IMaker<ISetUp<WeakMemberReference, LocalTpn.TypeProblem2.Member>>
    {
        public MemberMaker()
        {
        }
        
        public ITokenMatching<ISetUp<WeakMemberReference, LocalTpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakMemberReference, LocalTpn.TypeProblem2.Member>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new MemberPopulateScope(first.Item)); ;
            }
            return TokenMatching<ISetUp<WeakMemberReference, LocalTpn.TypeProblem2.Member>>.MakeNotMatch(
                    matching.Context);
        }

        public static ISetUp<WeakMemberReference, LocalTpn.TypeProblem2.Member> PopulateScope(string item)
        {
            return new MemberPopulateScope(item);
        }
        public static IResolve<WeakMemberReference> PopulateBoxes(
                NameKey key)
        {
            return new MemberResolveReferance(
                key);
        }

        private class MemberPopulateScope : ISetUp<WeakMemberReference, LocalTpn.TypeProblem2.Member>
        {
            private readonly string memberName;

            public MemberPopulateScope(string item)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
            }

            public ISetUpResult<WeakMemberReference, LocalTpn.TypeProblem2.Member> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                var nameKey = new NameKey(memberName);
                var member = context.TypeProblem.CreateMemberPossiblyOnParent(scope, nameKey);

                return new SetUpResult<WeakMemberReference, LocalTpn.TypeProblem2.Member>(new MemberResolveReferance( nameKey),member);
            }

        }


        private class MemberResolveReferance : IResolve<WeakMemberReference>
        {
            private readonly NameKey key;

            public MemberResolveReferance(
                NameKey key)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IIsPossibly<WeakMemberReference> Run(IResolvableScope scope, IResolveContext context)
            {
                return Possibly.Is(new WeakMemberReference(scope.PossiblyGetMember(false, key)));
            }
        }
    }

    
}