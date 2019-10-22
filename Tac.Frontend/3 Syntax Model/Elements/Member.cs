using Prototypist.LeftToRight;
using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticMemberMaker = AddElementMakers(
            () => new MemberMaker());
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> MemberMaker = StaticMemberMaker;
    }
}


namespace Tac.Semantic_Model
{
    internal class MemberMaker : IMaker<IPopulateScope<WeakMemberReference,Tpn.IMember>>
    {
        public MemberMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakMemberReference, Tpn.IMember>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReference, Tpn.IMember>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new MemberPopulateScope(first.Item)); ;
            }
            return TokenMatching<IPopulateScope<WeakMemberReference, Tpn.IMember>>.MakeNotMatch(
                    matching.Context);
        }

        public static IPopulateScope<WeakMemberReference, Tpn.IMember> PopulateScope(string item)
        {
            return new MemberPopulateScope(item);
        }
        public static IPopulateBoxes<WeakMemberReference> PopulateBoxes(
                NameKey key)
        {
            return new MemberResolveReferance(
                key);
        }

        private class MemberPopulateScope : IPopulateScope<WeakMemberReference, Tpn.IMember>
        {
            private readonly string memberName;

            public MemberPopulateScope(string item)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
            }

            public IResolvelizeScope<WeakMemberReference, Tpn.IMember> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {
                var nameKey = new NameKey(memberName);
                var member = context.TypeProblem.CreateMember(scope, nameKey);
                
                scope.Cast<ISetUpScope>().MightHaveMember(member);

                return new MemberFinalizeScope( nameKey, member);
            }

        }


        private class MemberFinalizeScope : IResolvelizeScope<WeakMemberReference, Tpn.IMember>
        {
            private readonly NameKey key;

            public MemberFinalizeScope(
                NameKey key, Tpn.IMember setUpSideNode)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
                SetUpSideNode = setUpSideNode ?? throw new ArgumentNullException(nameof(setUpSideNode));
            }

            public Tpn.IMember SetUpSideNode
            {
                get;
            }

            public IPopulateBoxes<WeakMemberReference> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new MemberResolveReferance(key);
            }
        }

        private class MemberResolveReferance : IPopulateBoxes<WeakMemberReference>
        {
            private readonly NameKey key;

            public MemberResolveReferance(
                NameKey key)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IIsPossibly<WeakMemberReference> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                return Possibly.Is(new WeakMemberReference(scope.PossiblyGetMember(false, key)));
            }
        }
    }

    
}