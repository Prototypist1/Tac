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
    internal interface ISomeScope {
        bool TryGetMember(IKey name, bool staticOnly, out IBox<IIsPossibly<IWeakMemberDefinition>> box);
    }

    internal interface IPopulatableScope: ISomeScope
    {
        bool TryAddGeneric(NameKey key, IBox<IIsPossibly<IFrontendGenericType>> definition);
        bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<IIsPossibly<WeakMemberDefinition>> type);
        bool TryAddType(IKey name, IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type);
    }

    internal interface IResolvableScope: ISomeScope, IConvertable<IFinalizedScope>
    {
        bool TryGetType(IKey name, out IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type);
    }

    //internal class ExteranlResolvableScope : IResolvableScope
    //{
    //    private readonly IFinalizedScope scope;
    //    private readonly ConcurrentIndexed<IMemberDefinition, IWeakMemberDefinition> memberCache = new ConcurrentIndexed<IMemberDefinition, IWeakMemberDefinition>();
    //    private readonly ConcurrentIndexed<ITypeReferance, IWeakTypeReferance> typeReferanceCache = new ConcurrentIndexed<ITypeReferance, IWeakTypeReferance>();
    //    private readonly ConcurrentIndexed<IVerifiableType, IFrontendType<IVerifiableType>> typeCache = new ConcurrentIndexed<IVerifiableType, IFrontendType<IVerifiableType>>();
    //    private readonly ConcurrentIndexed<IGenericType, IFrontendGenericType> genericTypeCache = new ConcurrentIndexed<IGenericType, IFrontendGenericType>();


    //    public ExteranlResolvableScope(IFinalizedScope scope)
    //    {
    //        this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
    //    }

    //    public IBuildIntention<IFinalizedScope> GetBuildIntention(TransformerExtensions.ConversionContext context)
    //    {
    //        return new BuildIntention<IFinalizedScope>(scope, () => { });
    //    }

    //    public bool TryGetMember(IKey name, bool staticOnly, out IBox<IIsPossibly<IWeakMemberDefinition>> box)
    //    {
    //        var member = scope.Members.SingleOrDefault(x => x.Key == name);

    //        if (member == null)
    //        {
    //            box = default;
    //            return false;
    //        }

    //        box = new Box<IIsPossibly<IWeakMemberDefinition>>(Possibly.Is( memberCache.GetOrAdd(member, new ExternalMemberDefinition(
    //            member, Possibly.Is(GetTypeReference(member.Type)), member.ReadOnly, member.Key
    //            ))));
    //        return true;

    //    }

    //    public bool TryGetType(IKey key, out IBox<IIsPossibly<IFrontendType<IVerifiableType>>> res)
    //    {
    //        if (!(key is GenericNameKey generic))
    //        {
    //            goto notGeneric;
    //        }
            
    //        var genericType = scope.GenericTypes.Where(x => x.Key.Name == new NameKey(generic.Name))
    //                .Where(x => x.Type.TypeParameterKeys.Count == generic.Types.Count()).SingleOrDefault();

    //        if (genericType == null)
    //        {
    //            goto notGeneric;
    //        }

    //        if (genericType.Type is IGenericInterfaceDefinition genericInterfaceDefinition)
    //        {
    //            res = new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(genericTypeCache.GetOrAdd(genericType.Type, new ExternalGenericType(generic, genericInterfaceDefinition))));
    //            return true;
    //        }
    //        else {
    //            throw new NotImplementedException($"We do not yet support external generics of type {genericType.GetType().Name}");
    //        }
            
    //        notGeneric:

    //        var type = scope.Types.SingleOrDefault(x => x.Key == key);

    //        if (type == null)
    //        {
    //            res = default;
    //            return false;
    //        }

    //        res = new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(typeCache.GetOrAdd(type.Type, new ExternalTypeDefinition(type.Type))));
    //        return true;
    //    }
        
    //    private IWeakTypeReferance GetTypeReference(ITypeReferance typeReferance)
    //    {
    //        return typeReferanceCache.GetOrAdd(typeReferance, new ExternalTypeReference(GetTypeDefinition(typeReferance.TypeDefinition)));
    //    }

    //    private IFrontendType<IVerifiableType> GetTypeDefinition(IVerifiableType typeDefinition)
    //    {
    //        return typeCache.GetOrAdd(typeDefinition, new ExternalTypeDefinition(typeDefinition));
    //    }
    //}

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