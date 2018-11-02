using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public class TestMethodDefinition : IMethodDefinition
    {

        public IVarifiableType InputType { get; }
        public IVarifiableType OutputType { get; }
        public IMemberDefinition ParameterDefinition { get; }
    }
}