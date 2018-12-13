using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public abstract class AbstractBlockDefinition: IAbstractBlockDefinition
    {
        protected AbstractBlockDefinition(IFinalizedScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers)
        {
            Scope = scope;
            Body = body;
            StaticInitailizers = staticInitailizers;
        }

        public IFinalizedScope Scope { get; set; }
        public ICodeElement[] Body { get; set; }
        public IEnumerable<ICodeElement> StaticInitailizers { get; set; }

        public abstract T Convert<T>(IOpenBoxesContext<T> context);

        public IVarifiableType Returns()
        {
            return new EmptyType();
        }
    }
}