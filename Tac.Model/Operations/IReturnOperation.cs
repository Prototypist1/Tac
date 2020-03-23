using Prototypist.Toolbox;

namespace Tac.Model.Operations
{
    public interface IReturnOperation : ICodeElement{
        OrType<ICodeElement, IError> Result { get; }
    }
}
