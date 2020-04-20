using Prototypist.Toolbox;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class ConstantNumber : IConstantNumber, IConstantNumberBuilder
    {
        private readonly BuildableValue<double> valueBuilder = new BuildableValue<double>();

        private ConstantNumber(){}

        #region IConstantNumber

        public double Value { get => valueBuilder.Get(); }
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.ConstantNumber(this);
        }

        public IVerifiableType Returns()
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

        public static IConstantNumber CreateAndBuild(double value) {
            var (x, y) = Create();
            y.Build(value);
            return x;
        }
    }

    public interface IConstantNumberBuilder {
        void Build(double value);
    }

}
