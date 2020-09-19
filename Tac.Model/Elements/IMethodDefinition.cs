using Prototypist.Toolbox;
using System;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    // what is internal about this??
    // well, it is not external... 

    // the ParameterDefinition is inclucded in the scope
    public interface IInternalMethodDefinition: IBlockDefinition
    {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
    }
}