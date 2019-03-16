﻿using Prototypist.LeftToRight;
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

namespace Tac.Semantic_Model
{
    // I am not really sure this is a useful concept
    internal interface IScoped
    {
        IResolvableScope Scope { get; }
    }

    internal interface IFrontendGenericType : IFrontendType<IGenericType>
    {
        IIsPossibly<GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
        OrType<IFrontendGenericType, IFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters);
    }

    internal class TypeParameter {
        public readonly GemericTypeParameterPlacholder parameterDefinition;
        public readonly IFrontendType<IVerifiableType> frontendType;

        public TypeParameter(GemericTypeParameterPlacholder parameterDefinition, IFrontendType<IVerifiableType> frontendType)
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.frontendType = frontendType ?? throw new ArgumentNullException(nameof(frontendType));
        }
    }

    internal class ScopeTemplate : NewScope //, IFinalizedScopeTemplate
    {
        public ScopeTemplate(Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] typeParameterDefinitions, NewScope parent) : base(parent)
        {
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
            foreach (var item in typeParameterDefinitions)
            {
                // for the sake of validation type parameters are types 
                if (!TryAddType(item.Key, new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(new GemericTypeParameterPlacholder(item.Key))))) {
                    throw new Exception("that is not right!");
                }
            }
        }

        public GemericTypeParameterPlacholder[] TypeParameterDefinitions { get; }

    }

    internal interface IPopulatableResolvableScope : IPopulatableScope, IResolvableScope
    {

    }

    internal class NewScope : IPopulatableResolvableScope
    {

        //internal class AssemblyManager
        //{
        //    private readonly IReadOnlyList<ExteranlResolvableScope> scopes;

        //    public AssemblyManager(IReadOnlyList<IAssembly> assemblies)
        //    {
        //        this.scopes = assemblies.Select(x => new ExteranlResolvableScope(x.Scope)).ToList();
        //    }

        //    public bool TryGetMember(IKey key, bool staticOnly, out IBox<IIsPossibly<IWeakMemberDefinition>> res)
        //    {
        //        var wrappedRes = scopes.Select<ExteranlResolvableScope,IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>>>(x =>
        //        {
        //            if (x.TryGetMember(key, staticOnly, out var someRes))
        //            {
        //                return Possibly.Is(someRes);
        //            }
        //            else
        //            {
        //                return Possibly.IsNot<IBox<IIsPossibly<IWeakMemberDefinition>>>();
        //            }
        //        }).OfType<IIsDefinately<IBox < IIsPossibly < IWeakMemberDefinition >>>>().SingleOrDefault();

        //        if (wrappedRes == null) {
        //            res = default;
        //            return false;
        //        }

        //        res = wrappedRes.Value;
        //        return true;
        //    }

        //    public bool TryGetType(IKey key, out IBox<IIsPossibly<IFrontendType<IVerifiableType>>> res)
        //    {
        //        var wrappedRes = scopes.Select<ExteranlResolvableScope, IIsPossibly<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>>>(x =>
        //        {
        //            if (x.TryGetType(key, out var someRes))
        //            {
        //                return Possibly.Is(someRes);
        //            }
        //            else
        //            {
        //                return Possibly.IsNot<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>>();
        //            }
        //        }).OfType<IIsDefinately<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>>>().SingleOrDefault();

        //        if (wrappedRes == null)
        //        {
        //            res = default;
        //            return false;
        //        }

        //        res = wrappedRes.Value;
        //        return true;
        //    }
            
        //}

        public IPopulatableResolvableScope Parent { get; }

        public IEnumerable<IKey> MemberKeys
        {
            get
            {
                return members.Keys;
            }
        }
        
        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>> members
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>>();

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>>>> types
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>>>>();

        private readonly ConcurrentDictionary<NameKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>> genericTypes
            = new ConcurrentDictionary<NameKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>>();

        public NewScope(IPopulatableResolvableScope parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        // how do I add dependencies?
        // well they are all just in the scope stack
        // NewScope() is root
        // than each dependency stacks on top
        // and then what we are buiding goes on last

        public NewScope()
        {
            // do these really belong here or should they be defined in some sort of 'standard library'
            // here for now I think 
            TryAddType(new NameKey("int"), new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(new NumberType())));
            TryAddType(new NameKey("string"), new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(new StringType())));
            TryAddType(new NameKey("any"), new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(new AnyType())));
            TryAddType(new NameKey("empty"), new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(new EmptyType())));
            TryAddType(new NameKey("bool"), new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(new BooleanType())));
            TryAddGeneric(
                new NameKey("method"),
                new Box<IIsPossibly<IFrontendGenericType>>(Possibly.Is(new GenericMethodType())));
            TryAddGeneric(
                new NameKey("implementation"), new Box<IIsPossibly<IFrontendGenericType>>(Possibly.Is(new GenericImplementationType())));
        }

        public IResolvableScope ToResolvable()
        {
            return this;
        }

        public bool TryAddGeneric(NameKey key, IBox<IIsPossibly<IFrontendGenericType>> definition)
        {
            var list = genericTypes.GetOrAdd(new NameKey(key.Name), new ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>());
            var visiblity = new Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>(DefintionLifetime.Static, definition);
            return list.TryAdd(visiblity);
        }
        
        public bool TryAddMember(DefintionLifetime defintionLifetime, IKey key, IBox<IIsPossibly<WeakMemberDefinition>> definition)
        {
            var list = members.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>());
            var visiblity = new Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        public bool TryAddType(IKey key, IBox<IIsPossibly<IFrontendType<IVerifiableType>>> definition)
        {
            var list = types.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>>>());
            var visiblity = new Visiblity<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>>(DefintionLifetime.Static, definition);
            return list.TryAdd(visiblity);
        }
        
        public bool TryGetMember(IKey name, bool staticOnly, out IBox<IIsPossibly<IWeakMemberDefinition>> member)
        {
            if (!members.TryGetValue(name, out var items))
            {
                goto checkParent;
            }

            var thing = items.SingleOrDefault();

            if (thing == default)
            {
                goto checkParent;
            }

            member = thing.Definition;
            return true;

            checkParent:
            
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

        public bool TryGetType(IKey name, out IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type)
        {
            if (name is GenericNameKey generic)
            {
                if (!genericTypes.TryGetValue(new NameKey(generic.Name), out var set)){goto exit;}
                
                var typesBoxes = generic.Types.Select(x=>
                {
                    TryGetType(x, out var innerTypeBox);
                    if (innerTypeBox == default) {
                        throw new Exception("I guess that is exceptional");
                    }
                    return innerTypeBox;
                }).ToList();

                type = new DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>>(() =>
                {
                    var overlayed = set.Select(x => x.Definition.GetValue())
                        .Where(x => x.IsDefinately(out var _, out var _))
                        .Where(x => x.GetOrThrow().TypeParameterDefinitions.Length == typesBoxes.Count())
                        .Single()
                        .Assign(out var single).GetOrThrow()
                        .Overlay(single.GetOrThrow().TypeParameterDefinitions.Zip(typesBoxes, (x, y) => new TypeParameter(x.GetOrThrow(), y.GetValue().GetOrThrow())).ToArray());
                    if (overlayed.Is(out IFrontendType<IVerifiableType> frontendType)) {
                        return Possibly.Is(frontendType);
                    }
                    if (overlayed.Is(out IFrontendGenericType frontendGeneric))
                    {
                        return Possibly.Is(frontendGeneric);
                    }
                    throw new Exception("the or type should have been IFrontendType<IVarifiableType> or IFrontendGenericType");
                });
                
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
        
        public IBuildIntention<IFinalizedScope> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = Model.Instantiated.Scope.Create();
            return new BuildIntention<IFinalizedScope>(toBuild, () =>
            {
                maker.Build(
                    members.Select(x=>new Tac.Model.Instantiated.Scope.IsStatic(x.Value.Single().Definition.GetValue().GetOrThrow().Convert(context),false)).ToArray(),
                    types.SelectMany(x=> x.Value.Select(y=> new Tac.Model.Instantiated.Scope.TypeData(x.Key,y.Definition.GetValue().GetOrThrow().Convert(context)))).ToList(),
                    genericTypes.SelectMany(x => x.Value.Select(y => new Tac.Model.Instantiated.Scope.GenericTypeData(x.Key, y.Definition.GetValue().GetOrThrow().Convert(context)))).ToList()
                    );
            });
        }
    }
}

