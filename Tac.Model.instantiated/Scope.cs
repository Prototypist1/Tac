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
        private readonly IFinalizedScope? parent;

        public class TypeData {
            public TypeData(IKey key, IVerifiableType type)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public IKey Key { get; } 
            public IVerifiableType Type { get; }
        }

        //public class GenericTypeData
        //{
        //    public GenericTypeData(NameKey key, IGenericType type)
        //    {
        //        Key = key ?? throw new ArgumentNullException(nameof(key));
        //        Type = type ?? throw new ArgumentNullException(nameof(type));
        //    }

        //    public NameKey Key { get; }
        //    public IGenericType Type { get; }
        //}

        

        private readonly ConcurrentDictionary<IKey, IsStatic> members = new ConcurrentDictionary<IKey, IsStatic>();
        //private readonly ConcurrentDictionary<IKey, IInterfaceType> types = new ConcurrentDictionary<IKey, IInterfaceType>();
        //private readonly IDictionary<NameKey, List<IGenericType>> genericTypes = new ConcurrentDictionary<NameKey, List<IGenericType>>();

        public Scope()
        {
        }

        public Scope(IFinalizedScope parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        //public IReadOnlyList<IKey> MemberKeys => members.Keys.ToList();

        //public IReadOnlyList<IMemberDefinition> Members => members.Select(x => x.Value.Value).ToList();

        public IReadOnlyDictionary<IKey, IsStatic> Members => members;

        //public IReadOnlyDictionary<IKey, IInterfaceType> Types => types;


        //public IReadOnlyList<GenericTypeEntry> GenericTypes => genericTypes.SelectMany(x=> x.Value.Select(y=> new GenericTypeEntry(y, new GenericKeyDefinition(x.Key,y.TypeParameterKeys)))).ToList();

        //IReadOnlyList<TypeEntry> IFinalizedScope.Types => types.Select(x => new TypeEntry(x.Key, x.Value)).ToList();

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

        public void Build(IReadOnlyList<IsStatic> toAdd) //, IReadOnlyList<Scope.TypeData> typesToAdd
        {
            foreach (var member in toAdd)
            {
                members[member.Value.Key] = member;
            }

            //foreach (var type in typesToAdd)
            //{
            //    types[type.Key] =  type.Type;
            //}

        }
        

        public static IFinalizedScope CreateAndBuild(IReadOnlyList<IsStatic> toAdd) {
            var (x, y) = Create();
            y.Build(toAdd);
            return x;
        }


        public static IFinalizedScope CreateAndBuild(IReadOnlyList<IsStatic> toAdd, IFinalizedScope parent)
        {
            var (x, y) = Create(parent);
            y.Build(toAdd);
            return x;
        }
    }

    public interface IFinalizedScopeBuilder {
        void Build(IReadOnlyList<IsStatic> members);
    }
}
