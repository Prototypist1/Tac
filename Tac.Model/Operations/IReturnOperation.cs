using Prototypist.Toolbox;

namespace Tac.Model.Operations
{
    public interface IReturnOperation : ICodeElement{
        IOrType<ICodeElement, IError> Result { get; }
    }
}
