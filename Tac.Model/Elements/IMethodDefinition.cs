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
        IReadOnlyList<IGenericParameter> TypeParameters { get; }
    }

    public interface IGenericParameter { 
        IKey Key { get; }
    }
}