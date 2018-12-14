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
        where T :class
    {
        private T t;
        public T Get()  =>t ;
        public void Set(T t)
        {
            if (t != null)
            {
                throw new ApplicationException();
            }
            this.t = t ?? throw new ArgumentNullException();
        }
    }

}