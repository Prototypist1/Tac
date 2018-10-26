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
        public delegate WeakMemberReferance Make(IBox<WeakMemberDefinition> memberDefinition);

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
        public MemberReferanceMaker(WeakMemberReferance.Make make,
            IElementBuilders elementBuilders, 
            IBox<IWeakReturnable> lhs)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        private readonly IBox<IWeakReturnable> lhs;
        private WeakMemberReferance.Make Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<WeakMemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberReferancePopulateScope(first.Item, Make, lhs));
            }

            return ResultExtension.Bad<IPopulateScope<WeakMemberReferance>>();
        }
    }

    public class MemberReferancePopulateScope : IPopulateScope<WeakMemberReferance>
    {

        private readonly IBox<IWeakReturnable> lhs;
        private readonly string memberName;
        private readonly WeakMemberReferance.Make make;
        private readonly DelegateBox<WeakMemberDefinition> box = new DelegateBox<WeakMemberDefinition>();

        public MemberReferancePopulateScope( string item, WeakMemberReferance.Make make,IBox<IWeakReturnable> lhs)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {

            return new MemberReferanceResolveReferance(memberName, make, box,lhs);
        }
    }

    public class MemberReferanceResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {

        private readonly string memberName;
        private readonly IBox<IWeakReturnable> lhs;
        private readonly WeakMemberReferance.Make make;
        private readonly DelegateBox<WeakMemberDefinition> box;

        public MemberReferanceResolveReferance(string memberName, WeakMemberReferance.Make make, DelegateBox<WeakMemberDefinition> box, IBox<IWeakReturnable> lhs)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public WeakMemberReferance Run(IResolveReferanceContext context)
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

            return make(box);
        }
        
    }
}