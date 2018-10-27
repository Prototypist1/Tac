namespace Tac.Semantic_Model
{
    public interface IMethodDefinition: IBlockDefinition {

        ITypeDefinition InputType { get; }
        ITypeDefinition OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
    }
}