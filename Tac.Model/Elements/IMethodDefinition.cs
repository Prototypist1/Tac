using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IMethodDefinition: IMethodType
    {
        IMemberDefinition ParameterDefinition { get; }

    }

    public interface IInternalMethodDefinition: IBlockDefinition, IMethodDefinition
    {
        bool IsEntryPoint { get; }
    }

    public interface IExternalMethodDefinition : IMethodDefinition
    {
    }
}