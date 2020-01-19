using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class ModuleDefinition : IModuleDefinition, IModuleDefinitionBuilder
    {
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<IReadOnlyList<ICodeElement>> buildableStaticInitialization = new Buildable<IReadOnlyList<ICodeElement>>();
        private readonly Buildable<IKey> buildableKey = new Buildable<IKey>();
        private readonly Buildable<IEntryPointDefinition> buildableEntryPoint = new Buildable<IEntryPointDefinition>();

        private ModuleDefinition() { }

        public IFinalizedScope Scope => buildableScope.Get();
        public IReadOnlyList<ICodeElement> StaticInitialization => buildableStaticInitialization.Get();
        public IKey Key => buildableKey.Get();
        public IEntryPointDefinition EntryPoint => buildableEntryPoint.Get();

        public T Convert<T,TBaking>(IOpenBoxesContext<T, TBaking> context)
            where TBaking: IBacking
        {
            return context.ModuleDefinition(this);
        }

        public void Build(IFinalizedScope scope, IReadOnlyList<ICodeElement> staticInitialization, IKey key, IEntryPointDefinition entryPoint)
        {
            buildableScope.Set(scope);
            buildableStaticInitialization.Set(staticInitialization);
            buildableKey.Set(key);
            buildableEntryPoint.Set(entryPoint);
        }
        
        public static (IModuleDefinition, IModuleDefinitionBuilder) Create()
        {
            var res = new ModuleDefinition();
            return (res, res);
        }

        public static IModuleDefinition CreateAndBuild(IFinalizedScope scope, IReadOnlyList<ICodeElement> staticInitialization, IKey key, IEntryPointDefinition entryPoint) {
            var (x, y) = Create();
            y.Build(scope, staticInitialization, key, entryPoint);
            return x;
        }

        public IVerifiableType Returns()
        {
            return InterfaceType.CreateAndBuild(Scope.Members.Values.Select(x=>x.Value).ToList());
        }
    }

    public interface IModuleDefinitionBuilder
    {
        void Build(IFinalizedScope scope, IReadOnlyList<ICodeElement> staticInitialization, IKey key, IEntryPointDefinition entryPoint);
    }
}
