using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class ModuleDefinition : IScoped, ICodeElement, ITypeDefinition
    {
        public ModuleDefinition(IScope scope, IEnumerable<ICodeElement> staticInitialization)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
        }
        
        public IScope Scope { get; }
        public IEnumerable<ICodeElement> StaticInitialization { get; }

        public IBox<ITypeDefinition> ReturnType(ScopeTree scope)
        {
            return new Box<ITypeDefinition>(this);
        }
    }


    public class ModuleDefinitionMaker : IMaker<ModuleDefinition>
    {
        public ModuleDefinitionMaker(Func<IScope, IEnumerable<ICodeElement>, ModuleDefinition> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<IScope, IEnumerable<ICodeElement>, ModuleDefinition> Make { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<ModuleDefinition> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("module"), out var frist)
                            .Has(ElementMatcher.IsBody, out CurleyBacketToken third)
                            .Has(ElementMatcher.IsDone)
                            .IsMatch)
            {

                var scope = new StaticScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(third);

                result = PopulateScope(scope, elements);
                return true;

            }
            result = default;
            return false;
        }

        private Steps.PopulateScope<ModuleDefinition> PopulateScope(StaticScope scope, Steps.PopulateScope<ICodeElement>[] elements)
        {
            return (tree) =>
            {
                return DetermineInferedTypes(scope, elements.Select(x => x(tree)).ToArray());
            };
        }

        private Steps.DetermineInferedTypes<ModuleDefinition> DetermineInferedTypes(StaticScope scope, Steps.DetermineInferedTypes<ICodeElement>[] elements)
        {
            return () => ResolveReferance(scope, elements.Select(x => x()).ToArray());
        }

        private Steps.ResolveReferance<ModuleDefinition> ResolveReferance(StaticScope scope, Steps.ResolveReferance<ICodeElement>[] elements)
        {
            return (tree) =>
            {
                return Make(scope,elements.Select(x => x(tree)).ToArray());
            };
        }
    }
}
