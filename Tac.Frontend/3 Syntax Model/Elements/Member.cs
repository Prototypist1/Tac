using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;

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
                NameKey key,
                Box<IIsPossibly<IFrontendType>> box)
        {
            return new MemberResolveReferance(
                key,
                box);
        }

        private class MemberPopulateScope : IPopulateScope<WeakMemberReference>
        {
            private readonly string memberName;
            private readonly Box<IIsPossibly<IFrontendType>> box = new Box<IIsPossibly<IFrontendType>>();

            public MemberPopulateScope(string item)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return box;
            }

            public IResolvelizeScope<WeakMemberReference> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {
                var nameKey = new NameKey(memberName);
                scope.TryAddInferedMember(nameKey);

                return new MemberFinalizeScope( nameKey, box);
            }

        }


        private class MemberFinalizeScope : IResolvelizeScope<WeakMemberReference>
        {
            private readonly NameKey key;
            private readonly Box<IIsPossibly<IFrontendType>> box;

            public MemberFinalizeScope(
                NameKey key,
                Box<IIsPossibly<IFrontendType>> box)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }

            public IPopulateBoxes<WeakMemberReference> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new MemberResolveReferance(key, box);
            }
        }

        private class MemberResolveReferance : IPopulateBoxes<WeakMemberReference>
        {
            private readonly NameKey key;
            private readonly Box<IIsPossibly<IFrontendType>> box;

            public MemberResolveReferance(
                NameKey key,
                Box<IIsPossibly<IFrontendType>> box)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }

            public IIsPossibly<WeakMemberReference> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                return box.Fill(Possibly.Is(new WeakMemberReference(scope.PossiblyGetMember(false, key))));
            }
        }
    }

    
}