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

        public ITypeDefinition GetGenericType(GenericExplicitTypeName type)
        {

            foreach (var scope in ScopeTree.Scopes(TopScope))
            {
                if (scope.TryGetGenericType(type, type.Types.Select(x => x.GetTypeDefinition(this)).ToArray(), out var typeDefinition))
                {
                    return typeDefinition;
                }
            }
            throw new Exception("");

        }

        public ITypeDefinition GetType(ExplicitTypeName name)
        {
            foreach (var scope in ScopeTree.Scopes(TopScope))
            {
                if (scope.TryGetType(name, out var typeDefinition))
                {
                    return typeDefinition;
                }
            }
            throw new Exception("");

        }

        public MemberDefinition GetMemberOrDefault(ExplicitMemberName name)
        {
            foreach (var scope in ScopeTree.Scopes(TopScope))
            {
                if (scope.TryGetMember(name, false, out var memberDefinition))
                {
                    return memberDefinition;
                }
            }
            return new MemberDefinition(false, name, RootScope.AnyType.GetTypeDefinition(this));

        }
    }
}

