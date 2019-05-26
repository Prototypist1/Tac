namespace Tac.Model.Elements
{
    public interface IConstantNumber: ICodeElement {
        double Value { get; }
    }

    public interface IConstantString : ICodeElement
    {
        string Value { get; }
    }


    public interface IConstantBool : ICodeElement
    {
        bool Value { get; }
    }


    //public interface IEmptyInstance : ICodeElement {
    //}
}
