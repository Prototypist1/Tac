using System;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IMethodDefinition: IMethodType, ICodeElement
    {
        IMemberDefinition ParameterDefinition { get; }

    }

    public interface IInternalMethodDefinition: IBlockDefinition, IMethodDefinition
    {
        bool IsEntryPoint { get; }
    }

    public interface IExternalMethodDefinition : IMethodDefinition
    {
        Guid Id { get; }
    }
}