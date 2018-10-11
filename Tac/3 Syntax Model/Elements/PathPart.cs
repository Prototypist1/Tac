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
        public PathPartMaker(Func<IBox<MemberDefinition>, PathPart> make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private Func<IBox<MemberDefinition>, PathPart> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<PathPart>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new PathPartPopulateScope(first.Item, Make));
            }

            return ResultExtension.Bad<IPopulateScope<PathPart>>();
        }
    }

    public class PathPartPopulateScope : IPopulateScope<PathPart>
    {
        private readonly string memberName;
        private readonly Func<IBox<MemberDefinition>, PathPart> make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public PathPartPopulateScope( string item, Func<IBox<MemberDefinition>, PathPart> make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<PathPart> Run(IPopulateScopeContext context)
        {

            return new PathPartResolveReferance(memberName, make, box);
        }
    }

    public class PathPartResolveReferance : IResolveReference<PathPart>
    {
        private readonly string memberName;
        private readonly Func<IBox<MemberDefinition>, PathPart> make;
        private readonly Box<IReturnable> box;

        public PathPartResolveReferance(string memberName, Func<IBox<MemberDefinition>, PathPart> make, Box<IReturnable> box)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public PathPart Run(IResolveReferanceContext context)
        {
            // TODO I need to build out the builder for Path Operation
            // I could be nice if the IResolvableScope to look in was injected
            return box.Fill(make(MakeBox(context)));
        }

        private DelegateBox<MemberDefinition> MakeBox(IResolveReferanceContext context)
        {
            return new DelegateBox<MemberDefinition>(() =>
            {
                // TODO

                // hmmm
                // maybe this should be injected??
                // we should only be using these inside a PathResolveReferance anyway...
                // it is hard to reach accross at the moment
                // because it would have to be passed in several steps earlier
                // in IOperationMaker<PathOperation>.TryMake
                // this is long before a BinaryResolveReferance<PathOperation> is created
                // maybe we could pass in a box??
                // I hate this API that reaches around the tree 
                // very dangerous!
                if (!context.TryGetParent<BinaryResolveReferance<PathOperation>>(out var parent))
                {
                    throw new Exception("parent should be a path resolve");
                }

                if (!context.TryGetParentContext(out var partentContext))
                {
                    throw new Exception("we should be able to get the parents context");
                }

                if (!parent.left.GetReturnType(partentContext).Cast<GenericTypeDefinition>().Scope.TryGetMember(new NameKey(memberName), false, out var member))
                {
                    throw new Exception("The left should have a member that matches");
                }

                return member.GetValue();
            });
        }
    }
}