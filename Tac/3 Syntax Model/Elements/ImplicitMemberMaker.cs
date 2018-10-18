using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public class ImplicitMemberMaker : IMaker<MemberDefinition>
    {
        private readonly IBox<IReturnable> type;

        public ImplicitMemberMaker(MemberDefinition.Make make, IBox<IReturnable> type)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        private MemberDefinition.Make Make { get; }

        public IResult<IPopulateScope<MemberDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                return ResultExtension.Good(new ImplicitMemberPopulateScope(first.Item, Make, type));
            }

            return ResultExtension.Bad<IPopulateScope<MemberDefinition>>();
        }

    }


    public class ImplicitMemberPopulateScope : IPopulateScope<MemberDefinition>
    {
        private readonly string memberName;
        private readonly MemberDefinition.Make make;
        private readonly IBox<IReturnable> type;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public ImplicitMemberPopulateScope(string item, MemberDefinition.Make make, IBox<IReturnable> type)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IResolveReference<MemberDefinition> Run(IPopulateScopeContext context)
        {
            
            IBox<MemberDefinition> memberDef = new Box<MemberDefinition>();

            if (!context.TryAddMember(new NameKey(memberName), memberDef))
            {
                throw new Exception("bad bad bad!");
            }


            return new ImplicitMemberResolveReferance(memberName,make, box,type);
        }


        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }


    }

    public class ImplicitMemberResolveReferance : IResolveReference<MemberDefinition>
    {
        private readonly MemberDefinition.Make make;
        private readonly Box<IReturnable> box;
        private readonly string memberName;
        private readonly IBox<IReturnable> type;

        public ImplicitMemberResolveReferance(
            string memberName,
            MemberDefinition.Make make, 
            Box<IReturnable> box,
            IBox<IReturnable> type)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public MemberDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(
                new MemberDefinition(false, new NameKey(memberName), type));
        }
    }
}