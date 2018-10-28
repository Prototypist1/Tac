using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface IFinalizedScope: IReadOnlyDictionary<IKey, IMemberDefinition>
    {
    }
}