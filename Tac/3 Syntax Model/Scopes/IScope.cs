using Prototypist.LeftToRight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    internal interface ISomeScope {
        bool TryGetMember(IKey name, bool staticOnly, out IBox<IIsPossibly<WeakMemberDefinition>> box);
    }

    internal interface IPopulatableScope: ISomeScope
    {
        bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<IIsPossibly<WeakMemberDefinition>> type);
        bool TryAddType(IKey name, IBox<IIsPossibly<IVarifiableType>> type);
    }

    internal interface IResolvableScope: ISomeScope
    {
        IFinalizedScope GetFinalized();
        bool TryGetType(IKey name, out IBox<IIsPossibly<IVarifiableType>> type);

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
    
    internal static class ResolvableScopeExtension
    {
        internal static IBox<IIsPossibly<IVarifiableType>> GetTypeOrThrow(this IResolvableScope scope, IKey name) {
            if (scope.TryGetType(name, out var thing)) {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }

        internal static IBox<WeakMemberDefinition> GetMemberOrThrow(this IResolvableScope scope, IKey name, bool staticOnly)
        {
            if (scope.TryGetMember(name, staticOnly, out var thing))
            {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }


        internal static IIsPossibly<IBox<IIsPossibly<IVarifiableType>>> PossiblyGetType(this IResolvableScope scope, IKey name) {
            if (scope.TryGetType(name, out var thing))
            {
                return Possibly.Is(thing);
            }
            return Possibly.Is(thing);
        }


        internal static IIsPossibly<IBox<IIsPossibly<WeakMemberDefinition>>> PossiblyGetMember(this ISomeScope scope, bool staticOnly, IKey name)
        {
            if (scope.TryGetMember(name, staticOnly, out var thing))
            {
                return Possibly.Is(thing);
            }
            return Possibly.Is(thing);
        }
    }
}