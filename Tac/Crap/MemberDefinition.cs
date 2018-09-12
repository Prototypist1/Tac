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

    public sealed class MemberDefinition: ICodeElement, IMemberSource
    {
        public MemberDefinition(bool readOnly, ExplicitMemberName key, ITypeSource type)
        {
            Type = type;
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public ITypeSource Type { get; }
        public bool ReadOnly { get; }
        public ExplicitMemberName Key { get; }

        public override bool Equals(object obj)
        {
            return obj is MemberDefinition definition &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<ExplicitMemberName>.Default.Equals(Key, definition.Key) &&
                   EqualityComparer<ITypeSource>.Default.Equals(Type, definition.Type);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ExplicitMemberName>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<ITypeSource>.Default.GetHashCode(Type);
            return hashCode;
        }

        public ITypeDefinition ReturnType(ScopeStack scope) {
            return Type.GetTypeDefinition(scope);
        }

        public MemberDefinition GetMemberDefinition(ScopeStack scopeStack) => this;
    }
}