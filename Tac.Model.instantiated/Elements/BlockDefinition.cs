using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class BlockDefinition : AbstractBlockDefinition, IBlockDefinition
    {
        public BlockDefinition(IFinalizedScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers) : base(scope, body, staticInitailizers)
        {
        }

        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.BlockDefinition(this);
        }
    }
}