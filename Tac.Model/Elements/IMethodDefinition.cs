using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IMethodDefinition: IBlockDefinition {

        IVarifiableType InputType { get; }
        IVarifiableType OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
    }
}