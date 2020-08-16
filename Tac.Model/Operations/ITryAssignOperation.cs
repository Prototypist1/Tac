using Tac.Model.Elements;

namespace Tac.Model.Operations
{
    public interface ITryAssignOperation : IOperation
    {
        ICodeElement Left { get; }
        ICodeElement Right { get; }
        ICodeElement Block { get; }
        IFinalizedScope Scope { get; }
    }

}
