using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class ConstantNumber : ICodeElement
    {
        public ConstantNumber(double value) 
        {
            Value = value;
        }

        public double Value { get; }

        public ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.NumberType);
        }
    }
}
