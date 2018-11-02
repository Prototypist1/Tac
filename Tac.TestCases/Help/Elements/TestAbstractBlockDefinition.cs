using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public class TestAbstractBlockDefinition: IAbstractBlockDefinition
    {
        public IFinalizedScope Scope { get; }
        public ICodeElement[] Body { get; }
        public IEnumerable<ICodeElement> StaticInitailizers { get; }
    }
}