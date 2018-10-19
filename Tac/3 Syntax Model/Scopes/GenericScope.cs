using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class GenericScope : IResolvableScope
    {

        private IResolvableScope Backing { get; }

        public IEnumerable<IKey> MembersKeys
        {
            get
            {
                return Backing.MembersKeys;
            }
        }

        private readonly ConcurrentDictionary<NameKey, IBox<IReturnable>> RealizedGenericTypes = new ConcurrentDictionary<NameKey, IBox<IReturnable>>();

        public GenericScope(IResolvableScope backing, IEnumerable<GenericTypeParameter> typeParameters)
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

        public bool TryGetType(IKey name, out IBox<IReturnable> type)
        {
            if (name is NameKey nameKey && RealizedGenericTypes.TryGetValue(nameKey, out var innerType))
            {
                type = innerType;
                return true;
            }

            if (Backing.TryGetType(name, out type))
            {
                return true;
            }

            return false;
        }
    }
}