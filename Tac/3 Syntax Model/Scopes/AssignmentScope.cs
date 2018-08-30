using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class AssignmentScope : IScope
    {
        public AssignmentScope(IScope enclosingScope)
        {
            EnclosingScope = enclosingScope;
        }

        public IScope EnclosingScope { get; }

        public bool TryGet(IEnumerable<AbstractName> names, out IReferanced item)
        {
            

            return EnclosingScope.TryGet(names, out item);
        }

        public bool TryGet(ImplicitTypeReferance key, out ITypeDefinition item)
        {

            return EnclosingScope.TryGet(key, out item);
        }
    }

}