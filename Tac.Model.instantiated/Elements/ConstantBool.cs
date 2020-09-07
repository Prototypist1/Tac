using Tac.Model.Elements;
using Prototypist.Toolbox;
using System.Collections.Generic;

namespace Tac.Model.Instantiated
{
    public class ConstantBool : IConstantBool, IConstantBoolBuilder
    {
        private readonly BuildableValue<bool> valueBuilder = new BuildableValue<bool>();

        private ConstantBool() { }

        #region IConstantBool

        public bool Value { get => valueBuilder.Get(); }


        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.ConstantBool(this);
        }

        public IVerifiableType Returns()
        {
            return new BooleanType();
        }

        #endregion


        public void Build(bool value)
        {
            valueBuilder.Set(value);
        }

        public static (IConstantBool, IConstantBoolBuilder) Create()
        {
            var res = new ConstantBool();
            return (res, res);
        }

        public static IConstantBool CreateAndBuild(bool value)
        {
            var (x, y) = Create();
            y.Build(value);
            return x;
        }
    }

    public interface IConstantBoolBuilder
    {
        void Build(bool value);
    }

}
