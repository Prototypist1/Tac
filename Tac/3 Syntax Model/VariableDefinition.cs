using System;
using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class VariableDefinition : IReferanced, ICodeElement
    {
        public VariableDefinition(bool readOnly, IReferance type, AbstractName key)
        {
            ReadOnly = readOnly;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public bool ReadOnly { get; }
        public IReferance Type { get; }
        public AbstractName Key { get; }
        
        public override bool Equals(object obj)
        {
            return obj is VariableDefinition definition &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<IReferance>.Default.Equals(Type, definition.Type) &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IReferance>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            return hashCode;
        }

        public ITypeDefinition ReturnType(IScope scope) {
            if (scope.TryGet(Type, out var referanced) && referanced is ITypeDefinition typeDefinition)
            {
                return typeDefinition;
            }
            throw new Exception("could not find type");
        }
    }

}