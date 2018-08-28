using System;
using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    public sealed class MemberDefinition: IReferanced, ICodeElement
    {
        public MemberDefinition(bool readOnly, bool isStatic, IReferance type, AbstractName key)
        {
            ReadOnly = readOnly;
            IsStatic = isStatic;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public bool ReadOnly { get; }
        public IReferance Type { get; }
        public AbstractName Key { get; }
        public bool IsStatic { get; }
        
        public override bool Equals(object obj)
        {
            return obj is MemberDefinition definition &&
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
            if (scope.TryGet(Type, out var res) && res is TypeDefinition type)
            {
                return type;
            }

            throw new Exception("Type not found");
        }
    }

}