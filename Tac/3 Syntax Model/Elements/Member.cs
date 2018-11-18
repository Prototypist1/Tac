using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
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
        
        public ITokenMatching<IPopulateScope<WeakMemberReferance>> TryMake(ITokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching.IsMatch)
            {
                return TokenMatching<IPopulateScope<WeakMemberReferance>>.Match(
                    matching.Tokens,
                    matching.Context, 
                    new MemberPopulateScope(first.Item)); ;
            }
            return TokenMatching<IPopulateScope<WeakMemberReferance>>.NotMatch(
                    matching.Tokens,
                    matching.Context);
        }
    }

    internal class MemberPopulateScope : IPopulateScope<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly Box<IVarifiableType> box = new Box<IVarifiableType>();

        public MemberPopulateScope(string item)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            var nameKey = new NameKey(memberName);
            if (!context.Scope.TryGetMember(nameKey, false, out var memberDef) && 
                !context.Scope.TryAddMember(DefintionLifetime.Instance,nameKey, new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,nameKey,new Box<IVarifiableType>(new AnyType())))))
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
        private readonly Box<IVarifiableType> box;

        public MemberResolveReferance(
            IResolvableScope resolvableScope,
            NameKey key, 
            Box<IVarifiableType> box)
        {
            this.resolvableScope = resolvableScope ?? throw new ArgumentNullException(nameof(resolvableScope));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public WeakMemberReferance Run(IResolveReferanceContext context)
        {
            return box.Fill(new WeakMemberReferance(resolvableScope.GetMemberOrThrow(key, false)));
        }
    }
    
}