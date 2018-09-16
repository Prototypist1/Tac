using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class ModuleDefinition : IScoped, ICodeElement
    {
        public ModuleDefinition(IScope scope, IEnumerable<ICodeElement> staticInitialization)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }


        public IScope Scope { get; }

        public ITypeDefinition ReturnType(ScopeStack scope)
        {
            throw new NotImplementedException();
        }
    }
}
