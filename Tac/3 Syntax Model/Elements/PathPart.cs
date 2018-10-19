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
    public class MemberReferance : ICodeElement, IReturnable
    {
        public delegate MemberReferance Make(IBox<MemberDefinition> memberDefinition);

        public MemberReferance(IBox<MemberDefinition> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public IBox<MemberDefinition> MemberDefinition { get; }

        public IReturnable Returns(IElementBuilders elementBuilders)
        {
            return MemberDefinition.GetValue();
        }
    }

    public class MemberReferanceMaker : IMaker<MemberReferance>
    {
        public MemberReferanceMaker(MemberReferance.Make make,
            IElementBuilders elementBuilders, 
            IBox<IReturnable> lhs)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        private readonly IBox<IReturnable> lhs;
        private MemberReferance.Make Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<MemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberReferancePopulateScope(first.Item, Make, lhs));
            }

            return ResultExtension.Bad<IPopulateScope<MemberReferance>>();
        }
    }

    public class MemberReferancePopulateScope : IPopulateScope<MemberReferance>
    {

        private readonly IBox<IReturnable> lhs;
        private readonly string memberName;
        private readonly MemberReferance.Make make;
        private readonly DelegateBox<MemberDefinition> box = new DelegateBox<MemberDefinition>();

        public MemberReferancePopulateScope( string item, MemberReferance.Make make,IBox<IReturnable> lhs)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<MemberReferance> Run(IPopulateScopeContext context)
        {

            return new MemberReferanceResolveReferance(memberName, make, box,lhs);
        }
    }

    public class MemberReferanceResolveReferance : IResolveReference<MemberReferance>
    {

        private readonly string memberName;
        private readonly IBox<IReturnable> lhs;
        private readonly MemberReferance.Make make;
        private readonly DelegateBox<MemberDefinition> box;

        public MemberReferanceResolveReferance(string memberName, MemberReferance.Make make, DelegateBox<MemberDefinition> box, IBox<IReturnable> lhs)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public MemberReferance Run(IResolveReferanceContext context)
        {
            box.Set(() =>
            {
                var lshtype = lhs.GetValue();
                if (lshtype is MemberDefinition memberDefinitions)
                {
                    lshtype = memberDefinitions.Type.GetValue();
                }
                return lshtype.Cast<IScoped>().Scope.GetMemberOrThrow(new NameKey(memberName), false).GetValue();
            });

            return make(box);
        }
        
    }
}