using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface ISomeScope {
        bool TryGetMember(IKey name, bool staticOnly, out IBox<MemberDefinition> box);
    }

    public interface IPopulatableScope: ISomeScope
    {
        bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<MemberDefinition> type);
        bool TryAddType(IKey name, IBox<IReturnable> type);
    }
    
    public interface IResolvableScope: ISomeScope
    {
        IFinalizedScope GetFinalized();
        bool TryGetType(IKey name, out IBox<IReturnable> type);
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

    public interface IFinalizedScope
    {
        IReadOnlyDictionary<IKey, IBox<MemberDefinition>> Members { get; }
    }

    public class FinalizedScope : IFinalizedScope
    {
        public FinalizedScope(IReadOnlyDictionary<IKey, IBox<MemberDefinition>> members)
        {
            Members = members ?? throw new ArgumentNullException(nameof(members));
        }

        public IReadOnlyDictionary<IKey, IBox<MemberDefinition>> Members
        {
            get;
        }
    }

    public static class ResolvableScopeExtension
    {
        public static IBox<IReturnable> GetTypeOrThrow(this IResolvableScope scope, NameKey name) {
            if (scope.TryGetType(name, out var thing)) {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }

        public static IBox<MemberDefinition> GetMemberOrThrow(this IResolvableScope scope, NameKey name, bool staticOnly)
        {
            if (scope.TryGetMember(name, staticOnly, out var thing))
            {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }
    }
}