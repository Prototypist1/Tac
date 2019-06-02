using System;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    // what is internal about this??
    // well, it is not external... 
    public interface IInternalMethodDefinition: IBlockDefinition,  ICodeElement
    {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
        bool IsEntryPoint { get; }
    }
    
}