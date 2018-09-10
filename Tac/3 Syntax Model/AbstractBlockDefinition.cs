using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public abstract class AbstractBlockDefinition : ICodeElement, IScoped
    {
        protected AbstractBlockDefinition(IScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers) {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }
        
        public IScope Scope { get; }
        public ICodeElement[] Body { get; }
        public IEnumerable<ICodeElement> StaticInitailizers { get; }

        public override bool Equals(object obj)
        {
            return obj is AbstractBlockDefinition definition &&
                   EqualityComparer<IScope>.Default.Equals(Scope, definition.Scope) &&
                   Body.SequenceEqual(definition.Body) &&
                   StaticInitailizers.SequenceEqual(definition.StaticInitailizers);
        }

        public override int GetHashCode()
        {
            var hashCode = 273578712;
            hashCode = hashCode * -1521134295 + Scope.GetHashCode();
            hashCode = hashCode * -1521134295 + Body.Sum(x => x.GetHashCode());
            hashCode = hashCode * -1521134295 + StaticInitailizers.Sum(x => x.GetHashCode());
            return hashCode;
        }
        public abstract ITypeDefinition ReturnType(ScopeStack scope);
    }
}