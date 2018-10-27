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
    public class ImplicitMemberMaker<T> : IMaker<T, WeakMemberDefinition>
    {
        private readonly IBox<IWeakReturnable> type;

        public ImplicitMemberMaker(Func<WeakMemberDefinition,T> make, IBox<IWeakReturnable> type)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        private Func<WeakMemberDefinition,T> Make { get; }

        public IResult<IPopulateScope<T, WeakMemberDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                return ResultExtension.Good(new ImplicitMemberPopulateScope<T>(first.Item, Make, type));
            }

            return ResultExtension.Bad<IPopulateScope<T, WeakMemberDefinition>>();
        }

    }


    public class ImplicitMemberPopulateScope<T> : IPopulateScope<T, WeakMemberDefinition>
    {
        private readonly string memberName;
        private readonly Func<WeakMemberDefinition,T> make;
        private readonly IBox<IWeakReturnable> type;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ImplicitMemberPopulateScope(string item, Func<WeakMemberDefinition,T> make, IBox<IWeakReturnable> type)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IPopulateBoxes<T, WeakMemberDefinition> Run(IPopulateScopeContext context)
        {
            
            IBox<WeakMemberDefinition> memberDef = new Box<WeakMemberDefinition>();

            if (!context.Scope.TryAddMember(DefintionLifetime.Instance,new NameKey(memberName), memberDef))
            {
                throw new Exception("bad bad bad!");
            }


            return new ImplicitMemberResolveReferance<T>(memberName,make, box,type);
        }


        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }


    }

    public class ImplicitMemberResolveReferance<T> : IPopulateBoxes<T, WeakMemberDefinition>
    {
        private readonly Func<WeakMemberDefinition,T> make;
        private readonly Box<IWeakReturnable> box;
        private readonly string memberName;
        private readonly IBox<IWeakReturnable> type;

        public ImplicitMemberResolveReferance(
            string memberName,
            Func<WeakMemberDefinition,T> make, 
            Box<IWeakReturnable> box,
            IBox<IWeakReturnable> type)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public IOpenBoxes<T, WeakMemberDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(
                new WeakMemberDefinition(false, new NameKey(memberName), type));
            return new ImplicitMemberOpenBoxes<T>(item, make);
        }
    }

    internal class ImplicitMemberOpenBoxes<T> : IOpenBoxes<T, WeakMemberDefinition>
    {
        private WeakMemberDefinition CodeElement { get; }
        private readonly Func<WeakMemberDefinition, T> make;

        public ImplicitMemberOpenBoxes(WeakMemberDefinition item, Func<WeakMemberDefinition, T> make)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public T Run(IOpenBoxesContext context)
        {
            return make(CodeElement);
        }
    }
}