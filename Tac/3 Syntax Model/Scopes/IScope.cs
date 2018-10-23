﻿using Prototypist.LeftToRight;
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
        IFinalizedScope Finalize();
        bool TryGetType(IKey name, out IBox<IReturnable> type);
    }

    public interface IFinalizedScope
    {
        IReadOnlyList<IBox<MemberDefinition>> Members { get; }
    }

    public static class ResolvableScopeExtensions {

        public static IBox<IReturnable> GetType(this IResolvableScope self, IKey name) {
            if (self.TryGetType(name, out var res)) {
                return res;
            }
            throw new Exception($"could not find type: {name}");
        }
        public static IBox<MemberDefinition> GetMember(this IResolvableScope self, bool staticOnly, IKey name) {
            if (self.TryGetMember(name, staticOnly, out var res))
            {
                return res;
            }
            throw new Exception($"could not find member: {name}");
        }
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