using Prototypist.Toolbox;
using System.Collections.Generic;

namespace Tac.Model.Elements
{
    public interface IModuleDefinition : ICodeElement
    {
        IKey Key { get; }
        IFinalizedScope Scope { get; }
        IReadOnlyList<IOrType<ICodeElement, IError>> StaticInitialization { get; }

        IEntryPointDefinition EntryPoint { get; }
    }

    public interface IProject<TBacking> 
        where TBacking : IBacking
    {
        IModuleDefinition ModuleDefinition { get; }
        IReadOnlyList<IAssembly<TBacking>> References { get; }
    }
}
