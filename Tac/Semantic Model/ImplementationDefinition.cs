using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ImplementationDefinition : BlockDefinition, IReferanced<ImplementationName>
    {
        public ImplementationName Key { get; }

        public TypeReferance ContextType { get; }
        public TypeReferance InputType { get => ParameterDefinition.Type; }
        public TypeReferance OutputType { get; }
        public ParameterDefinition ParameterDefinition { get; }
        public BlockDefinition MethodBodyDefinition { get; }

        public override TReferanced Get<TKey, TReferanced>(TKey key) => throw new NotImplementedException();
    }
}
