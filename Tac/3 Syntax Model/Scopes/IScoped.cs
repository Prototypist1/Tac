using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScoped<out TScope> where TScope : IScope
    {
        TScope Scope { get; }
    }

    public static class ScopedExtensions {

        public static ScopeStack GrowScopeStack(this IScoped<IScope> scope, ScopeStack stack) {
            return new ScopeStack(stack, scope.Scope);
        }
    }

    // is this really IScope???
    // I mean it has the interface...
    // but it is sorta different...
    // hmmm
    public class ScopeStack 
    {
        public ScopeStack(ScopeStack scopes, IScope newScope) {
            if (scopes == null)
            {
                throw new ArgumentNullException(nameof(scopes));
            }

            if (newScope == null)
            {
                throw new ArgumentNullException(nameof(newScope));
            }

            var stack = scopes.Scopes.ToList();
            stack.Insert(0,newScope);
            Scopes = stack.ToArray();
        }
        public ScopeStack(IEnumerable<IScope> scopes) => Scopes = scopes?.ToArray() ?? throw new ArgumentNullException(nameof(scopes));

        public IScope[] Scopes { get; }

        public bool TryGet(IEnumerable<AbstractName> names, out IReferanced item) {
            foreach (var scope in Scopes) {
                if (scope.TryGet(names, out item)) {
                    return true;
                }
            }
            item = default;
            return false;
        }

        public bool TryGet(ImplicitTypeReferance key, out ITypeDefinition item) {
            foreach (var scope in Scopes)
            {
                if (scope.TryGet(key, out item))
                {
                    return true;
                }
            }
            item = default;
            return false;
        }
    }
}

