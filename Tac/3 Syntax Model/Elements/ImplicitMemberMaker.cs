using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    internal class ImplicitMemberMaker : IMaker<WeakMemberReferance>
    {
        private readonly IBox<IType> type;

        public ImplicitMemberMaker( IBox<IType> type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }


        public IResult<IPopulateScope<WeakMemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                return ResultExtension.Good(new ImplicitMemberPopulateScope(first.Item, type));
            }

            return ResultExtension.Bad<IPopulateScope<WeakMemberReferance>>();
        }

    }


    internal class ImplicitMemberPopulateScope : IPopulateScope<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly IBox<IType> type;
        private readonly Box<IType> box = new Box<IType>();

        public ImplicitMemberPopulateScope(string item, IBox<IType> type)
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


        public IBox<IType> GetReturnType()
        {
            return box;
        }


    }

    internal class ImplicitMemberResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {
        private readonly Box<IType> box;
        private readonly string memberName;
        private readonly IBox<IType> type;

        public ImplicitMemberResolveReferance(
            string memberName,
            Box<IType> box,
            IBox<IType> type)
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
                        new WeakMemberDefinition(false, new NameKey(memberName), type))));
        }
    }
}