namespace Tac.Model.Operations
{
    public interface IReturnOperation : ICodeElement{
        ICodeElement Result { get; }
    }
}
