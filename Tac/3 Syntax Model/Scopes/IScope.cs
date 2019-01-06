using Prototypist.LeftToRight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    internal interface ISomeScope {
        bool TryGetMember(IKey name, bool staticOnly, out IBox<IIsPossibly<IWeakMemberDefinition>> box);
    }

    internal interface IPopulatableScope: ISomeScope
    {
        bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<IIsPossibly<WeakMemberDefinition>> type);
        bool TryAddType(IKey name, IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type);
    }

    internal interface IResolvableScope: ISomeScope, IConvertable<IFinalizedScope>
    {
        bool TryGetType(IKey name, out IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type);
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
        internal static IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetTypeOrThrow(this IResolvableScope scope, IKey name) {
            if (scope.TryGetType(name, out var thing)) {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }

        internal static IBox<IIsPossibly<IWeakMemberDefinition>> GetMemberOrThrow(this IResolvableScope scope, IKey name, bool staticOnly)
        {
            if (scope.TryGetMember(name, staticOnly, out var thing))
            {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }


        internal static IIsPossibly<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>> PossiblyGetType(this IResolvableScope scope, IKey name) {
            if (scope.TryGetType(name, out var thing))
            {
                return Possibly.Is(thing);
            }
            return Possibly.Is(thing);
        }


        internal static IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> PossiblyGetMember(this ISomeScope scope, bool staticOnly, IKey name)
        {
            if (scope.TryGetMember(name, staticOnly, out var thing))
            {
                return Possibly.Is(thing);
            }
            return Possibly.Is(thing);
        }
    }
}