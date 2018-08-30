using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public MemberDefinition(bool readOnly, bool isStatic, AbstractName key) : this (readOnly, isStatic, new OrType<Referance, ImplicitTypeReferance>(new ImplicitTypeReferance()), key)
        {
        }

        public MemberDefinition(bool readOnly, bool isStatic, Referance referance, AbstractName key) : this(false, isStatic, new OrType<Referance, ImplicitTypeReferance>(referance), key)
        {
        }

        public MemberDefinition(bool readOnly, bool isStatic, OrType<Referance, ImplicitTypeReferance> type, AbstractName key)
        {
            ReadOnly = readOnly;
            IsStatic = isStatic;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public bool ReadOnly { get; }
        public OrType<Referance, ImplicitTypeReferance> Type { get; }
        public AbstractName Key { get; }
        public bool IsStatic { get; }
        
        public override bool Equals(object obj)
        {
            return obj is MemberDefinition definition &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<OrType<Referance, ImplicitTypeReferance>>.Default.Equals(Type, definition.Type) &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<OrType<Referance, ImplicitTypeReferance>>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            return hashCode;
        }

        public ITypeDefinition ReturnType(IScope scope) {
            if (Type.Is(out Referance referance) &&  scope.TryGet(referance.key.names, out var referanceRes) && referanceRes is TypeDefinition referanceType)
            {
                return referanceType;
            }

            if (Type.Is(out ImplicitTypeReferance implicitTypeReferance) && scope.TryGet(implicitTypeReferance, out var implicitTypeReferanceRes) && implicitTypeReferanceRes is TypeDefinition implicitTypeReferanceType)
            {
                return implicitTypeReferanceType;
            }
            
            throw new Exception("Type not found");
        }
    }

}