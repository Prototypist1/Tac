using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IFinalizedScope
    {
        IReadOnlyDictionary<IKey, IMemberDefinition> Members { get; }
    }
}