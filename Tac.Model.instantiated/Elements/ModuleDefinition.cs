using System;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class ModuleDefinition : IModuleDefinition, IModuleDefinitionBuilder
    {
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<IEnumerable<ICodeElement>> buildableStaticInitialization = new Buildable<IEnumerable<ICodeElement>>();

        private ModuleDefinition() { }

        public IFinalizedScope Scope => buildableScope.Get();
        public IEnumerable<ICodeElement> StaticInitialization => buildableStaticInitialization.Get();

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ModuleDefinition(this);
        }

        public IVerifiableType Returns()
        {
            return this;
        }

        public void Build(IFinalizedScope scope, IEnumerable<ICodeElement> staticInitialization)
        {
            buildableScope.Set(scope);
            buildableStaticInitialization.Set(staticInitialization);
        }
        
        public static (IModuleDefinition, IModuleDefinitionBuilder) Create()
        {
            var res = new ModuleDefinition();
            return (res, res);
        }

        public static IModuleDefinition CreateAndBuild(IFinalizedScope scope, IEnumerable<ICodeElement> staticInitialization) {
            var (x, y) = Create();
            y.Build(scope, staticInitialization);
            return x;
        }
    }

    public interface IModuleDefinitionBuilder
    {
        void Build(IFinalizedScope scope, IEnumerable<ICodeElement> staticInitialization);
    }
}
