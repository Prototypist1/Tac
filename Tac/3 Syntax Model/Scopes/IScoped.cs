using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScoped
    {
        IScope Scope { get; }
    }

    public static class ScopedExtensions
    {

        public static ScopeStack GrowScopeStack(this IScoped scope, ScopeStack stack)
        {
            return new ScopeStack(stack, scope.Scope);
        }
    }

    public class ScopeTree
    {
        public ScopeTree(IScope root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
        }

        /// <summary>
        /// we only point upwards
        /// object A {
        ///     x : object B{
        ///     
        ///     }
        /// }
        /// ScopeParent[B] = A;
        /// </summary>
        private Dictionary<IScope, IScope> ScopeParent { get; } = new Dictionary<IScope, IScope>();

        public IScope Root { get; }

        public IScope[] Scopes(IScope scope)
        {
            return Inner().ToArray();

            IEnumerable<IScope> Inner()
            {
                yield return scope;
                while (ScopeParent.ContainsKey(scope))
                {
                    scope = ScopeParent[scope];
                    yield return scope;
                }
            }
        }
        
        internal ScopeTree Add(IScope oldTop, IScope newTop)
        {
            ScopeParent[newTop] = oldTop;
            return this;
        }
    }

    public class ScopeStack
    {

        public ScopeStack(ScopeTree scopeTree, IScope topScope)
        {
            ScopeTree = scopeTree ?? throw new ArgumentNullException(nameof(scopeTree));
            TopScope = topScope ?? throw new ArgumentNullException(nameof(topScope));
        }

        public ScopeStack(ScopeStack scopes, IScope newScope) : this(scopes.ScopeTree.Add(scopes.TopScope, newScope), newScope) { }

        public ScopeTree ScopeTree { get; }
        public IScope TopScope { get; }
        
        public IBox<ITypeDefinition> GetType(ExplicitTypeName name)
        {
            if (name is GenericExplicitTypeName type) {
                var types = type.Types.Select(x => GetType(x)).ToArray();

                foreach (var scope in ScopeTree.Scopes(TopScope))
                {
                    if (scope.TryGetGenericType(type.Key, types, out var typeDefinition))
                    {
                        return typeDefinition;
                    }
                }
            }

            foreach (var scope in ScopeTree.Scopes(TopScope))
            {
                if (scope.TryGetType(name.Key, out var typeDefinition))
                {
                    return typeDefinition;
                }
            }
            throw new Exception("");

        }

        public bool TryGetMemberPath(ExplicitMemberName name, out int depth, out IBox<MemberDefinition> box)
        {
            var up = 0;
            foreach (var scope in ScopeTree.Scopes(TopScope))
            {
                if (scope.TryGetMember(name.Key, false, out var memberDefinition))
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

