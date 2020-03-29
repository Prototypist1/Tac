using Prototypist.Toolbox;

namespace Tac.Model.WithErrors.Operations
{
    public interface IReturnOperation : ICodeElement{
        IOrType<ICodeElement, IError> Result { get; }
    }
}
