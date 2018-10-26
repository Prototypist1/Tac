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
    public class ImplicitMemberMaker : IMaker<WeakMemberDefinition>
    {
        private readonly IBox<IWeakReturnable> type;

        public ImplicitMemberMaker(WeakMemberDefinition.Make make, IBox<IWeakReturnable> type)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        private WeakMemberDefinition.Make Make { get; }

        public IResult<IPopulateScope<WeakMemberDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                return ResultExtension.Good(new ImplicitMemberPopulateScope(first.Item, Make, type));
            }

            return ResultExtension.Bad<IPopulateScope<WeakMemberDefinition>>();
        }

    }


    public class ImplicitMemberPopulateScope : IPopulateScope<WeakMemberDefinition>
    {
        private readonly string memberName;
        private readonly WeakMemberDefinition.Make make;
        private readonly IBox<IWeakReturnable> type;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ImplicitMemberPopulateScope(string item, WeakMemberDefinition.Make make, IBox<IWeakReturnable> type)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IPopulateBoxes<WeakMemberDefinition> Run(IPopulateScopeContext context)
        {
            
            IBox<WeakMemberDefinition> memberDef = new Box<WeakMemberDefinition>();

            if (!context.Scope.TryAddMember(DefintionLifetime.Instance,new NameKey(memberName), memberDef))
            {
                throw new Exception("bad bad bad!");
            }


            return new ImplicitMemberResolveReferance(memberName,make, box,type);
        }


        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }


    }

    public class ImplicitMemberResolveReferance : IPopulateBoxes<WeakMemberDefinition>
    {
        private readonly WeakMemberDefinition.Make make;
        private readonly Box<IWeakReturnable> box;
        private readonly string memberName;
        private readonly IBox<IWeakReturnable> type;

        public ImplicitMemberResolveReferance(
            string memberName,
            WeakMemberDefinition.Make make, 
            Box<IWeakReturnable> box,
            IBox<IWeakReturnable> type)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public WeakMemberDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(
                new WeakMemberDefinition(false, new NameKey(memberName), type));
        }
    }
}