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

    public interface IProject<TBacking> 
    {
        IRootScope RootScope { get; }
        IReadOnlyList<IAssembly<TBacking>> References { get; }
    }
}
