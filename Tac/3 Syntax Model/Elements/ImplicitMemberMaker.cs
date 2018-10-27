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
    public class ImplicitMemberMaker : IMaker<WeakMemberReferance>
    {
        private readonly IBox<IWeakReturnable> type;

        public ImplicitMemberMaker( IBox<IWeakReturnable> type)
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


    public class ImplicitMemberPopulateScope : IPopulateScope<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly IBox<IWeakReturnable> type;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ImplicitMemberPopulateScope(string item, IBox<IWeakReturnable> type)
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


        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }


    }

    public class ImplicitMemberResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {
        private readonly Box<IWeakReturnable> box;
        private readonly string memberName;
        private readonly IBox<IWeakReturnable> type;

        public ImplicitMemberResolveReferance(
            string memberName,
            Box<IWeakReturnable> box,
            IBox<IWeakReturnable> type)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public IOpenBoxes<WeakMemberReferance> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(
                new WeakMemberReferance(
                    new Box<WeakMemberDefinition>(
                        new WeakMemberDefinition(false, new NameKey(memberName), type))));
            return new ImplicitMemberOpenBoxes(item);
        }
    }
    
    internal class ImplicitMemberOpenBoxes : IOpenBoxes<WeakMemberReferance>
    {
        public WeakMemberReferance CodeElement { get; }

        public ImplicitMemberOpenBoxes(WeakMemberReferance item)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberReferance(CodeElement);
        }
    }
}