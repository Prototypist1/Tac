using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class ConstantNumber : IConstantNumber, IConstantNumberBuilder
    {
        private readonly BuildableValue<double> valueBuilder = new BuildableValue<double>();

        private ConstantNumber(){}

        #region IConstantNumber

        public double Value { get => valueBuilder.Get() }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ConstantNumber(this);
        }

        public IVarifiableType Returns()
        {
            return new NumberType();
        }

        #endregion


        public void Build(double value)
        {
            valueBuilder.Set(value);
        }
        
        public static (IConstantNumber, IConstantNumberBuilder) Create()
        {
            var res = new ConstantNumber();
            return (res, res);
        }
    }

    public interface IConstantNumberBuilder {
        void Build(double value);
    }

}
