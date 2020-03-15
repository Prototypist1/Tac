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

        public class TypeData {
            public TypeData(IKey key, IVerifiableType type)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public IKey Key { get; } 
            public IVerifiableType Type { get; }
        }

        

        private readonly ConcurrentDictionary<IKey, IsStatic> members = new ConcurrentDictionary<IKey, IsStatic>();

        public Scope()
        {
        }


        public IReadOnlyDictionary<IKey, IsStatic> Members => members;


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
        }
        

        public static IFinalizedScope CreateAndBuild(IReadOnlyList<IsStatic> toAdd) {
            var (x, y) = Create();
            y.Build(toAdd);
            return x;
        }


    }

    public interface IFinalizedScopeBuilder {
        void Build(IReadOnlyList<IsStatic> members);
    }
}
