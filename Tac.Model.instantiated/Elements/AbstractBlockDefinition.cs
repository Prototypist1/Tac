using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{

    // I don't think I get to use base classes :/
    //public abstract class AbstractBlockDefinition: IAbstractBlockDefinition, IAbstractBlockDefinitionBuilder
    //{
    //    protected (AbstractBlockDefinition, IAbstractBlockDefinitionBuilder) Build(IFinalizedScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers)
    //    {
    //        Scope = scope;
    //        Body = body;
    //        StaticInitailizers = staticInitailizers;
    //    }

    //    public abstract T Convert<T>(IOpenBoxesContext<T> context);


    //}


    internal class Buildable<T>
        where T : class
    {
        private IIsPossibly<T> t = Possibly.IsNot<T>();
        public T Get()
        {
            if (t is IIsDefinately<T> definate)
            {
                return definate.Value;
            }
            throw new ApplicationException();
        }
        public void Set(T t)
        {
            if (t is IIsPossibly<T> && !(t is IIsDefinately<T>))
            {
                throw new ApplicationException();
            }
            this.t = Possibly.Is(t);
        }
    }

    internal class BuildableValue<T>
        where T : struct
    {
        private bool set = false;
        private T t;
        public T Get()
        {
            if (!set)
            {
                throw new ApplicationException();
            }
            return t;
        }
        public void Set(T t)
        {
            if (set)
            {
                throw new ApplicationException();
            }
            this.t = t;
            set = true;
        }
    }

}