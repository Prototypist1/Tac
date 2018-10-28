using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IMethodDefinition: IBlockDefinition {

        IType InputType { get; }
        IType OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
    }
}