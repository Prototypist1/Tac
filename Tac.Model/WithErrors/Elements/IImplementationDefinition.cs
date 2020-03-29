using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.WithErrors.Elements
{
    public interface IImplementationDefinition : ICodeElement
    {
        IVerifiableType OutputType { get; }
        IMemberDefinition ContextDefinition { get; }
        IMemberDefinition ParameterDefinition { get; }
        IFinalizedScope Scope { get; }
        IEnumerable<ICodeElement> MethodBody { get; }
        IEnumerable<ICodeElement> StaticInitialzers { get; }
    }
}
