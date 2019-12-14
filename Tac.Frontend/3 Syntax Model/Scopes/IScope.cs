//using Prototypist.Toolbox;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Tac.Frontend;
//using Tac.Model;
//using Tac.Model.Elements;
//using Tac.Semantic_Model.CodeStuff;
//using Tac.Semantic_Model.Operations;
//using static Tac.SyntaxModel.Elements.AtomicTypes.PrimitiveTypes;

//namespace Tac.Semantic_Model
//{
//    // these interface make no sense....
//    // coding is so damn hard
//    // I wish I had a brain big enough to model all of this
//    // 😭


//    internal interface IPopulatableScope
//    {
//        bool TryAddGeneric(NameKey key, IBox<IIsPossibly<IFrontendGenericType>> definition);
//        bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<IIsPossibly<WeakMemberDefinition>> type);
//        IMemberBuilder GetOrAddInferedMember(IKey name, IMemberBuilder memberBuilder);
//        bool TryAddType(IKey name, IBox<IIsPossibly<IFrontendType>> type);
//        IResolvelizableScope GetResolvelizableScope();
//        IPopulatableScope AddChild();
//        IPopulatableScope AddGenericChild(IGenericTypeParameterPlacholder[] parameters);
//    }

//    internal interface IResolvelizableScope{
//        IResolvableScope FinalizeScope(IResolvableScope parent);
//        IResolvableScope FinalizeScope();
//    }

//    internal interface IResolvableScope: IConvertable<IFinalizedScope>
//    {
//        bool TryGetMember(IKey name, bool staticOnly, out IBox<IIsPossibly<IWeakMemberDefinition>> box);
//        IEnumerable<IKey> MemberKeys { get; }
//        bool TryGetType(IKey name, out IBox<IIsPossibly<IFrontendType>> type);
//    }

//    public class ScopeEnty<T>
//        where T: class
//    {
//        public readonly IBox<T> element;
//        public readonly IKey key;

//        public ScopeEnty(IBox<T> element, IKey key)
//        {
//            this.element = element ?? throw new ArgumentNullException(nameof(element));
//            this.key = key ?? throw new ArgumentNullException(nameof(key));
//        }
//    }
    
//    internal static class ResolvableScopeExtension
//    {
//        internal static IBox<IIsPossibly<IFrontendType>> GetTypeOrThrow(this IResolvableScope scope, IKey name) {
//            if (scope.TryGetType(name, out var thing)) {
//                return thing;
//            }
//            throw new Exception($"{name} should exist in scope");
//        }

//        internal static IBox<IIsPossibly<IWeakMemberDefinition>> GetMemberOrThrow(this IResolvableScope scope, IKey name, bool staticOnly)
//        {
//            if (scope.TryGetMember(name, staticOnly, out var thing))
//            {
//                return thing;
//            }
//            throw new Exception($"{name} should exist in scope");
//        }


//        internal static IIsPossibly<IBox<IIsPossibly<IFrontendType>>> PossiblyGetType(this IResolvableScope scope, IKey name) {
//            if (scope.TryGetType(name, out var thing))
//            {
//                return Possibly.Is(thing);
//            }
//            return Possibly.IsNot<IBox<IIsPossibly<IFrontendType>>>();
//        }


//        internal static IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> PossiblyGetMember(this IResolvableScope scope, bool staticOnly, IKey name)
//        {
//            if (scope.TryGetMember(name, staticOnly, out var thing))
//            {
//                return Possibly.Is(thing);
//            }
//            return Possibly.IsNot<IBox<IIsPossibly<IWeakMemberDefinition>>>();
//        }
//    }
//}