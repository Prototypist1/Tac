using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
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

    public interface IGenericMethodDefinition : IBlockDefinition
    {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
        IMemberDefinition ParameterDefinition { get; }
        // TODO you are here
        // TypeParameters should not be of type IVerifiableType
        // these aren't full type yet..
        IReadOnlyList<IVerifiableType> TypeParameters { get; }
    }
}