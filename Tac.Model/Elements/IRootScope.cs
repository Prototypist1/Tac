using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IRootScope : ICodeElement
    {

        IFinalizedScope Scope { get; }
        IEntryPointDefinition EntryPoint { get; }
        IReadOnlyList<IAssignOperation> Assignments { get; }
    }

    // do I need this? is it really a code element?
    // yeah I think it is
    // it's just an IFinalizedScope...
    // but maybe I want to identify that this scope is different
    public interface IDependencyScope : ICodeElement
    {
        IFinalizedScope Scope { get; }
    }
}
