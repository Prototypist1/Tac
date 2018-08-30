using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScope
    {
        bool TryGet(IEnumerable<AbstractName> names, out IReferanced item);
        bool TryGet(ImplicitTypeReferance key, out ITypeDefinition item);
    }
}