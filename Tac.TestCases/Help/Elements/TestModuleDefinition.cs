using System.Collections.Generic;

namespace Tac.Model.Elements
{
    public class TestModuleDefinition : IModuleDefinition
    {
        public IFinalizedScope Scope { get; }
        public IEnumerable<ICodeElement> StaticInitialization { get; }
    }
}
