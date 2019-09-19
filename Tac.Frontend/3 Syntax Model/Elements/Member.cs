using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement>> StaticMemberMaker = AddElementMakers(
            () => new MemberMaker());
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement>> MemberMaker = StaticMemberMaker;
    }
}


namespace Tac.Semantic_Model
{
    internal class MemberMaker : IMaker<IPopulateScope<WeakMemberReference>>
    {
        public MemberMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakMemberReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new MemberPopulateScope(first.Item)); ;
            }
            return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeNotMatch(
                    matching.Context);
        }

        public static IPopulateScope<WeakMemberReference> PopulateScope(string item)
        {
            return new MemberPopulateScope(item);
        }
        public static IPopulateBoxes<WeakMemberReference> PopulateBoxes(
                NameKey key)
        {
            return new MemberResolveReferance(
                key);
        }

        private class MemberPopulateScope : IPopulateScope<WeakMemberReference>
        {
            private readonly string memberName;

            public MemberPopulateScope(string item)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
            }

            public IResolvelizeScope<WeakMemberReference> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {
                var nameKey = new NameKey(memberName);
                var memberBuilder = scope.GetOrAddInferedMember(nameKey, new MemberBuilder());

                return new MemberFinalizeScope( nameKey, memberBuilder);
            }

        }


        private class MemberFinalizeScope : IResolvelizeScope<WeakMemberReference>, IMemberBuilder
        {
            private readonly NameKey key;
            private readonly IMemberBuilder memberBuilder;

            public MemberFinalizeScope(
                NameKey key,
                IMemberBuilder memberBuilder)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
                this.memberBuilder = memberBuilder ?? throw new ArgumentNullException(nameof(memberBuilder));
            }

            public void AcceptsType(Box<IFrontendType> box)
            {
                memberBuilder.AcceptsType(box);
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