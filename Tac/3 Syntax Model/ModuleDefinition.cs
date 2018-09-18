using System;
using System.Collections.Generic;
using System.Text;
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

        public ITypeDefinition ReturnType(ScopeStack scope)
        {
            return this;
        }
    }
}
