using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IMethodDefinition: IBlockDefinition {

        IConvertableType InputType { get; }
        IConvertableType OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
    }
}