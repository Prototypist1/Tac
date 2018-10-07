using Prototypist.LeftToRight;
using System;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class PathPart : ICodeElement
    {
        public PathPart(PathBox memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public PathBox MemberDefinition { get; }

        // does return type ever depend on ScopeTree?
        // I mean we use the root scope pretty often
        // but do we juse use that?
        public IBox<ITypeDefinition> ReturnType(RootScope rootScope)
        {
            return rootScope.PathPartType(MemberDefinition.GetValue().Type.GetValue().Key);
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

        public PathPartPopulateScope( string item, Func<IBox<MemberDefinition>, PathPart> make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<PathPart> Run(IPopulateScopeContext context)
        {

            return new PathPartResolveReferance(memberName, make);
        }
    }

    public class PathPartResolveReferance : IResolveReference<PathPart>
    {
        private readonly string memberName;
        private readonly Func<IBox<MemberDefinition>, PathPart> make;

        public PathPartResolveReferance(string memberName, Func<IBox<MemberDefinition>, PathPart> make)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public PathPart Run(IResolveReferanceContext context)
        {
            // TODO I need to build out the builder for Path Operation
            // I could be nice if the IResolvableScope to look in was injected
            return make(new PathBox(new NameKey(memberName)).Follow());
        }

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            // TODO hard
            // we have a box with a member 
            // the box with the member has a box with the type
            // maybe it is time to break out the delegate box...
            // delegates are so uncontrollable...

            // I think delegate boxes are ok...
            // 
            return context.RootScope.PathPartType(??);
        }
    }
}