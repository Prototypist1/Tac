using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;

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
                IResolvableScope resolvableScope,
                NameKey key,
                Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> box)
        {
            return new MemberResolveReferance(
                resolvableScope,
                key,
                box);
        }

        private class MemberPopulateScope : IPopulateScope<WeakMemberReference>
        {
            private readonly string memberName;
            private readonly Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> box = new Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>();

            public MemberPopulateScope(string item)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
            }

            public IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> GetReturnType()
            {
                return box;
            }

            public IPopulateBoxes<WeakMemberReference> Run(IPopulateScopeContext context)
            {
                var nameKey = new NameKey(memberName);
                if (!context.Scope.TryGetMember(nameKey, false, out var memberDef) &&
                    !context.Scope.TryAddMember(
                        DefintionLifetime.Instance,
                        nameKey,
                        new Box<IIsPossibly<WeakMemberDefinition>>(
                            Possibly.Is(
                                new WeakMemberDefinition(
                                    false,
                                    nameKey,
                                    Possibly.Is(
                                        new WeakTypeReference(
                                            Possibly.Is(
                                                new Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>(
                                                    Possibly.Is<IConvertableFrontendType<IVerifiableType>>(
                                                        PrimitiveTypes.CreateAnyType()))))))))))
                {
                    throw new Exception("uhh that is not right");
                }

                return new MemberResolveReferance(context.GetResolvableScope(), nameKey, box);
            }

        }

        private class MemberResolveReferance : IPopulateBoxes<WeakMemberReference>
        {
            private readonly IResolvableScope resolvableScope;
            private readonly NameKey key;
            private readonly Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> box;

            public MemberResolveReferance(
                IResolvableScope resolvableScope,
                NameKey key,
                Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> box)
            {
                this.resolvableScope = resolvableScope ?? throw new ArgumentNullException(nameof(resolvableScope));
                this.key = key ?? throw new ArgumentNullException(nameof(key));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }

            public IIsPossibly<WeakMemberReference> Run(IResolveReferenceContext context)
            {
                return box.Fill(Possibly.Is(new WeakMemberReference(resolvableScope.PossiblyGetMember(false, key))));
            }
        }
    }

    
}