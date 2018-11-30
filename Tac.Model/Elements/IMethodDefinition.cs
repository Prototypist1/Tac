using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IMethodDefinition: IBlockDefinition, IMethodType
    {
        IMemberDefinition ParameterDefinition { get; }
    }
}