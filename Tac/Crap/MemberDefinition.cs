using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    // TODO remove!
    public sealed class ImplicitlyTypedMemberDefinition 
    {
        public ImplicitlyTypedMemberDefinition(bool readOnly, ExplicitMemberName key)
        {
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }


        public ExplicitMemberName Key { get; }
        public bool ReadOnly { get; }

        public override bool Equals(object obj)
        {
            return obj is MemberDefinition definition &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<ExplicitMemberName>.Default.Equals(Key, definition.Key);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ExplicitMemberName>.Default.GetHashCode(Key);
            return hashCode;
        }

        public MemberDefinition MakeMemberDefinition(ITypeSource type) {
            return new MemberDefinition(ReadOnly, Key, type);
        }
    }

    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 

    public class MemberDefinition: ICodeElement, IMemberSource
    {
        public MemberDefinition(bool readOnly, ExplicitMemberName key, ITypeDefinition type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public ITypeDefinition Type { get; }
        public bool ReadOnly { get; }
        public ExplicitMemberName Key { get; }

        public override bool Equals(object obj)
        {
            return obj is MemberDefinition definition &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<ExplicitMemberName>.Default.Equals(Key, definition.Key) &&
                   EqualityComparer<ITypeDefinition>.Default.Equals(Type, definition.Type);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ExplicitMemberName>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<ITypeDefinition>.Default.GetHashCode(Type);
            return hashCode;
        }

        public ITypeDefinition ReturnType(ScopeStack scope) {
            return Type;
        }

        public MemberDefinition GetMemberDefinition(ScopeStack scopeStack) => this;
    }
}