using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public class TestImplementationDefinition : IImplementationDefinition
    {
        public IVarifiableType OutputType { get; }
        public IMemberDefinition ContextDefinition { get; }
        public IMemberDefinition ParameterDefinition { get; }
        public IFinalizedScope Scope { get; }
        public IEnumerable<ICodeElement> MethodBody { get; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; }
    }
}
