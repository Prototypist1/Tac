﻿using Prototypist.Toolbox;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class ConstantString : IConstantString, IConstantStringBuilder
    {
        private readonly Buildable<string> valueBuilder = new Buildable<string>();

        private ConstantString() { }

        #region IConstantString

        public string Value { get => valueBuilder.Get(); }
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.ConstantString(this);
        }

        public OrType<IVerifiableType, IError> Returns()
        {
            return new OrType<IVerifiableType, IError>(new StringType());
        }

        #endregion


        public void Build(string value)
        {
            valueBuilder.Set(value);
        }

        public static (IConstantString, IConstantStringBuilder) Create()
        {
            var res = new ConstantString();
            return (res, res);
        }

        public static IConstantString CreateAndBuild(string value)
        {
            var (x, y) = Create();
            y.Build(value);
            return x;
        }
    }

    public interface IConstantStringBuilder
    {
        void Build(string value);
    }

}
