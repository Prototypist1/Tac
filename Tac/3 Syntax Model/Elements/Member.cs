using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    internal class MemberMaker : IMaker<IPopulateScope<WeakMemberReferance>>
    {
        public MemberMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakMemberReferance>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReferance>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new MemberPopulateScope(first.Item)); ;
            }
            return TokenMatching<IPopulateScope<WeakMemberReferance>>.MakeNotMatch(
                    matching.Context);
        }
    }

    internal class MemberPopulateScope : IPopulateScope<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly Box<IIsPossibly<IVarifiableType>> box = new Box<IIsPossibly<IVarifiableType>>();

        public MemberPopulateScope(string item)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
        }

        public IBox<IIsPossibly<IVarifiableType>> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            var nameKey = new NameKey(memberName);
            if (!context.Scope.TryGetMember(nameKey, false, out var memberDef) && 
                !context.Scope.TryAddMember(
                    DefintionLifetime.Instance,
                    nameKey, 
                    new Box<IIsPossibly< WeakMemberDefinition>>(
                        Possibly.Is(
                            new WeakMemberDefinition(
                                false,
                                nameKey,
                                new WeakTypeReferance( 
                                    Possibly.Is( 
                                        new Box<IIsPossibly< IVarifiableType>>(
                                            Possibly.Is(
                                                new AnyType())))))))))
            {
                throw new Exception("uhh that is not right");
            }
            
            return new MemberResolveReferance(context.GetResolvableScope(), nameKey, box);
        }

    }

    internal class MemberResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {
        private readonly IResolvableScope resolvableScope;
        private readonly NameKey key;
        private readonly Box<IIsPossibly<IVarifiableType>> box;

        public MemberResolveReferance(
            IResolvableScope resolvableScope,
            NameKey key, 
            Box<IIsPossibly<IVarifiableType>> box)
        {
            this.resolvableScope = resolvableScope ?? throw new ArgumentNullException(nameof(resolvableScope));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IIsPossibly<WeakMemberReferance> Run(IResolveReferanceContext context)
        {
            return box.Fill( Possibly.Is(new WeakMemberReferance(resolvableScope.PossiblyGetType(key,false))));
        }
    }
    
}