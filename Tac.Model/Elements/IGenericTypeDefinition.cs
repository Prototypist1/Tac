using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IGenericTypeDefinition : ICodeElement
    {
        IFinalizedScope Scope { get; }
        IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    }

    public interface IGenericTypeParameterDefinition {
        IKey Key { get; }
    }

}
