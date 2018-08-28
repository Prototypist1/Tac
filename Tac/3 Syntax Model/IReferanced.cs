using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IReferanced: ICodeElement
    {
        AbstractName Key { get; }
    }

}