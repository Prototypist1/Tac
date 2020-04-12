using Prototypist.Toolbox;
using System;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    // what is internal about this??
    // well, it is not external... 
    public interface IInternalMethodDefinition: IBlockDefinition
    {
        IOrType<IVerifiableType, IError> InputType { get; }
        IOrType<IVerifiableType, IError> OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
    }
}