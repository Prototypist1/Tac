using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface IConstantNumber: ICodeElement {
        double Value { get; }
    }
}
