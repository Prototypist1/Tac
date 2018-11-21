using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Frontend._2_Parser;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    internal class ImplicitMemberMaker : IMaker<IPopulateScope<WeakMemberReferance>>
    {
        private readonly IBox<IVarifiableType> type;

        public ImplicitMemberMaker( IBox<IVarifiableType> type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }


        public ITokenMatching<IPopulateScope<WeakMemberReferance>> TryMake(ITokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("var"), out var _)
                .Has(new NameMaker(), out var first);

            if (matching.IsMatch)
            {

                return TokenMatching<IPopulateScope<WeakMemberReferance>>.Match(
                matching.Tokens,
                matching.Context,
                new ImplicitMemberPopulateScope(first.Item, type));
            }

            return TokenMatching<IPopulateScope<WeakMemberReferance>>.NotMatch(
                matching.Tokens,
                matching.Context);
        }

    }


    internal class ImplicitMemberPopulateScope : IPopulateScope<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly IBox<IVarifiableType> type;
        private readonly Box<IVarifiableType> box = new Box<IVarifiableType>();

        public ImplicitMemberPopulateScope(string item, IBox<IVarifiableType> type)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            
            IBox<WeakMemberDefinition> memberDef = new Box<WeakMemberDefinition>();

            if (!context.Scope.TryAddMember(DefintionLifetime.Instance,new NameKey(memberName), memberDef))
            {
                throw new Exception("bad bad bad!");
            }


            return new ImplicitMemberResolveReferance(memberName,box,type);
        }


        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }


    }

    internal class ImplicitMemberResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {
        private readonly Box<IVarifiableType> box;
        private readonly string memberName;
        private readonly IBox<IVarifiableType> type;

        public ImplicitMemberResolveReferance(
            string memberName,
            Box<IVarifiableType> box,
            IBox<IVarifiableType> type)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public WeakMemberReferance Run(IResolveReferanceContext context)
        {
           return box.Fill(
                new WeakMemberReferance(
                    new Box<WeakMemberDefinition>(
                        new WeakMemberDefinition(false, new NameKey(memberName), new WeakTypeReferance(type)))));
        }
    }
}