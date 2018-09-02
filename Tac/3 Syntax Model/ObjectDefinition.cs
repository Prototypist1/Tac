using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public sealed class ObjectDefinition: IScoped<ObjectScope>, ICodeElement, ITypeDefinition
    {
        public ObjectDefinition(ObjectScope scope, IEnumerable<ICodeElement> codeElements) {
            Scope = scope;
            CodeElements = codeElements.ToArray();
        }

        public ObjectScope Scope { get; }
        public ICodeElement[] CodeElements { get; }

        public override bool Equals(object obj)
        {
            return obj is ObjectDefinition definition &&
                   EqualityComparer<ObjectScope>.Default.Equals(Scope, definition.Scope);
        }

        public override int GetHashCode()
        {
            var hashCode = 1953067843;
            hashCode = hashCode * -1521134295 + EqualityComparer<ObjectScope>.Default.GetHashCode(Scope);
            return hashCode;
        }

        public ITypeDefinition<IScope> ReturnType(ScopeStack scope) {
            return this;
        }
    }
}
