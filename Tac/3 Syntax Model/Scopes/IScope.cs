using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface ISomeScope {
        bool TryGetMember(IKey name, bool staticOnly, out IBox<WeakMemberDefinition> box);
    }

    public interface IPopulatableScope: ISomeScope
    {
        bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<WeakMemberDefinition> type);
        bool TryAddType(IKey name, IBox<IType> type);
    }
    
    public interface IResolvableScope: ISomeScope
    {
        IWeakFinalizedScope GetFinalized();
        bool TryGetType(IKey name, out IBox<IType> type);
    }

    public class ScopeEnty<T>
        where T: class
    {
        public readonly IBox<T> element;
        public readonly IKey key;

        public ScopeEnty(IBox<T> element, IKey key)
        {
            this.element = element ?? throw new ArgumentNullException(nameof(element));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }

    public interface IWeakFinalizedScope
    {
        IReadOnlyDictionary<IKey, IBox<WeakMemberDefinition>> Members { get; }
    }

    public class FinalizedScope : IWeakFinalizedScope
    {
        public FinalizedScope(IReadOnlyDictionary<IKey, IBox<WeakMemberDefinition>> members)
        {
            Members = members ?? throw new ArgumentNullException(nameof(members));
        }

        public IReadOnlyDictionary<IKey, IBox<WeakMemberDefinition>> Members
        {
            get;
        }
    }

    public static class ResolvableScopeExtension
    {
        public static IBox<IType> GetTypeOrThrow(this IResolvableScope scope, NameKey name) {
            if (scope.TryGetType(name, out var thing)) {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }

        public static IBox<WeakMemberDefinition> GetMemberOrThrow(this IResolvableScope scope, NameKey name, bool staticOnly)
        {
            if (scope.TryGetMember(name, staticOnly, out var thing))
            {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }
    }
}