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
        public ObjectDefinition(ObjectScope scope, IEnumerable<AssignOperation> assigns) {
            Scope = scope;
            Assignments = assigns.ToArray();
        }

        public IScope Scope { get; }
        public AssignOperation[] Assignments { get; }
        
        public ITypeDefinition ReturnType(ScopeStack scope) {
            return this;
        }
    }
}
