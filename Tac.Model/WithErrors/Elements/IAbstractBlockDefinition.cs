using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.WithErrors.Elements
{
    public interface IAbstractBlockDefinition: ICodeElement {
        IFinalizedScope Scope { get; }
        IReadOnlyList<IOrType<ICodeElement, IError>> Body { get; }
        IReadOnlyList<ICodeElement> StaticInitailizers { get; }
    }
}