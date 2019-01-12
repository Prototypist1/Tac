using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class Scope : IFinalizedScope, IFinalizedScopeBuilder
    {
        private readonly IFinalizedScope parent;

        public class TypeData {
            public TypeData(IKey key, IVerifiableType type)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public IKey Key { get; } 
            public IVerifiableType Type { get; }
        }

        public class IsStatic
        {
            public IsStatic(IMemberDefinition value, bool @static)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
                Static = @static;
            }

            public IMemberDefinition Value { get; }
            public bool Static { get; }
        }

        private interface ITypeHolder {
        }

        private class TypeHolder : ITypeHolder {
            public readonly IVerifiableType type;

            public TypeHolder(IVerifiableType type) {
                this.type = type ?? throw new ArgumentNullException(nameof(type));
            }
        }
        private class GenericTypeHolder : ITypeHolder { }

        private readonly IDictionary<IKey, IsStatic> members = new ConcurrentDictionary<IKey, IsStatic>();
        private readonly IDictionary<IKey, List<ITypeHolder>> types = new ConcurrentDictionary<IKey, List<ITypeHolder>>();

        public Scope()
        {
        }

        public Scope(IFinalizedScope parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public IEnumerable<IKey> MemberKeys => members.Keys;

        public IEnumerable<IMemberDefinition> Members => members.Select(x => x.Value.Value);

        public IEnumerable<IVerifiableType> Types => types.SelectMany(x=>x.Value.Select(y=>y);

        public IEnumerable<IKey> TypeKeys => types.Keys;

        public bool TryGetMember(IKey name, bool staticOnly, out IMemberDefinition member)
        {
            if (members.TryGetValue(name, out var isStatic) && (!staticOnly || isStatic.Static)) { 
                member = isStatic.Value;
                return true;
            }
            
            if (parent == null)
            {
                member = default;
                return false;
            }
            return parent.TryGetMember(name, staticOnly, out member);
        }

        public bool TryGetParent(out IFinalizedScope res)
        {
            res = parent;
            return parent == null;
        }

        public bool TryGetType(IKey name, out IVerifiableType type)
        {
            if (name is GenericNameKey genericNameKey)
            {
                if (!types.TryGetValue(new NameKey(genericNameKey.Name), out var list))
                {
                    foreach (var item in list.OfType<GenericTypeHolder>())
                    {
                        throw new NotImplementedException();
                    }
                }
            }else{
                if (!types.TryGetValue(name, out var list))
                {
                    type = list.OfType<TypeHolder>().Single().type;
                    return true;
                }
            }

            if (parent == null)
            {
                type = default;
                return false;
            }
            return parent.TryGetType(name, out type);
        }
        
        public static (IFinalizedScope, IFinalizedScopeBuilder) Create()
        {
            var res = new Scope();
            return (res, res);
        }

        public void Build(IReadOnlyList<IsStatic> toAdd, IReadOnlyList<Scope.TypeData> typesToAdd)
        {
            foreach (var member in toAdd)
            {
                members[member.Value.Key] = member;
            }

            foreach (var type in typesToAdd)
            {
                if (types.ContainsKey(type.Key))
                {
                    types[type.Key].Add(new TypeHolder(type.Type));
                }
                else {
                    types[type.Key] = new List<ITypeHolder>() { new TypeHolder(type.Type) };
                }
            }
        }
    }

    public interface IFinalizedScopeBuilder {
        void Build(IReadOnlyList<Scope.IsStatic> members, IReadOnlyList<Scope.TypeData> types);
    }
}
