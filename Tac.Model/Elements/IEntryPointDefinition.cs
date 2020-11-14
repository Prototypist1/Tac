namespace Tac.Model.Elements
{
    public interface IEntryPointDefinition: IAbstractBlockDefinition
    {
        //IVerifiableType VerifiableType();
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }

    }
}