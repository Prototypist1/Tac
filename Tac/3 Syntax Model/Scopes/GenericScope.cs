using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class GenericScope : IScope
    {

        private IScope Backing { get; }

        public IReadOnlyList<MemberDefinition> Members
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

        public bool TryGetMember(NameKey name, bool staticOnly, out MemberDefinition member)
        {
            return Backing.TryGetMember(name, staticOnly, out member);
        }

        public bool TryGetType(NameKey name, out ITypeDefinition type)
        {
            return RealizedGenericTypes.TryGetValue(name, out type) || Backing.TryGetType(name, out type);
        }

        public bool TryGetGenericType(NameKey name, IEnumerable<ITypeDefinition> genericTypeParameters, out TypeDefinition typeDefinition)
        {
            return Backing.TryGetGenericType(name, genericTypeParameters, out typeDefinition);
        }
    }
}