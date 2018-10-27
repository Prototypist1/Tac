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
    public interface IMemberReferance : ICodeElement, IReturnable
    {
        IMemberDefinition MemberDefinition { get; }
    }

    public class WeakMemberReferance : IWeakCodeElement, IWeakReturnable
    {
        public WeakMemberReferance(IBox<WeakMemberDefinition> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public IBox<WeakMemberDefinition> MemberDefinition { get; }

        public IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return MemberDefinition.GetValue();
        }
    }

    public class MemberReferanceMaker : IMaker<WeakMemberReferance>
    {
        public MemberReferanceMaker(
            IBox<IWeakReturnable> lhs)
        {
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        private readonly IBox<IWeakReturnable> lhs;

        public IResult<IPopulateScope<WeakMemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberReferancePopulateScope(first.Item, lhs));
            }

            return ResultExtension.Bad<IPopulateScope<WeakMemberReferance>>();
        }
    }

    public class MemberReferancePopulateScope : IPopulateScope< WeakMemberReferance>
    {

        private readonly IBox<IWeakReturnable> lhs;
        private readonly string memberName;
        private readonly DelegateBox<WeakMemberDefinition> box = new DelegateBox<WeakMemberDefinition>();

        public MemberReferancePopulateScope( string item, IBox<IWeakReturnable> lhs)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {

            return new MemberReferanceResolveReferance(memberName, box,lhs);
        }
    }

    public class MemberReferanceResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {

        private readonly string memberName;
        private readonly IBox<IWeakReturnable> lhs;
        private readonly DelegateBox<WeakMemberDefinition> box;

        public MemberReferanceResolveReferance(string memberName, DelegateBox<WeakMemberDefinition> box, IBox<IWeakReturnable> lhs)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IOpenBoxes<WeakMemberReferance> Run(IResolveReferanceContext context)
        {
            box.Set(() =>
            {
                var lshtype = lhs.GetValue();
                if (lshtype is WeakMemberDefinition memberDefinitions)
                {
                    lshtype = memberDefinitions.Type.GetValue();
                }
                return lshtype.Cast<IScoped>().Scope.Members[new NameKey(memberName)].GetValue();
            });

            var itme = new WeakMemberReferance(box);
            return new MemberReferanceOpenBoxes(itme);
        }
        
    }

    internal class MemberReferanceOpenBoxes : IOpenBoxes< WeakMemberReferance>
    {
        public WeakMemberReferance CodeElement { get; }

        public MemberReferanceOpenBoxes(WeakMemberReferance itme)
        {
            this.CodeElement = itme ?? throw new ArgumentNullException(nameof(itme));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberReferance(CodeElement);
        }
    }
}