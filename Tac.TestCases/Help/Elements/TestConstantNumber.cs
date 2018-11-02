namespace Tac.Model.Elements
{
    public class TestConstantNumber : IConstantNumber
    {
        public TestConstantNumber(double value)
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
            return new TestNumberType();
        }
    }
}
