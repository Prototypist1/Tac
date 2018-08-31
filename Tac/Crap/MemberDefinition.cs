using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public sealed class ImplicitMemberDefinition : AbstractMemberDefinition<ImplicitTypeReferance>
    {
        public ImplicitMemberDefinition(bool readOnly, bool isStatic, ImplicitTypeReferance type, AbstractName key) : base(readOnly, isStatic, type, key)
        {
        }
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            
            if (Type.Is(out ImplicitTypeReferance implicitTypeReferance) && scope.TryGet(Type, out var implicitTypeReferanceRes) && implicitTypeReferanceRes is TypeDefinition implicitTypeReferanceType)
            {
                return implicitTypeReferanceType;
            }

            throw new Exception("Type not found");
        }
    }

    public sealed class MemberDefinition : AbstractMemberDefinition<Referance>
    {
        public MemberDefinition(bool readOnly, bool isStatic, Referance type, AbstractName key) : base(readOnly, isStatic, type, key)
        {
        }
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            if ( scope.TryGet(Type.key.names, out var referanceRes) && referanceRes is TypeDefinition referanceType)
            {
                return referanceType;
            }
            
            throw new Exception("Type not found");
        }
    }

    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    public abstract class AbstractMemberDefinition: IReferanced, ICodeElement
    {
        public AbstractMemberDefinition(bool readOnly, bool isStatic, AbstractName key)
        {
            ReadOnly = readOnly;
            IsStatic = isStatic;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public bool ReadOnly { get; }
        public AbstractName Key { get; }
        public bool IsStatic { get; }

        public override bool Equals(object obj)
        {
            return obj is AbstractMemberDefinition definition &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            return hashCode;
        }

        public abstract ITypeDefinition ReturnType(ScopeStack scope);
    }

    public abstract class AbstractMemberDefinition<TType>: AbstractMemberDefinition
        where TType : class 
    {
        public AbstractMemberDefinition(bool readOnly, bool isStatic, TType type, AbstractName key): base(readOnly,isStatic,key)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public TType Type { get; }
        
        public override bool Equals(object obj)
        {
            return obj is AbstractMemberDefinition<TType> definition &&
                   Type.Equals(definition.Type) && 
                   base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }
        
    }

}