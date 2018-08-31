using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ParameterDefinition: IReferanced
    {
        public ParameterDefinition(bool readOnly, Referance type, AbstractName key)
        {
            ReadOnly = readOnly;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public bool ReadOnly { get; }
        public Referance Type { get; }
        public AbstractName Key { get; }

        public override bool Equals(object obj)
        {
            return obj is ParameterDefinition definition && definition != null &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<Referance>.Default.Equals(Type, definition.Type) &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Referance>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            return hashCode;
        }

        public ITypeDefinition ReturnType(ScopeStack scope) {
            if (scope.TryGet(Type.key.names, out var res) && res is ITypeDefinition type)
            {
                return type;
            }
            else {
                throw new Exception("return type not found");
            }
        }
    }
}