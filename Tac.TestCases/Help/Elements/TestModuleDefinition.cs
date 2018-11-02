using System;
using System.Collections.Generic;

namespace Tac.Model.Elements
{
    public class TestModuleDefinition : IModuleDefinition
    {
        public TestModuleDefinition(IFinalizedScope scope, IEnumerable<ICodeElement> staticInitialization)
        {
            Scope = scope;
            StaticInitialization = staticInitialization;
        }

        public IFinalizedScope Scope { get; set; }
        public IEnumerable<ICodeElement> StaticInitialization { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ModuleDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }
    }
}
