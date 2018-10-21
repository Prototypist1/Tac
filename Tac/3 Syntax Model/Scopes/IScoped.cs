using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScoped
    {
        IResolvableScope Scope { get; }
    }


    public class ScopeTree
    {
        private class Scope : IPopulatableScope, IResolvableScope,
            IInstanceScope
        {

            public bool TryAddInstanceMember(IKey key, IBox<MemberDefinition> definition)
            {
                return TryAddMember(DefintionLifetime.Instance, key, definition);
            }

            public bool TryAddLocal(IKey key, IBox<MemberDefinition> definition)
            {
                return TryAddMember(DefintionLifetime.Local, key, definition);
            }

            public bool TryAddStaticMember(IKey key, IBox<MemberDefinition> definition)
            {
                return TryAddMember(DefintionLifetime.Static, key, definition);
            }

            public bool TryAddStaticType(IKey key, IBox<IReturnable> definition)
            {
                return TryAddType(DefintionLifetime.Static, key, definition);
            }

            public bool TryAddStaticGenericType(IKey key, IBox<GenericTypeDefinition> definition)
            {
                return TryAddGeneric(DefintionLifetime.Static, key, definition);
            }

            protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<MemberDefinition>>>> members
        = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<MemberDefinition>>>>();

            protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IReturnable>>>> types
                = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IReturnable>>>>();


            protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>> genericTypes = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>>();

            protected bool TryAddMember(DefintionLifetime defintionLifetime, IKey key, IBox<MemberDefinition> definition)
            {
                var list = members.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<MemberDefinition>>>());
                var visiblity = new Visiblity<IBox<MemberDefinition>>(defintionLifetime, definition);
                return list.TryAdd(visiblity);
            }

            protected bool TryAddType(DefintionLifetime defintionLifetime, IKey key, IBox<IReturnable> definition)
            {
                var list = types.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<IReturnable>>>());
                var visiblity = new Visiblity<IBox<IReturnable>>(defintionLifetime, definition);
                return list.TryAdd(visiblity);
            }

            protected bool TryAddGeneric(DefintionLifetime defintionLifetime, IKey key, IBox<GenericTypeDefinition> definition)
            {
                var list = genericTypes.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>());
                var visiblity = new Visiblity<IBox<GenericTypeDefinition>>(defintionLifetime, definition);
                return list.TryAdd(visiblity);
            }

            public bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> member)
            {
                if (!members.TryGetValue(name, out var items))
                {
                    member = default;
                    return false;
                }

                var thing = items.SingleOrDefault();

                if (thing == default)
                {
                    member = default;
                    return false;
                }

                member = thing.Definition;
                return true;
            }

            public bool TryGetType(IKey name, out IBox<IReturnable> type)
            {
                if (!types.TryGetValue(name, out var items))
                {
                    type = default;
                    return false;
                }

                var thing = items.SingleOrDefault();

                if (thing == default)
                {
                    type = default;
                    return false;
                }

                type = thing.Definition;
                return true;
            }

            public IResolvableScope ToResolvable()
            {
                return this;
            }

            public IReadOnlyList<IBox<IReturnable>> Types
            {
                get
                {
                    return types.Select(x => x.Value.Single().Definition).ToArray();
                }
            }

            public IReadOnlyList<IBox<MemberDefinition>> Members
            {
                get
                {
                    return members.Select(x => x.Value.Single().Definition).ToArray();
                }
            }
        }

        private readonly Scope root;

        public ScopeTree()
        {
            root = new Scope();
        }

        /// <summary>
        /// we only point upwards, say you have:
        /// 
        /// object A {
        ///     x : object B{
        ///     
        ///     }
        /// }
        /// 
        /// then:
        /// 
        /// ScopeParent[B] = A;
        /// </summary>
        private Dictionary<Scope, Scope> ScopeParent { get; } = new Dictionary<Scope, Scope>();
        
        private Scope[] Scopes(Scope scope)
        {
            return Inner().ToArray();

            IEnumerable<Scope> Inner()
            {
                yield return scope;
                while (ScopeParent.ContainsKey(scope))
                {
                    scope = ScopeParent[scope];
                    yield return scope;
                }
            }
        }
        
        private ScopeTree Add(Scope oldTop, Scope newTop)
        {
            ScopeParent[newTop] = oldTop;
            return this;
        }
        
        public class ScopeStack
        {
            public static ScopeStack Root() {

                var scopeTree = new ScopeTree();
                return new ScopeStack(scopeTree, scopeTree.root);
            }

            public (IStaticScope,ScopeStack) StaticScope()
            {
                var res = new Scope();
                ScopeTree.Add(TopScope, res);
                return (res, new ScopeStack(ScopeTree, res));
            }

            public (ILocalStaticScope,ScopeStack) LocalStaticScope()
            {
                var res = new Scope();
                ScopeTree.Add(TopScope, res);
                return (res, new ScopeStack(ScopeTree,res));
            }

            private ScopeStack(ScopeTree scopeTree, Scope topScope)
            {
                ScopeTree = scopeTree ?? throw new ArgumentNullException(nameof(scopeTree));
                TopScope = topScope ?? throw new ArgumentNullException(nameof(topScope));
            }
            
            public ScopeTree ScopeTree { get; }
            private Scope TopScope { get; }

            public IBox<IReturnable> GetType(IKey key)
            {
                foreach (var scope in ScopeTree.Scopes(TopScope))
                {
                    if (scope.TryGetType(key, out var typeDefinition))
                    {
                        return typeDefinition;
                    }
                }
                throw new Exception("");

            }

            public bool TryGetMemberPath(NameKey name, out int depth, out IBox<MemberDefinition> box)
            {
                var up = 0;
                foreach (var scope in ScopeTree.Scopes(TopScope))
                {
                    if (scope.TryGetMember(name, false, out var memberDefinition))
                    {
                        depth = up;
                        box = memberDefinition;
                        return true;
                    }
                    up++;
                }
                depth = 0;
                box = default;
                return false;
            }

            internal IBox<MemberDefinition> GetMemberDefinition(NameKey key)
            {
                foreach (var scope in ScopeTree.Scopes(TopScope))
                {
                    if (scope.TryGetMember(key, false, out var memberDefinition))
                    {
                        return memberDefinition;
                    }
                }
                return default;
            }
        }
    }
}

