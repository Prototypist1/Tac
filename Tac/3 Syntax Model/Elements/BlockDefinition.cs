using System;
using System.Collections.Generic;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    public class BlockDefinition : AbstractBlockDefinition
    {
        public BlockDefinition(ICodeElement[] body, IScope scope, IEnumerable<ICodeElement> staticInitailizers) : base(scope ?? throw new System.ArgumentNullException(nameof(scope)), body, staticInitailizers) { }

        public override IBox<ITypeDefinition> ReturnType(ScopeTree scopes)
        {
            return new ScopeStack(scopes,Scope).GetType(RootScope.EmptyType);
        }
    }

    public class BlockDefinitionMaker : IMaker<BlockDefinition>
    {
        public BlockDefinitionMaker(Func<ICodeElement[], IScope, IEnumerable<ICodeElement>, BlockDefinition> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<ICodeElement[], IScope, IEnumerable<ICodeElement>, BlockDefinition> Make { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<BlockDefinition> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
               .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
               .Has(ElementMatcher.IsDone)
               .IsMatch)
            {
                var scope = new LocalStaticScope();

                var innerMatchingContext = matchingContext.Child(scope);
                var elements = innerMatchingContext.ParseBlock(body);

                result = PopulateScope(scope, elements);

                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<BlockDefinition> PopulateScope(LocalStaticScope scope, Steps.PopulateScope<ICodeElement>[] elements) {
            return (tree) =>
            {
                return DetermineInferedTypes(scope, elements.Select(x=>x(tree)).ToArray());
            };
        }

        private Steps.DetermineInferedTypes<BlockDefinition> DetermineInferedTypes(LocalStaticScope scope, Steps.DetermineInferedTypes<ICodeElement>[] elements)
        {
            return () => ResolveReferance(scope, elements.Select(x => x()).ToArray());
        }

        private Steps.ResolveReferance<BlockDefinition> ResolveReferance(LocalStaticScope scope, Steps.ResolveReferance<ICodeElement>[] elements)
        {
            return (tree) =>
            {
                return Make(elements.Select(x => x(tree)).ToArray(), scope, new ICodeElement[0]);
            };
        }
    }
}