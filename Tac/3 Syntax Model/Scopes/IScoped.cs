using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Parser;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // I am not really sure this is a useful concept
    //
    internal interface IScoped
    {
        IFinalizedScope Scope { get; }
    }

    internal class NewScope : IPopulatableScope, IResolvableScope, IFinalizedScope
    {
        public NewScope Parent { get; }

        public IEnumerable<IKey> MemberKeys => members.Keys;

        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>> members
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>>();

        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IVarifiableType>>>> types
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IVarifiableType>>>>();

        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakGenericTypeDefinition>>>> genericTypes
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakGenericTypeDefinition>>>>();

        public NewScope(NewScope parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public NewScope()
        {
            TryAddType(new NameKey("int"), new Box<IVarifiableType>(new NumberType()));
            TryAddType(new NameKey("string"), new Box<IVarifiableType>(new StringType()));
            TryAddType(new NameKey("any"), new Box<IVarifiableType>(new AnyType()));
            TryAddType(new NameKey("empty"), new Box<IVarifiableType>(new EmptyType()));
            TryAddType(new NameKey("bool"), new Box<IVarifiableType>(new BooleanType()));
        }
        
        public IResolvableScope ToResolvable()
        {
            return this;
        }
        
        protected bool TryAddGeneric(DefintionLifetime defintionLifetime, IKey key, IBox<WeakGenericTypeDefinition> definition)
        {
            var list = genericTypes.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<WeakGenericTypeDefinition>>>());
            var visiblity = new Visiblity<IBox<WeakGenericTypeDefinition>>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }


        public bool TryAddMember(DefintionLifetime defintionLifetime, IKey key, IBox<WeakMemberDefinition> definition)
        {
            var list = members.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>());
            var visiblity = new Visiblity<IBox<WeakMemberDefinition>>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        public bool TryAddType(IKey key, IBox<IVarifiableType> definition)
        {
            var list = types.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<IVarifiableType>>>());
            var visiblity = new Visiblity<IBox<IVarifiableType>>(DefintionLifetime.Static, definition);
            return list.TryAdd(visiblity);
        }

        public bool TryGetMember(IKey name, bool staticOnly, out IBox<WeakMemberDefinition> member)
        {
            if (!members.TryGetValue(name, out var items))
            {
                if (Parent != null)
                {
                    return TryGetMember(name, staticOnly, out member);
                }
                else
                {
                    member = default;
                    return false;
                }
            }

            var thing = items.SingleOrDefault();

            if (thing == default)
            {
                if (Parent != null)
                {
                    return TryGetMember(name, staticOnly, out member);
                }
                else
                {
                    member = default;
                    return false;
                }
            }

            member = thing.Definition;
            return true;
        }

        public bool TryGetType(IKey name, out IBox<IVarifiableType> type)
        {
            if (!types.TryGetValue(name, out var items))
            {
                if (Parent != null)
                {
                    return TryGetType(name, out type);
                }
                else
                {
                    type = default;
                    return false;
                }
            }

            var thing = items.SingleOrDefault();

            if (thing == default)
            {
                if (Parent != null)
                {
                    return TryGetType(name, out type);
                }
                else
                {
                    type = default;
                    return false;
                }
            }

            type = thing.Definition;
            return true;
        }

        public IFinalizedScope GetFinalized()
        {
            return this;
        }

        public bool TryGetMember(IKey name, bool staticOnly, out IMemberDefinition res)
        {
            if (TryGetMember(name, staticOnly, out IBox<WeakMemberDefinition> box)) {
                res = box.GetValue();
                return true;
            }
            res = default;
            return false;
        }

        public bool TryGetType(IKey name, out IVarifiableType type)
        {
            if (TryGetType(name,  out IBox<IVarifiableType> box))
            {
                type = box.GetValue();
                return true;
            }
            type = default;
            return false;
        }
    }

    //internal class ScopeTree
    //{
    //    private class Scope

    //    {

    //        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>> members
    //    = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>>();

    //        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IVarifiableType>>>> types
    //            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IVarifiableType>>>>();


    //        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakGenericTypeDefinition>>>> genericTypes = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakGenericTypeDefinition>>>>();

    //        public bool TryAddMember(DefintionLifetime defintionLifetime, IKey key, IBox<WeakMemberDefinition> definition)
    //        {
    //            var list = members.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>());
    //            var visiblity = new Visiblity<IBox<WeakMemberDefinition>>(defintionLifetime, definition);
    //            return list.TryAdd(visiblity);
    //        }

    //        public bool TryAddType(IKey key, IBox<IVarifiableType> definition)
    //        {
    //            var list = types.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<IVarifiableType>>>());
    //            var visiblity = new Visiblity<IBox<IVarifiableType>>(DefintionLifetime.Static, definition);
    //            return list.TryAdd(visiblity);
    //        }

    //        protected bool TryAddGeneric(DefintionLifetime defintionLifetime, IKey key, IBox<WeakGenericTypeDefinition> definition)
    //        {
    //            var list = genericTypes.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<WeakGenericTypeDefinition>>>());
    //            var visiblity = new Visiblity<IBox<WeakGenericTypeDefinition>>(defintionLifetime, definition);
    //            return list.TryAdd(visiblity);
    //        }

    //        public bool TryGetMember(IKey name, bool staticOnly, out IBox<WeakMemberDefinition> member)
    //        {
    //            if (!members.TryGetValue(name, out var items))
    //            {
    //                member = default;
    //                return false;
    //            }

    //            var thing = items.SingleOrDefault();

    //            if (thing == default)
    //            {
    //                member = default;
    //                return false;
    //            }

    //            member = thing.Definition;
    //            return true;
    //        }

    //        public bool TryGetType(IKey name, out IBox<IVarifiableType> type)
    //        {
    //            if (!types.TryGetValue(name, out var items))
    //            {
    //                type = default;
    //                return false;
    //            }

    //            var thing = items.SingleOrDefault();

    //            if (thing == default)
    //            {
    //                type = default;
    //                return false;
    //            }

    //            type = thing.Definition;
    //            return true;
    //        }

    //        public IWeakFinalizedScope GetFinalized()
    //        {
    //            return new FinalizedScope(members.ToDictionary(x => x.Key, x => x.Value.Single().Definition));
    //        }

    //        public IReadOnlyList<IBox<IVarifiableType>> Types
    //        {
    //            get
    //            {
    //                return types.Select(x => x.Value.Single().Definition).ToArray();
    //            }
    //        }

    //        public IReadOnlyList<IBox<WeakMemberDefinition>> Members
    //        {
    //            get
    //            {
    //                return members.Select(x => x.Value.Single().Definition).ToArray();
    //            }
    //        }
    //    }

    //    private readonly Scope root;

    //    public ScopeTree()
    //    {
    //        root = new Scope();
    //        root.TryAddType(new NameKey("int"), new Box<IVarifiableType>(new NumberType()));
    //        root.TryAddType(new NameKey("string"), new Box<IVarifiableType>(new StringType()));
    //        root.TryAddType(new NameKey("any"), new Box<IVarifiableType>(new AnyType()));
    //        root.TryAddType(new NameKey("empty"), new Box<IVarifiableType>(new EmptyType()));
    //        root.TryAddType(new NameKey("bool"), new Box<IVarifiableType>(new BooleanType()));
    //    }

    //    /// <summary>
    //    /// we only point upwards, say you have:
    //    /// 
    //    /// object A {
    //    ///     x : object B{
    //    ///     
    //    ///     }
    //    /// }
    //    /// 
    //    /// then:
    //    /// 
    //    /// ScopeParent[B] = A;
    //    /// </summary>
    //    private Dictionary<Scope, Scope> ScopeParent { get; } = new Dictionary<Scope, Scope>();

    //    private Scope[] Scopes(Scope scope)
    //    {
    //        return Inner().ToArray();

    //        IEnumerable<Scope> Inner()
    //        {
    //            yield return scope;
    //            while (ScopeParent.ContainsKey(scope))
    //            {
    //                scope = ScopeParent[scope];
    //                yield return scope;
    //            }
    //        }
    //    }

    //    private ScopeTree Add(Scope oldTop, Scope newTop)
    //    {
    //        ScopeParent[newTop] = oldTop;
    //        return this;
    //    }

    //    public class ScopeStack : IPopulatableScope, IResolvableScope
    //    {
    //        public static ScopeStack Root()
    //        {

    //            var scopeTree = new ScopeTree();
    //            return new ScopeStack(scopeTree, scopeTree.root);
    //        }

    //        public ScopeStack ChildScope()
    //        {
    //            var res = new Scope();
    //            ScopeTree.Add(TopScope, res);
    //            return new ScopeStack(ScopeTree, res);
    //        }

    //        private ScopeStack(ScopeTree scopeTree, Scope topScope)
    //        {
    //            ScopeTree = scopeTree ?? throw new ArgumentNullException(nameof(scopeTree));
    //            TopScope = topScope ?? throw new ArgumentNullException(nameof(topScope));
    //        }

    //        public ScopeTree ScopeTree { get; }
    //        private Scope TopScope { get; }

    //        public IBox<IVarifiableType> GetType(IKey key)
    //        {
    //            foreach (var scope in ScopeTree.Scopes(TopScope))
    //            {
    //                if (scope.TryGetType(key, out var typeDefinition))
    //                {
    //                    return typeDefinition;
    //                }
    //            }
    //            throw new Exception("");

    //        }

    //        public bool TryGetMember(IKey name, bool staticOnly, out IBox<WeakMemberDefinition> box)
    //        {
    //            foreach (var scope in ScopeTree.Scopes(TopScope))
    //            {
    //                if (scope.TryGetMember(name, staticOnly, out var memberDefinition))
    //                {
    //                    box = memberDefinition;
    //                    return true;
    //                }
    //            }
    //            box = default;
    //            return false;
    //        }

    //        public bool TryGetType(IKey key, out IBox<IVarifiableType> result)
    //        {
    //            foreach (var scope in ScopeTree.Scopes(TopScope))
    //            {
    //                if (scope.TryGetType(key, out var typeDefinition))
    //                {
    //                    result = typeDefinition;
    //                    return true;
    //                }
    //            }
    //            result = default;
    //            return false;
    //        }

    //        public IResolvableScope ToResolvable()
    //        {
    //            return this;
    //        }

    //        public bool TryAddMember(DefintionLifetime lifeTime, IKey name, IBox<WeakMemberDefinition> member)
    //        {
    //            return TopScope.TryAddMember(lifeTime, name, member);
    //        }

    //        public bool TryAddType(IKey name, IBox<IVarifiableType> type)
    //        {
    //            return TopScope.TryAddType(name, type);
    //        }

    //        public IWeakFinalizedScope GetFinalized()
    //        {
    //            return TopScope.GetFinalized();
    //        }
    //    }
    //}
}

