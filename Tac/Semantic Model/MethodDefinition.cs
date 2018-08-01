using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    public class MethodDefinition: BlockDefinition
    {
        public TypeReferance InputType { get => ParameterDefinition.Type; }
        public TypeReferance OutputType { get; }
        public ParameterDefinition ParameterDefinition { get; }
        public BlockDefinition MethodBodyDefinition { get; }

        public override TReferanced Get<TKey, TReferanced>(TKey key) => throw new NotImplementedException();
    }
}