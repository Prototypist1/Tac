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

        public class GenericTypeData
        {
            public GenericTypeData(NameKey key, IGenericType type)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public NameKey Key { get; }
            public IGenericType Type { get; }
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

        private readonly IDictionary<IKey, IsStatic> members = new ConcurrentDictionary<IKey, IsStatic>();
        private readonly IDictionary<IKey, IVerifiableType> types = new ConcurrentDictionary<IKey, IVerifiableType>();
        private readonly IDictionary<NameKey, List<IGenericType>> genericTypes = new ConcurrentDictionary<NameKey, List<IGenericType>>();

        public Scope()
        {
        }

        public Scope(IFinalizedScope parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public IEnumerable<IKey> MemberKeys => members.Keys;

        public IEnumerable<IMemberDefinition> Members => members.Select(x => x.Value.Value);

        public IEnumerable<IVerifiableType> Types => types.Select(x=>x.Value);

        public IEnumerable<IKey> TypeKeys => types.Keys;

        public IEnumerable<IGenericType> GenericTypes => genericTypes.SelectMany(x=>x.Value);

        public IEnumerable<GenericKeyDefinition> GenericTypeKeys => genericTypes.SelectMany(x=> x.Value.Select(y=>new GenericKeyDefinition(x.Key,y.TypeParameterKeys)));

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
                if (!genericTypes.TryGetValue(new NameKey(genericNameKey.Name), out var list))
                {
                    //TODO



                }
            }else{
                if (!types.TryGetValue(name, out var item))
                {
                    type = item;
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

        public static (IFinalizedScope, IFinalizedScopeBuilder) Create(IFinalizedScope parent)
        {
            var res = new Scope(parent);
            return (res, res);
        }

        public static (IFinalizedScope, IFinalizedScopeBuilder) Create()
        {
            var res = new Scope();
            return (res, res);
        }

        public void Build(IReadOnlyList<IsStatic> toAdd, IReadOnlyList<Scope.TypeData> typesToAdd, IReadOnlyList<Scope.GenericTypeData> genericTypesToAdd)
        {
            foreach (var member in toAdd)
            {
                members[member.Value.Key] = member;
            }

            foreach (var type in typesToAdd)
            {
                types[type.Key] =  type.Type;
            }

            foreach (var genericType in genericTypesToAdd)
            {
                if (genericTypes.ContainsKey(genericType.Key)) {
                    genericTypes[genericType.Key].Add(genericType.Type);
                }
                else
                {
                    genericTypes[genericType.Key] = new List<IGenericType>() { genericType.Type };
                }
            }
        }
        

        public static IFinalizedScope CreateAndBuild(IReadOnlyList<IsStatic> toAdd, IReadOnlyList<Scope.TypeData> typesToAdd, IReadOnlyList<Scope.GenericTypeData> genericTypes) {
            var (x, y) = Create();
            y.Build(toAdd, typesToAdd, genericTypes);
            return x;
        }


        public static IFinalizedScope CreateAndBuild(IReadOnlyList<IsStatic> toAdd, IReadOnlyList<Scope.TypeData> typesToAdd, IReadOnlyList<Scope.GenericTypeData> genericTypes, IFinalizedScope parent)
        {
            var (x, y) = Create(parent);
            y.Build(toAdd, typesToAdd, genericTypes);
            return x;
        }
    }

    public interface IFinalizedScopeBuilder {
        void Build(IReadOnlyList<Scope.IsStatic> members, IReadOnlyList<Scope.TypeData> types, IReadOnlyList<Scope.GenericTypeData> genericTypes);
    }
}
