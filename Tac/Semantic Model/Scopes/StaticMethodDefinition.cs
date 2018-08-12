using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class StaticMethodDefinition : MethodDefinition, IReferanced
    {
        public StaticMethodDefinition(TypeReferance outputType, ParameterDefinition parameterDefinition, CodeElement[] body, AbstractName key) : base(outputType, parameterDefinition, body)
        {
            Key = key;
        }

        public AbstractName Key { get; }
    }
}
