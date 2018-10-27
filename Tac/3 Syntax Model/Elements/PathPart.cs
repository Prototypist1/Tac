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

    public class MemberReferanceMaker<T> : IMaker<T, WeakMemberReferance>
    {
        public MemberReferanceMaker(Func<WeakMemberReferance,T> make,
            IElementBuilders elementBuilders, 
            IBox<IWeakReturnable> lhs)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        private readonly IBox<IWeakReturnable> lhs;
        private Func<WeakMemberReferance,T> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<T, WeakMemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberReferancePopulateScope<T>(first.Item, Make, lhs));
            }

            return ResultExtension.Bad<IPopulateScope<T, WeakMemberReferance>>();
        }
    }

    public class MemberReferancePopulateScope<T> : IPopulateScope<T, WeakMemberReferance>
    {

        private readonly IBox<IWeakReturnable> lhs;
        private readonly string memberName;
        private readonly Func<WeakMemberReferance,T> make;
        private readonly DelegateBox<WeakMemberDefinition> box = new DelegateBox<WeakMemberDefinition>();

        public MemberReferancePopulateScope( string item, Func<WeakMemberReferance,T> make,IBox<IWeakReturnable> lhs)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<T, WeakMemberReferance> Run(IPopulateScopeContext context)
        {

            return new MemberReferanceResolveReferance<T>(memberName, make, box,lhs);
        }
    }

    public class MemberReferanceResolveReferance<T> : IPopulateBoxes<T, WeakMemberReferance>
    {

        private readonly string memberName;
        private readonly IBox<IWeakReturnable> lhs;
        private readonly Func<WeakMemberReferance,T> make;
        private readonly DelegateBox<WeakMemberDefinition> box;

        public MemberReferanceResolveReferance(string memberName, Func<WeakMemberReferance,T> make, DelegateBox<WeakMemberDefinition> box, IBox<IWeakReturnable> lhs)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IOpenBoxes<T, WeakMemberReferance> Run(IResolveReferanceContext context)
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
            return new MemberReferanceOpenBoxes<T>(itme, make);
        }
        
    }

    internal class MemberReferanceOpenBoxes<T> : IOpenBoxes<T, WeakMemberReferance>
    {
        public WeakMemberReferance CodeElement { get; }
        private readonly Func<WeakMemberReferance, T> make;

        public MemberReferanceOpenBoxes(WeakMemberReferance itme, Func<WeakMemberReferance, T> make)
        {
            this.CodeElement = itme ?? throw new ArgumentNullException(nameof(itme));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public T Run(IOpenBoxesContext context)
        {
            return make(CodeElement);
        }
    }
}