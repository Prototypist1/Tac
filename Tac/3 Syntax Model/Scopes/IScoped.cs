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
                    return Parent.TryGetMember(name, staticOnly, out member);
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
                    return Parent.TryGetMember(name, staticOnly, out member);
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
                    return Parent.TryGetType(name, out type);
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
                    return Parent.TryGetType(name, out type);
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

        public bool TryGetParent(out IFinalizedScope res)
        {
            res = Parent;
            return Parent != null;
        }
    }
    
}

