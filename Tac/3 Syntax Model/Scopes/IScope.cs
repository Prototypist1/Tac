using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface ISomeScope {
        bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> box);
    }

    public interface IPopulatableScope: ISomeScope
    {
        IResolvableScope ToResolvable();

        bool TryAddMember(IKey name, IBox<MemberDefinition> type);
        bool TryAddType(IKey name, IBox<IReturnable> type);
    }

    public interface IResolvableScope: ISomeScope
    {
        IReadOnlyList<IBox<MemberDefinition>> Members { get; }

        bool TryGetType(IKey name, out IBox<IReturnable> type);
    }
    
    public static class IIResolvableScopeExtension
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