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

        public bool ContainsInTree(ICodeElement element) => Equals(element);

        public override bool Equals(object obj)
        {
            return obj is ConstantNumber number &&
                   Value == number.Value;
        }

        public override int GetHashCode() => -1937169414 + Value.GetHashCode();
    }
}
