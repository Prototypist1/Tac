namespace Tac.Model.Elements
{
    public interface IConstantNumber: ICodeElement {
        double Value { get; }
    }

    public interface IEmptyInstance : ICodeElement {
    }
}
