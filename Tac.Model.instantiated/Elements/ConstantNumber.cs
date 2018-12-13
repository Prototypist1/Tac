using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class ConstantNumber : IConstantNumber
    {
        public ConstantNumber(double value)
        {
            Value = value;
        }

        public double Value { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ConstantNumber(this);
        }

        public IVarifiableType Returns()
        {
            return new NumberType();
        }
    }
}
