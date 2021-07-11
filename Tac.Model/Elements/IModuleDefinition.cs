using Prototypist.Toolbox;
using System.Collections.Generic;

namespace Tac.Model.Elements
{
    public interface IModuleDefinition : ICodeElement
    {
        IKey Key { get; }
        IFinalizedScope Scope { get; }
        IReadOnlyList<ICodeElement> StaticInitialization { get; }

        IEntryPointDefinition EntryPoint { get; }
    }

    public interface IProject<out TAssembly, out TBacking>
        where TAssembly : IAssembly<TBacking>
    {
        IRootScope RootScope { get; }
        IFinalizedScope DependencyScope { get; }
        IReadOnlyList<TAssembly> References { get; }
    }
}
