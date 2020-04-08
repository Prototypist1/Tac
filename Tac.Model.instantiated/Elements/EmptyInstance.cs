using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated.Elements
{
    public class EmptyInstance : IEmptyInstance, IEmptyInstanceBuilder
    {
        private EmptyInstance() { }

        #region IEmptyInstance

        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.EmptyInstance(this);
        }

        public IOrType<IVerifiableType, IError> Returns()
        {
            return OrType.Make<IVerifiableType, IError>(new EmptyType());
        }

        #endregion


        public void Build()
        {
        }

        public static (IEmptyInstance, IEmptyInstanceBuilder) Create()
        {
            var res = new EmptyInstance();
            return (res, res);
        }

        public static IEmptyInstance CreateAndBuild()
        {
            var (x, y) = Create();
            y.Build();
            return x;
        }
    }

    public interface IEmptyInstanceBuilder
    {
        void Build();
    }

}
