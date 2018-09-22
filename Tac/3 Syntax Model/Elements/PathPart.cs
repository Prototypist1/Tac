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

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<PathPart> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                result = PopulateScope(matchingContext.ScopeStack.TopScope, first.Item);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<PathPart> PopulateScope(IScope scope, string memberName)
        {
            return (tree) =>
            {
                IBox<MemberDefinition> memberDef;
                if (!scope.TryGetMember(new NameKey(memberName), false, out memberDef))
                {
                    throw new Exception("That is not right!");
                }

                return DetermineInferedTypes(scope, memberDef);
            };
        }

        private Steps.DetermineInferedTypes<PathPart> DetermineInferedTypes(IScope scope,  IBox<MemberDefinition> box)
        {
            return () =>
            {
                return ResolveReferance(scope, box);
            };
        }

        private Steps.ResolveReferance<PathPart> ResolveReferance(IScope scope,  IBox<MemberDefinition> box)
        {
            return (tree) =>
            {
                return Make(box);
            };
        }
    }
}