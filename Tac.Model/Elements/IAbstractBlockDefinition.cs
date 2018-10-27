using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public interface IAbstractBlockDefinition: ICodeElement {
        IFinalizedScope Scope { get; }
        ICodeElement[] Body { get; }
        IEnumerable<IAssignOperation> StaticInitailizers { get; }
    }
}