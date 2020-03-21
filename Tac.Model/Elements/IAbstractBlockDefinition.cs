using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IAbstractBlockDefinition: ICodeElement {
        IFinalizedScope Scope { get; }
        IReadOnlyList<OrType<ICodeElement, IError>> Body { get; }
        IReadOnlyList<ICodeElement> StaticInitailizers { get; }
    }
}