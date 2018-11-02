using System.Collections.Generic;

namespace Tac.Model.Elements
{
    public class TestBlockDefinition : TestAbstractBlockDefinition, IBlockDefinition
    {
        public TestBlockDefinition(IFinalizedScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers) : base(scope, body, staticInitailizers)
        {
        }

        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.BlockDefinition(this);
        }
    }
}