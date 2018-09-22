using Prototypist.LeftToRight;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public class ObjectDefinition: ITypeDefinition, ICodeElement
    {
        public ObjectDefinition(ObjectScope scope, IEnumerable<AssignOperation> assigns) {
            Scope = scope;
            Assignments = assigns.ToArray();
        }

        public IScope Scope { get; }
        public AssignOperation[] Assignments { get; }
        
        public IBox<ITypeDefinition> ReturnType(ScopeTree scope) {
            return new Box<ITypeDefinition>(this);
        }
    }
    
    public class ObjectDefinitionMaker : IMaker<ObjectDefinition>
    {
        public ObjectDefinitionMaker(Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> Make { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<ObjectDefinition> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                           .Has(ElementMatcher.KeyWord("object"), out var keyword)
                           .Has(ElementMatcher.IsBody, out CurleyBacketToken block)
                           .Has(ElementMatcher.IsDone)
                           .IsMatch)
            {
                var scope = new ObjectScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(block);

                result = PopulateScope(scope, elements);

                return true;
            }
            result = default;
            return false;
        }

        private Steps.PopulateScope<ObjectDefinition> PopulateScope(StaticScope scope, Steps.PopulateScope<ICodeElement>[] elements)
        {
            return (tree) =>
            {
                return DetermineInferedTypes(scope, elements.Select(x => x(tree)).ToArray());
            };
        }

        private Steps.DetermineInferedTypes<ObjectDefinition> DetermineInferedTypes(StaticScope scope, Steps.DetermineInferedTypes<ICodeElement>[] elements)
        {
            return () => ResolveReferance(scope, elements.Select(x => x()).ToArray());
        }

        private Steps.ResolveReferance<ObjectDefinition> ResolveReferance(StaticScope scope, Steps.ResolveReferance<ICodeElement>[] elements)
        {
            return (tree) =>
            {
                return Make(scope, elements.Select(x => x(tree).Cast<AssignOperation>()).ToArray());
            };
        }
    }
}
