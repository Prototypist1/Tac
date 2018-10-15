using Prototypist.LeftToRight;
using System;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public class PathPart : ICodeElement, IReturnable
    {
        public delegate PathPart Make(IBox<MemberDefinition> memberDefinition);

        public PathPart(IBox<MemberDefinition> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }
        
        public IBox<MemberDefinition> MemberDefinition { get; }
        
        public IReturnable ReturnType(IElementBuilders elementBuilders)
        {
            return this;
        }
    }

    public class PathPartMaker : IMaker<PathPart>
    {
        public PathPartMaker(PathPart.Make make,
            IElementBuilders elementBuilders, 
            IBox<IReturnable> lhs)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        private readonly IBox<IReturnable> lhs;
        private PathPart.Make Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<PathPart>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new PathPartPopulateScope(first.Item, Make, lhs));
            }

            return ResultExtension.Bad<IPopulateScope<PathPart>>();
        }
    }

    public class PathPartPopulateScope : IPopulateScope<PathPart>
    {

        private readonly IBox<IReturnable> lhs;
        private readonly string memberName;
        private readonly PathPart.Make make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public PathPartPopulateScope( string item, PathPart.Make make,IBox<IReturnable> lhs)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<PathPart> Run(IPopulateScopeContext context)
        {

            return new PathPartResolveReferance(memberName, make, box,lhs);
        }
    }

    public class PathPartResolveReferance : IResolveReference<PathPart>
    {

        private readonly string memberName;
        private readonly IBox<IReturnable> lhs;
        private readonly PathPart.Make make;
        private readonly Box<IReturnable> box;

        public PathPartResolveReferance(string memberName, PathPart.Make make, Box<IReturnable> box, IBox<IReturnable> lhs)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public PathPart Run(IResolveReferanceContext context)
        {
            return box.Fill(make(new DelegateBox<MemberDefinition>().Set(() =>
            {
                var lshtype = lhs.GetValue();
                if (lshtype is Member member) {
                    lshtype = member.MemberDefinition.GetValue().Type.GetValue();
                }
                return lshtype.Cast<IScoped>().Scope.GetMemberOrThrow(new NameKey(memberName), false).GetValue();
            })));
        }
        
    }
}