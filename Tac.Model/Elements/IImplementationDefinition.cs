using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IImplementationDefinition : ICodeElement, IImplementationType
    {
        IMemberDefinition ContextDefinition { get; }
        IMemberDefinition ParameterDefinition { get; }
        IFinalizedScope Scope { get; }
        IEnumerable<ICodeElement> MethodBody { get; }
        IEnumerable<ICodeElement> StaticInitialzers { get; }
    }
}
