using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class AssignmentScope : IScope
    {

        public bool TryGet(IEnumerable<AbstractName> names, out IReferanced item)
        {
        }

        public bool TryGet(ImplicitTypeReferance key, out ITypeDefinition item)
        {
        }
    }

}