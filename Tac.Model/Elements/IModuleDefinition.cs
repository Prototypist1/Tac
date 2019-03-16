using System.Collections.Generic;

namespace Tac.Model.Elements
{
    public interface IModuleDefinition : ICodeElement, IVerifiableType

    {
        IKey Key { get; }
        IFinalizedScope Scope { get; }
        IReadOnlyList<ICodeElement> StaticInitialization { get; }
    }

    public interface IModuleAsAssembly<TBacking> : IModuleDefinition
        where TBacking : IBacking
    {

        IReadOnlyList<IAssembly<TBacking>> References { get; }
    }
}
