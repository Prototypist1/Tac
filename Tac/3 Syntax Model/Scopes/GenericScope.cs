using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class GenericScope : IScope
    {

        private IScope Backing { get; }

        public IReadOnlyList<IBox<MemberDefinition>> Members
        {
            get
            {
                return Backing.Members;
            }
        }

        private readonly ConcurrentDictionary<NameKey, ITypeDefinition> RealizedGenericTypes = new ConcurrentDictionary<NameKey, ITypeDefinition>();

        public GenericScope(IScope backing, IEnumerable<GenericTypeParameter> typeParameters)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            foreach (var typeParameter in typeParameters)
            {
                if (!RealizedGenericTypes.TryAdd(typeParameter.Definition.Key, typeParameter.TypeDefinition))
                {
                    throw new Exception("uhh these should add");
                }
            }
        }

        public bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> member)
        {
            return Backing.TryGetMember(name, staticOnly, out member);
        }

        public bool TryGetType(NameKey name, out IBox<ITypeDefinition> type)
        {
            if (RealizedGenericTypes.TryGetValue(name, out var innerType)) {
                type = new Box<ITypeDefinition>(innerType);
                return true;
            }

            if (Backing.TryGetType(name, out type)) {
                return true;
            }

            return false;
        }

        public bool TryGetGenericType(NameKey name, IEnumerable<ITypeDefinition> genericTypeParameters, out IBox<GenericTypeDefinition> typeDefinition)
        {
            return Backing.TryGetGenericType(name, genericTypeParameters, out typeDefinition);
        }
    }
}