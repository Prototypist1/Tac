using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IAbstractBlockDefinition: ICodeElement {
        IFinalizedScope Scope { get; }
        ICodeElement[] Body { get; }
        IEnumerable<ICodeElement> StaticInitailizers { get; }
    }
}