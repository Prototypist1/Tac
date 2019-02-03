using System;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IInternalMethodDefinition: IBlockDefinition, IMethodType, ICodeElement
    {
        IMemberDefinition ParameterDefinition { get; }
        bool IsEntryPoint { get; }
    }
    
}