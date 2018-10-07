using Prototypist.LeftToRight;
using System;
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

        public readonly Scope root;

        public ScopeTree(Scope root)
        {
            root = root ?? throw new ArgumentNullException(nameof(root));
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


        public Scope[] Scopes(Scope scope)
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
        
        internal ScopeTree Add(Scope oldTop, Scope newTop)
        {
            ScopeParent[newTop] = oldTop;
            return this;
        }
    }

    public class ScopeStack
    {

        public ScopeStack(ScopeTree scopeTree, Scope topScope)
        {
            ScopeTree = scopeTree ?? throw new ArgumentNullException(nameof(scopeTree));
            TopScope = topScope ?? throw new ArgumentNullException(nameof(topScope));
        }

        public ScopeStack(ScopeStack scopes, Scope newScope) : this(scopes.ScopeTree.Add(scopes.TopScope, newScope), newScope) { }

        public ScopeTree ScopeTree { get; }
        public Scope TopScope { get; }
        
        public IBox<ITypeDefinition> GetType(IKey key)
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

