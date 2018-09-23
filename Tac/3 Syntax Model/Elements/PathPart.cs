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
        public PathPart(IBox<MemberDefinition> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }
        
        public IBox<MemberDefinition> MemberDefinition { get; }

        public IBox<ITypeDefinition> ReturnType(ScopeStack scope)
        {
            return MemberDefinition.ReturnType(scope);
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

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<PathPart> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                result = new PathPartPopulateScope(matchingContext.ScopeStack.TopScope, first.Item, Make);
                return true;
            }

            result = default;
            return false;
        }
        
        private class PathPartPopulateScope : IPopulateScope<PathPart>
        {
            private readonly IScope scope;
            private readonly string memberName;
            private readonly Func<IBox<MemberDefinition>, PathPart> make;

            public PathPartPopulateScope(IScope topScope, string item, Func<IBox<MemberDefinition>, PathPart> make)
            {
                this.scope = topScope ?? throw new ArgumentNullException(nameof(topScope));
                this.memberName = item ?? throw new ArgumentNullException(nameof(item));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IResolveReferance<PathPart> Run(ScopeTree tree)
            {
                IBox<MemberDefinition> memberDef;
                if (!scope.TryGetMember(new NameKey(memberName), false, out memberDef))
                {
                    throw new Exception("That is not right!");
                }

                return new PathPartResolveReferance( memberDef,make);
            }
        }

        private class PathPartResolveReferance : IResolveReferance<PathPart>
        {
            private readonly IBox<MemberDefinition> memberDef;
            private readonly Func<IBox<MemberDefinition>, PathPart> make;

            public PathPartResolveReferance(IBox<MemberDefinition> memberDef, Func<IBox<MemberDefinition>, PathPart> make)
            {
                this.memberDef = memberDef ?? throw new ArgumentNullException(nameof(memberDef));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public PathPart Run(ScopeTree tree)
            {
                return make(memberDef);
            }
        }
    }
}