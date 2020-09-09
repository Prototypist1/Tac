using Prototypist.Toolbox;

namespace Tac.Model.Operations
{
    public interface IReturnOperation : IOperation{
        ICodeElement Result { get; }
    }
}
