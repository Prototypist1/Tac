using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // TODO split scopes out in to socpes an scope builders
    public interface IScope
    {
        bool TryGetType(ExplicitTypeName name, out ITypeDefinition type);
        bool TryGetGenericType(ExplicitTypeName name, IEnumerable<ITypeDefinition> genericTypeParameters, out TypeDefinition typeDefinition);
        bool TryGetMember(ExplicitMemberName name, bool staticOnly, out MemberDefinition member);
        // this is a weird API, can I just get away with taking in a scopeStack and return a ITypeDefinition???
        bool TryGetType(ImplicitTypeReferance key, out Func<ScopeStack, ITypeDefinition> item);
    }

    public class GenericScope : IScope {
        
        private IScope Backing { get; }

        private readonly ConcurrentDictionary<ExplicitTypeName, ITypeDefinition> RealizedGenericTypes = new ConcurrentDictionary<ExplicitTypeName, ITypeDefinition>();

        public GenericScope(IScope backing, IEnumerable<GenericTypeParameter> typeParameters)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            foreach (var typeParameter in typeParameters)
            {
                if (!RealizedGenericTypes.TryAdd(typeParameter.Definition.Name, typeParameter.TypeDefinition)) {
                    throw new Exception("uhh these should add");
                }
            }
        }

        public bool TryGetType(ImplicitTypeReferance key, out Func<ScopeStack, ITypeDefinition> item) => Backing.TryGetType(key, out item);
        public bool TryGetMember(ExplicitMemberName name, bool staticOnly, out MemberDefinition member) => Backing.TryGetMember(name, staticOnly, out member);
        public bool TryGetType(ExplicitTypeName name, out ITypeDefinition type) => RealizedGenericTypes.TryGetValue(name,out type) || Backing.TryGetType(name, out type);
        public bool TryGetGenericType(ExplicitTypeName name, IEnumerable<ITypeDefinition> genericTypeParameters, out TypeDefinition typeDefinition) => Backing.TryGetGenericType(name, genericTypeParameters, out typeDefinition);
    }
}