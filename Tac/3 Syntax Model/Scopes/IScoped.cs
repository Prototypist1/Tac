using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
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

    internal class ScopeTemplate : NewScope //, IFinalizedScopeTemplate
    {
        public ScopeTemplate(IGenericTypeParameterDefinition[] typeParameterDefinitions, NewScope parent):base(parent)
        {
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
            foreach (var item in typeParameterDefinitions)
            {
                // for the sake of validation type parameters are types 

                if (!TryAddType(item.Key, new Box<IFrontendType>(new GemericTypeParameterPlacholder(item.Key)))) {
                    throw new Exception("that is not right!");
                }
            }
        }

        public IGenericTypeParameterDefinition[] TypeParameterDefinitions {get;}

        //public IFinalizedScope CreateScope(GenericTypeParameter[] parameters)
        //{
        //    // ok so maybe types are not so final as I thought and depend on the evaulation context 
        //    // I have IFrontendType maybe that still does a look up, in case the type is generic
        //    // I don't want to have to do any of this copying
        //    // it is a big fat mess 

        //    int i = "Todo";
        //    // add so validation

        //    var res = new NewScope(Parent);

        //    // we need to add everything back
        //    // but we need to replace the place holder types

        //    foreach (var entry in members)
        //    {
        //        foreach (var member in entry.Value)
        //        {
        //            int i = "Todo";
        //            // members probably have types that need to be updated 
        //            if (!res.TryAddMember(member.DefintionLifeTime, entry.Key, member.Definition)) {
        //                throw new Exception("bad bad");
        //            }
        //        }
        //    }

        //    foreach (var entry in types)
        //    {
        //        var type = entry.Value.Single();

        //        var haveKey = parameters.Where(x => x.Parameter.Key.Equals(entry.Key));
        //        if (haveKey.Count()==1)
        //        {
        //            if (!res.TryAddType(entry.Key, new Box<IFrontendType>(haveKey.Single().Type)))
        //            {
        //                throw new Exception("bad bad");
        //            }
        //        }
        //        else {
        //            if (!res.TryAddType(entry.Key, type.Definition))
        //            {
        //                throw new Exception("bad bad");
        //            }
        //        }
        //    }

        //    foreach (var entry in genericTypes)
        //    {
        //        foreach (var genericType in entry.Value)
        //        {
        //            if (!res.TryAddType( entry.Key, genericType.Definition))
        //            {
        //                throw new Exception("bad bad");
        //            }
        //        }
        //    }

        //    return res;
        //}
    }

    internal class NewScope : IPopulatableScope, IResolvableScope, IFinalizedScope
    {
        public NewScope Parent { get; }

        public IEnumerable<IKey> MemberKeys
        {
            get
            {
                return members.Keys;
            }
        }

        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>> members
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>>();

        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IFrontendType>>>> types
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IFrontendType>>>>();

        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IGenericType>>>> genericTypes
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IGenericType>>>>();

        public NewScope(NewScope parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public NewScope()
        {
            TryAddType(new NameKey("int"), new Box<IFrontendType>(new NumberType()));
            TryAddType(new NameKey("string"), new Box<IFrontendType>(new StringType()));
            TryAddType(new NameKey("any"), new Box<IFrontendType>(new AnyType()));
            TryAddType(new NameKey("empty"), new Box<IFrontendType>(new EmptyType()));
            TryAddType(new NameKey("bool"), new Box<IFrontendType>(new BooleanType()));
            // TODO, I need to figure out how method types work
            //
            TryAddGeneric(
                new NameKey("method"),
                new Box<IGenericType>(new GenericMethodType()));
            TryAddGeneric(
                new NameKey("implementation"), new Box<IGenericType>(new GenericImplementationType()));
        }

        public IResolvableScope ToResolvable()
        {
            return this;
        }

        protected bool TryAddGeneric(NameKey key, IBox<IGenericType> definition)
        {
            var list = genericTypes.GetOrAdd(new NameKey(key.Name), new ConcurrentSet<Visiblity<IBox<IGenericType>>>());
            var visiblity = new Visiblity<IBox<IGenericType>>(DefintionLifetime.Static, definition);
            return list.TryAdd(visiblity);
        }

        public bool TryAddMember(DefintionLifetime defintionLifetime, IKey key, IBox<WeakMemberDefinition> definition)
        {
            var list = members.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<WeakMemberDefinition>>>());
            var visiblity = new Visiblity<IBox<WeakMemberDefinition>>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        public bool TryAddType(IKey key, IBox<IFrontendType> definition)
        {
            var list = types.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<IFrontendType>>>());
            var visiblity = new Visiblity<IBox<IFrontendType>>(DefintionLifetime.Static, definition);
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

        public bool TryGetType(IKey name, out IBox<IFrontendType> type)
        {
            if (name is GenericNameKey generic)
            {
                if (!genericTypes.TryGetValue(new NameKey(generic.Name), out var set)){goto exit;}
                
                var typesBoxes = generic.Types.Select(x=>
                {
                    TryGetType(x, out IBox<IFrontendType> innerTypeBox);
                    if (innerTypeBox == default) {
                        throw new Exception("I guess that is exceptional");
                    }
                    return innerTypeBox;
                }).ToList();

                type =  new DelegateBox<IFrontendType>(() => set
                    .Select(single => single.Definition.GetValue())
                    .Where(x => x.TypeParameterDefinitions.Length == typesBoxes.Count())
                    .Single()
                );
                
                return true;
            }
            
            if (!types.TryGetValue(name, out var items)) {goto exit;}

            var thing = items.SingleOrDefault();

            if (thing == default){ goto exit; }

            type = thing.Definition;
            return true;

            // goto 🤘
            exit:
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
        
        public IFinalizedScope GetFinalized()
        {
            return this;
        }

        public bool TryGetMember(IKey name, bool staticOnly, out WeakMemberDefinition res)
        {
            if (TryGetMember(name, staticOnly, out IBox<WeakMemberDefinition> box))
            {
                res = box.GetValue();
                return true;
            }
            res = default;
            return false;
        }

        public bool TryGetType(IKey name, out IFrontendType type)
        {
            if (TryGetType(name, out IBox<IFrontendType> box))
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

