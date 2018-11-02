using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model
{
    // what even is the point of this interface??
    // I want an alis
    // but is it worth the price?
    public interface IFinalizedScope: IReadOnlyDictionary<IKey, IMemberDefinition>
    {
    }
}