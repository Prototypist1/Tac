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
        public IBox<ITypeDefinition> ReturnType(IScope scope)
        {
            return scope.GetTypeOrThrow(new GenericNameKey(RootScope.MemberType, MemberDefinition.Key));
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
                return ResultExtension.Good(new PathPartPopulateScope(matchingContext.ScopeStack.TopScope, first.Item, Make));
            }

            return ResultExtension.Bad<IPopulateScope<PathPart>>();
        }
    }

    public class PathPartPopulateScope : IPopulateScope<PathPart>
    {
        private readonly IScope scope;
        private readonly string memberName;
        private readonly Func<IBox<MemberDefinition>, PathPart> make;

        public PathPartPopulateScope(IScope topScope, string item, Func<IBox<MemberDefinition>, PathPart> make)
        {
            scope = topScope ?? throw new ArgumentNullException(nameof(topScope));
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<PathPart> Run(IPopulateScopeContext context)
        {
            if (!scope.TryGetMember(new NameKey(memberName), false, out var memberDef))
            {
                throw new Exception("That is not right!");
            }

            return new PathPartResolveReferance(memberDef, make);
        }
    }

    public class PathPartResolveReferance : IResolveReference<PathPart>
    {
        private readonly IBox<MemberDefinition> memberDef;
        private readonly Func<IBox<MemberDefinition>, PathPart> make;

        public PathPartResolveReferance(IBox<MemberDefinition> memberDef, Func<IBox<MemberDefinition>, PathPart> make)
        {
            this.memberDef = memberDef ?? throw new ArgumentNullException(nameof(memberDef));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public PathPart Run(IResolveReferanceContext context)
        {
            return make(memberDef);
        }

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return context.Tree.Root.GetTypeOrThrow(RootScope.PathPartType.Key);
        }
    }
}