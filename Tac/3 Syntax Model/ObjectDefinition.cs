using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public class ObjectDefinition: ITypeDefinition, ICodeElement
    {
        public ObjectDefinition(ObjectScope scope, IEnumerable<ICodeElement> codeElements) {
            Scope = scope;
            CodeElements = codeElements.ToArray();
        }

        public IScope Scope { get; }
        public ICodeElement[] CodeElements { get; }
        
        public ITypeDefinition ReturnType(ScopeStack scope) {
            return this;
        }
    }
}
