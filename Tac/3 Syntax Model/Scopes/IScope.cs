using Prototypist.LeftToRight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    internal interface ISomeScope {
        bool TryGetMember(IKey name, bool staticOnly, out IBox<WeakMemberDefinition> box);
    }

    internal interface IPopulatableScope: ISomeScope
    {
        bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<WeakMemberDefinition> type);
        bool TryAddType(IKey name, IBox<IVarifiableType> type);
    }

    internal interface IResolvableScope: ISomeScope
    {
        IFinalizedScope GetFinalized();
        bool TryGetType(IKey name, out IBox<IVarifiableType> type);
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

    //internal interface IWeakFinalizedScope: IFinalizedScope
    //{
    //    new IBox<WeakMemberDefinition> this[IKey key] { get; }
    //    new IEnumerable<IKey> MemberKeys { get; }
    //    new IEnumerable<IBox<WeakMemberDefinition>> Values { get; }
    //    new bool ContainsKey(IKey key);
    //    bool TryGetValue(IKey key, out IBox<WeakMemberDefinition> value);
    //}

    //internal class FinalizedScope : IWeakFinalizedScope
    //{
        
    //    private readonly IReadOnlyDictionary<IKey, IBox<WeakMemberDefinition>> Members;

    //    public FinalizedScope(IReadOnlyDictionary<IKey, IBox<WeakMemberDefinition>> members)
    //    {
    //        Members = members ?? throw new ArgumentNullException(nameof(members));
    //    }

    //    public IBox<WeakMemberDefinition> this[IKey key]
    //    {
    //        get
    //        {
    //            return Members[key];
    //        }
    //    }

    //    IMemberDefinition IReadOnlyDictionary<IKey, IMemberDefinition>.this[IKey key]
    //    {
    //        get
    //        {
    //            return Members[key].GetValue();
    //        }
    //    }

    //    public IEnumerable<IKey> MemberKeys
    //    {
    //        get
    //        {
    //            return Members.Keys;
    //        }
    //    }

    //    public IEnumerable<IBox<WeakMemberDefinition>> Values
    //    {
    //        get
    //        {
    //            return Members.Values;
    //        }
    //    }

    //    public int Count
    //    {
    //        get
    //        {
    //            return Members.Count;
    //        }
    //    }

    //    IEnumerable<IMemberDefinition> IReadOnlyDictionary<IKey, IMemberDefinition>.Values
    //    {
    //        get
    //        {
    //            return Members.Values.Select(x => x.GetValue());
    //        }
    //    }

    //    public bool ContainsKey(IKey key)
    //    {
    //        return Members.ContainsKey(key);
    //    }

    //    public IEnumerator<KeyValuePair<IKey, IBox<WeakMemberDefinition>>> GetEnumerator()
    //    {
    //        return Members.GetEnumerator();
    //    }

    //    public bool TryGetValue(IKey key, out IBox<WeakMemberDefinition> value)
    //    {
    //        return Members.TryGetValue(key, out value);
    //    }

    //    public bool TryGetValue(IKey key, out IMemberDefinition value)
    //    {
    //        if (!Members.TryGetValue(key, out var res))
    //        {
    //            value = default;
    //            return false;
    //        }
    //        value = res.GetValue();
    //        return true;
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return Members.GetEnumerator();
    //    }

    //    IEnumerator<KeyValuePair<IKey, IMemberDefinition>> IEnumerable<KeyValuePair<IKey, IMemberDefinition>>.GetEnumerator()
    //    {
    //        return Members.Select(x => new KeyValuePair<IKey, IMemberDefinition>(x.Key, x.Value.GetValue())).GetEnumerator();
    //    }
    //}

    internal static class ResolvableScopeExtension
    {
        internal static IBox<IVarifiableType> GetTypeOrThrow(this IResolvableScope scope, IKey name) {
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
    }
}