using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    
    public sealed class MemberDefinition: IReferanced, ICodeElement
    {
        public MemberDefinition(bool readOnly, bool isStatic, AbstractName key, ITypeSource type)
        {
            Type = type;
            ReadOnly = readOnly;
            IsStatic = isStatic;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public ITypeSource Type { get; }
        public bool ReadOnly { get; }
        public AbstractName Key { get; }
        // does static really live here?
        // the scope stack should know this...
        public bool IsStatic { get; }

        public override bool Equals(object obj)
        {
            return obj is MemberDefinition definition &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key) &&
                   EqualityComparer<ITypeSource>.Default.Equals(Type, definition.Type);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<ITypeSource>.Default.GetHashCode(Type);
            return hashCode;
        }

        public ITypeDefinition<IScope> ReturnType(ScopeStack scope) {
            if (!Type.TryGetTypeDefinition(scope, out var res)) {
                throw new Exception("Type not found");
            }

            return res;
        }
    }
}