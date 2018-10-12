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
    public class ImplicitMemberMaker : IMaker<Member>
    {
        private readonly IBox<IReturnable> type;

        public ImplicitMemberMaker(Func<int, MemberDefinition, Member> make, IBox<IReturnable> type)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        private Func<int, MemberDefinition, Member> Make { get; }

        public IResult<IPopulateScope<Member>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                return ResultExtension.Good(new ImplicitMemberPopulateScope(first.Item, Make, type));
            }

            return ResultExtension.Bad<IPopulateScope<Member>>();
        }

    }


    public class ImplicitMemberPopulateScope : IPopulateScope<Member>
    {
        private readonly string memberName;
        private readonly Func<int, MemberDefinition, Member> make;
        private readonly IBox<IReturnable> type;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public ImplicitMemberPopulateScope(string item, Func<int, MemberDefinition, Member> make, IBox<IReturnable> type)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IResolveReference<Member> Run(IPopulateScopeContext context)
        {
            
            var innerType = new MemberDefinition(false, new NameKey(memberName), type);
            IBox<MemberDefinition> memberDef = new Box<MemberDefinition>(innerType);

            if (!context.TryAddMember(new NameKey(memberName), memberDef))
            {
                throw new Exception("bad bad bad!");
            }


            return new ImplicitMemberResolveReferance(innerType, make, box);
        }


        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }


    }

    public class ImplicitMemberResolveReferance : IResolveReference<Member>
    {
        private readonly MemberDefinition memberDef;
        private readonly Func<int, MemberDefinition, Member> make;
        private readonly Box<IReturnable> box;

        public ImplicitMemberResolveReferance(
            MemberDefinition innerType, 
            Func<int, MemberDefinition, Member> make, 
            Box<IReturnable> box)
        {
            memberDef = innerType ?? throw new ArgumentNullException(nameof(innerType));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        
        public Member Run(IResolveReferanceContext context)
        {
            return box.Fill(make(0, memberDef));
        }
    }
}