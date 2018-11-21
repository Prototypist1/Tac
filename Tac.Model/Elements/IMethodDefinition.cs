using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IMethodDefinition: IBlockDefinition {

        ITypeReferance InputType { get; }
        ITypeReferance OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
    }
}