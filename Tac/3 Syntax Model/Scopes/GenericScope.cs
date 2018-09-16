using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class GenericScope : IScope
    {

        private IScope Backing { get; }

        private readonly ConcurrentDictionary<ExplicitTypeName, ITypeDefinition> RealizedGenericTypes = new ConcurrentDictionary<ExplicitTypeName, ITypeDefinition>();

        public GenericScope(IScope backing, IEnumerable<GenericTypeParameter> typeParameters)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            foreach (var typeParameter in typeParameters)
            {
                if (!RealizedGenericTypes.TryAdd(new ExplicitTypeName(typeParameter.Definition.Name), typeParameter.TypeDefinition))
                {
                    throw new Exception("uhh these should add");
                }
            }
        }

        public bool TryGetMember(ExplicitMemberName name, bool staticOnly, out MemberDefinition member)
        {
            return Backing.TryGetMember(name, staticOnly, out member);
        }

        public bool TryGetType(ExplicitTypeName name, out ITypeDefinition type)
        {
            return RealizedGenericTypes.TryGetValue(name, out type) || Backing.TryGetType(name, out type);
        }

        public bool TryGetGenericType(ExplicitTypeName name, IEnumerable<ITypeDefinition> genericTypeParameters, out TypeDefinition typeDefinition)
        {
            return Backing.TryGetGenericType(name, genericTypeParameters, out typeDefinition);
        }
    }
}