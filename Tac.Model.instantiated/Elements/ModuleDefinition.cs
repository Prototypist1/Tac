using System;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class ModuleDefinition : IModuleDefinition
    {
        public ModuleDefinition(IFinalizedScope scope, IEnumerable<ICodeElement> staticInitialization)
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
