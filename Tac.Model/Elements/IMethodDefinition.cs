using System;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    // what is internal about this??
    public interface IInternalMethodDefinition: IBlockDefinition,  ICodeElement
    {
        IMemberDefinition ParameterDefinition { get; }
        bool IsEntryPoint { get; }
    }
    
}