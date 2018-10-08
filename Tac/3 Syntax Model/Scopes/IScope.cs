using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IPopulatableScope {
        IResolvableScope ToResolvable();
    }

    public interface IResolvableScope {
        bool TryGetType(IKey name, out IBox<ITypeDefinition> type);
        bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> member);
    }
    
    public static class IIResolvableScopeExtension
    {
        public static IBox<ITypeDefinition> GetTypeOrThrow(this IResolvableScope scope, NameKey name) {
            if (scope.TryGetType(name, out var thing)) {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }
    }
}