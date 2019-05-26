using System;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class ModuleDefinition : IModuleDefinition, IModuleDefinitionBuilder
    {
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<IReadOnlyList<ICodeElement>> buildableStaticInitialization = new Buildable<IReadOnlyList<ICodeElement>>();
        private readonly Buildable<IKey> buildableKey = new Buildable<IKey>();

        private ModuleDefinition() { }

        public IFinalizedScope Scope => buildableScope.Get();
        public IReadOnlyList<ICodeElement> StaticInitialization => buildableStaticInitialization.Get();
        public IKey Key => buildableKey.Get();

        public T Convert<T,TBaking>(IOpenBoxesContext<T, TBaking> context)
            where TBaking: IBacking
        {
            return context.ModuleDefinition(this);
        }

        public void Build(IFinalizedScope scope, IReadOnlyList<ICodeElement> staticInitialization, IKey key)
        {
            buildableScope.Set(scope);
            buildableStaticInitialization.Set(staticInitialization);
            buildableKey.Set(key);
        }
        
        public static (IModuleDefinition, IModuleDefinitionBuilder) Create()
        {
            var res = new ModuleDefinition();
            return (res, res);
        }

        public static IModuleDefinition CreateAndBuild(IFinalizedScope scope, IReadOnlyList<ICodeElement> staticInitialization, IKey key) {
            var (x, y) = Create();
            y.Build(scope, staticInitialization, key);
            return x;
        }

        public IVerifiableType Returns() {

        }
    }

    public interface IModuleDefinitionBuilder
    {
        void Build(IFinalizedScope scope, IReadOnlyList<ICodeElement> staticInitialization, IKey key);
    }
}
