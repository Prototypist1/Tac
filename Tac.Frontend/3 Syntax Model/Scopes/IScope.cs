using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
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
    // these interface make no sense....
    // coding is so damn hard
    // I wish I had a brain big enough to model all of this
    // 😭

    internal interface ISomeScope {
        bool TryGetMember(IKey name, bool staticOnly, out IBox<IIsPossibly<IWeakMemberDefinition>> box);
    }

    internal interface IPopulatableScope
    {
        bool TryAddGeneric(NameKey key, IBox<IIsPossibly<IFrontendGenericType>> definition);
        bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<IIsPossibly<WeakMemberDefinition>> type);
        bool TryAddType(IKey name, IBox<IIsPossibly<IFrontendType>> type);
        IFinalizableScope GetFinalizableScope();
    }

    internal interface IFinalizableScope{
        IResolvableScope FinalizeScope();
    }

    internal interface IResolvableScope: ISomeScope, IConvertable<IFinalizableScope>
    {
        IEnumerable<IKey> MemberKeys { get; }
        bool TryGetType(IKey name, out IBox<IIsPossibly<IFrontendType>> type);
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
        internal static IBox<IIsPossibly<IFrontendType>> GetTypeOrThrow(this IResolvableScope scope, IKey name) {
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


        internal static IIsPossibly<IBox<IIsPossibly<IFrontendType>>> PossiblyGetType(this IResolvableScope scope, IKey name) {
            if (scope.TryGetType(name, out var thing))
            {
                return Possibly.Is(thing);
            }
            return Possibly.IsNot<IBox<IIsPossibly<IFrontendType>>>();
        }


        internal static IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> PossiblyGetMember(this ISomeScope scope, bool staticOnly, IKey name)
        {
            if (scope.TryGetMember(name, staticOnly, out var thing))
            {
                return Possibly.Is(thing);
            }
            return Possibly.IsNot<IBox<IIsPossibly<IWeakMemberDefinition>>>();
        }
    }
}