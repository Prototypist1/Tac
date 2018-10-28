using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IMethodDefinition: IBlockDefinition {

        ITypeDefinition InputType { get; }
        ITypeDefinition OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
    }
}