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
using Tac.Semantic_Model.Operations;
using static Tac._3_Syntax_Model.Elements.Atomic_Types.PrimitiveTypes;

namespace Tac.Semantic_Model
{
    // I am not really sure this is a useful concept
    internal interface IScoped
    {
        IResolvableScope Scope { get; }
    }

    internal interface IFrontendGenericType : IFrontendType
    {
        IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
        OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters);
    }

    internal class TypeParameter {
        public readonly IGenericTypeParameterPlacholder parameterDefinition;
        public readonly IFrontendType frontendType;

        public TypeParameter(IGenericTypeParameterPlacholder parameterDefinition, IFrontendType frontendType)
        {
            this.parameterDefinition = parameterDefinition;
            this.frontendType = frontendType ?? throw new ArgumentNullException(nameof(frontendType));
        }
    }

    internal class PopulatableScopeTemplate : PopulatableScope {
        public PopulatableScopeTemplate(IGenericTypeParameterPlacholder[] typeParameterDefinitions, PopulatableScope parent) : base(parent)
        {
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
            foreach (var item in typeParameterDefinitions)
            {
                // for the sake of validation type parameters are types 
                if (!TryAddType(item.Key, new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateGenericTypeParameterPlacholder(item.Key)))))
                {
                    throw new Exception("that is not right!");
                }
            }
        }

        public IGenericTypeParameterPlacholder[] TypeParameterDefinitions { get; }
    }

    internal class PopulatableScope : IPopulatableScope
    {
        private readonly ConcurrentDictionary<IKey,  ConcurrentSet<Tuple<DefintionLifetime, IBox<IIsPossibly<WeakMemberDefinition>>>>> members
            = new ConcurrentDictionary<IKey,  ConcurrentSet<Tuple<DefintionLifetime, IBox<IIsPossibly<WeakMemberDefinition>>>>>();

        private readonly ConcurrentDictionary<IKey, IMemberBuilder> inferedMembers
            = new ConcurrentDictionary<IKey, IMemberBuilder>();

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<IBox<IIsPossibly<IFrontendType>>>> types
            = new ConcurrentDictionary<IKey, ConcurrentSet<IBox<IIsPossibly<IFrontendType>>>>();

        private readonly ConcurrentDictionary<NameKey, ConcurrentSet<IBox<IIsPossibly<IFrontendGenericType>>>> genericTypes
            = new ConcurrentDictionary<NameKey, ConcurrentSet<IBox<IIsPossibly<IFrontendGenericType>>>>();

        public IIsPossibly<PopulatableScope> Parent { get; }

        private readonly FinalizableScope finalizableScope;

        public PopulatableScope(PopulatableScope parent) {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = Possibly.Is(parent);
            finalizableScope = new FinalizableScope(this);
        }


        public PopulatableScope()
        {

            this.Parent = Possibly.IsNot<PopulatableScope>();
            finalizableScope = new FinalizableScope(this);

            // do these really belong here or should they be defined in some sort of 'standard library'
            // here for now I think 
            TryAddType(new NameKey("int"), new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateNumberType())));
            TryAddType(new NameKey("string"), new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateStringType())));
            TryAddType(new NameKey("any"), new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateAnyType())));
            TryAddType(new NameKey("empty"), new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateEmptyType())));
            TryAddType(new NameKey("bool"), new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateBooleanType())));
            TryAddGeneric(
                new NameKey("method"),
                new Box<IIsPossibly<IFrontendGenericType>>(Possibly.Is<IFrontendGenericType>(PrimitiveTypes.CreateGenericMethodType())));
            TryAddGeneric(
                new NameKey("implementation"), new Box<IIsPossibly<IFrontendGenericType>>(Possibly.Is<IFrontendGenericType>(PrimitiveTypes.CreateGenericImplementationType())));
        }



        public bool TryAddGeneric(NameKey key, IBox<IIsPossibly<IFrontendGenericType>> definition)
        {
            if (finalizableScope.IsFinal)
            {
                throw new ApplicationException("bug: don't add after finalize");
            }
            var list = genericTypes.GetOrAdd(new NameKey(key.Name), new ConcurrentSet<IBox<IIsPossibly<IFrontendGenericType>>>());
            return list.TryAdd(definition);
        }

        public bool TryAddMember(DefintionLifetime defintionLifetime, IKey key, IBox<IIsPossibly<WeakMemberDefinition>> definition)
        {
            if (finalizableScope.IsFinal)
            {
                throw new ApplicationException("bug: don't add after finalize");
            }
            var list = members.GetOrAdd(key, new ConcurrentSet<Tuple<DefintionLifetime, IBox<IIsPossibly<WeakMemberDefinition>>>>());
            return list.TryAdd(new Tuple<DefintionLifetime, IBox<IIsPossibly<WeakMemberDefinition>>>(defintionLifetime,definition));
        }


        public IMemberBuilder GetOrAddInferedMember(IKey name, IMemberBuilder memberBuilder)
        {
            if (finalizableScope.IsFinal)
            {
                throw new ApplicationException("bug: don't add after finalize");
            }
            return inferedMembers.GetOrAdd(memberBuilder.Key, memberBuilder);
        }

        public bool TryAddType(IKey key, IBox<IIsPossibly<IFrontendType>> definition)
        {
            if (finalizableScope.IsFinal)
            {
                throw new ApplicationException("bug: don't add after finalize");
            }
            var list = types.GetOrAdd(key, new ConcurrentSet<IBox<IIsPossibly<IFrontendType>>>());
            return list.TryAdd(definition);
        }

        protected class FinalizableScope: IResolvelizableScope{
            private readonly PopulatableScope owner;
            public bool IsFinal { get; private set; } = false;

            public FinalizableScope(PopulatableScope owner) {
                this.owner = owner;
            }

            // these two methods are frustratingly similar 
            public IResolvableScope FinalizeScope()
            {
                if (owner.Parent.IsDefinately(out var _, out var _)) {
                    throw new ApplicationException("bug: this scope has a parent, you can't use FinalizeScope() for it");
                }

                if (IsFinal)
                {
                    throw new ApplicationException("bug: don't finalize twice");
                }

                IsFinal = true;

                var resolvaleMembers = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>>();
                foreach (var memberCollection in owner.members)
                {
                    // TOOD this is a whole project
                    // we need to get the error to the right place if they tried to add more than one
                    var list = resolvaleMembers.GetOrAdd(memberCollection.Key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>());

                    var member = memberCollection.Value.First();
                    list.TryAdd(new Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>(member.Item1, member.Item2));
                }

                foreach (var member in owner.inferedMembers)
                {
                    if (!resolvaleMembers.TryGetValue(member.Key, out var _))
                    {
                        var list = resolvaleMembers.GetOrAdd(member.Key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>());
                        list.TryAdd(new Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>(DefintionLifetime.Instance, member.Value.Build()));
                    }
                }

                var resolvableTypes = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType>>>>>();

                foreach (var pair in owner.types)
                {
                    var list = resolvableTypes.GetOrAdd(pair.Key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType>>>>());
                    foreach (var item in pair.Value)
                    {
                        list.TryAdd(new Visiblity<IBox<IIsPossibly<IFrontendType>>>(DefintionLifetime.Static, item));
                    }
                }

                var resolvableGenericTypes = new ConcurrentDictionary<NameKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>>();

                foreach (var pair in owner.genericTypes)
                {
                    var list = resolvableGenericTypes.GetOrAdd(pair.Key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>());
                    foreach (var item in pair.Value)
                    {
                        list.TryAdd(new Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>(DefintionLifetime.Static, item));
                    }
                }

                return new ResolvableScope(resolvaleMembers, resolvableTypes, resolvableGenericTypes);
            }

            public IResolvableScope FinalizeScope(IResolvableScope parent)
            {
                if (!owner.Parent.IsDefinately(out var _, out var _))
                {
                    throw new ApplicationException("bug: this scope does not have a parent, you can't use FinalizeScope(IResolvableScope parent) for it");
                }

                if (IsFinal) {
                    throw new ApplicationException("bug: don't finalize twice");
                }

                IsFinal = true;

                var resolvaleMembers = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>>();
                foreach (var memberCollection in owner.members)
                {
                    if (!resolvaleMembers.TryGetValue(memberCollection.Key,out var _))
                    {
                        var list = resolvaleMembers.GetOrAdd(memberCollection.Key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>());

                        // TODO
                        // we can't just take the first
                        // most of these are probably the same varialbe
                        // check if the types are compatible 
                        var member = memberCollection.Value.First();
                        list.TryAdd(new Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>(member.Item1, member.Item2));
                    }
                    else { 
                        // TODO validate the definitions are compatible 
                    }
                }

                foreach (var member in owner.inferedMembers)
                {
                    if (!resolvaleMembers.TryGetValue(member.Key, out var _) && !parent.TryGetMember(member.Key,false,out var _))
                    {
                        var list = resolvaleMembers.GetOrAdd(member.Key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>());
                        list.TryAdd(new Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>(DefintionLifetime.Instance, member.Value.Build()));
                    }
                }

                var resolvableTypes = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType>>>>>();

                foreach (var pair in owner.types)
                {
                    var list = resolvableTypes.GetOrAdd(pair.Key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType>>>>());
                    foreach (var item in pair.Value)
                    {
                        list.TryAdd(new Visiblity<IBox<IIsPossibly<IFrontendType>>>(DefintionLifetime.Static, item));
                    }
                }

                var resolvableGenericTypes = new ConcurrentDictionary<NameKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>>();

                foreach (var pair in owner.genericTypes)
                {
                    var list = resolvableGenericTypes.GetOrAdd(pair.Key, new ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>());
                    foreach (var item in pair.Value)
                    {
                        list.TryAdd(new Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>(DefintionLifetime.Static, item));
                    }
                }

                return new ResolvableScope(parent, resolvaleMembers, resolvableTypes, resolvableGenericTypes);
            }
        }


        public IResolvelizableScope GetResolvelizableScope()
        {
            return finalizableScope;
        }

        public IPopulatableScope AddChild()
        {
            return new PopulatableScope(this);
        }

        public IPopulatableScope AddGenericChild(IGenericTypeParameterPlacholder[] parameters)
        {
            return new PopulatableScopeTemplate(parameters,this);
        }

    }

    internal class ResolvableScope : IResolvableScope
    {

        public  IIsPossibly<IResolvableScope> Parent { get; }

        public IEnumerable<IKey> MemberKeys
        {
            get
            {
                return members.Keys;
            }
        }

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>> members;

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType>>>>> types;

        private readonly ConcurrentDictionary<NameKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>> genericTypes;

        public ResolvableScope(IResolvableScope parent, ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>> members,
             ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType>>>>> types,
              ConcurrentDictionary<NameKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>> genericTypes)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = Possibly.Is( parent) ;
            this.members = members ?? throw new ArgumentNullException(nameof(members));
            this.types = types ?? throw new ArgumentNullException(nameof(types));
            this.genericTypes = genericTypes ?? throw new ArgumentNullException(nameof(genericTypes));
        }

        public ResolvableScope(
            ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<WeakMemberDefinition>>>>> members,
            ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendType>>>>> types,
            ConcurrentDictionary<NameKey, ConcurrentSet<Visiblity<IBox<IIsPossibly<IFrontendGenericType>>>>> genericTypes)
        {
            Parent = Possibly.IsNot<IResolvableScope>();
            this.members = members ?? throw new ArgumentNullException(nameof(members));
            this.types = types ?? throw new ArgumentNullException(nameof(types));
            this.genericTypes = genericTypes ?? throw new ArgumentNullException(nameof(genericTypes));
        }

        // how do I add dependencies?
        // well they are all just in the scope stack
        // NewScope() is root
        // than each dependency stacks on top
        // and then what we are buiding goes on last

        // yeah.. that is not right at all
        // the reference stuff have a name right? so they just exist in the base scope under their name

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
            
            if (Parent is IIsDefinately<IResolvableScope> resolvbaleParent)
            {
                return resolvbaleParent.Value.TryGetMember(name, staticOnly, out member);
            }
            else
            {
                member = default;
                return false;
            }
        }

        public bool TryGetType(IKey name, out IBox<IIsPossibly<IFrontendType>> type)
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

                type = new DelegateBox<IIsPossibly<IFrontendType>>(() =>
                {
                    var overlayed = set.Select(x => x.Definition.GetValue())
                        .Where(x => x.IsDefinately(out var _, out var _))
                        .Where(x => x.GetOrThrow().TypeParameterDefinitions.Length == typesBoxes.Count())
                        .Single()
                        .Assign(out var single).GetOrThrow()
                        .Overlay(single.GetOrThrow().TypeParameterDefinitions.Zip(typesBoxes, (x, y) => new TypeParameter(x.GetOrThrow(), y.GetValue().GetOrThrow())).ToArray());
                    if (overlayed.Is(out IConvertableFrontendType<IVerifiableType> frontendType)) {
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

            if (Parent is IIsDefinately<IResolvableScope> resolvbaleParent)
            {
                var res = resolvbaleParent.Value.TryGetType(name, out var theType);
                type = theType;
                return res;
            }
            else
            {
                type = default;
                return false;
            }
        }
        
        public IBuildIntention<IFinalizedScope> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = Model.Instantiated.Scope.Create();
            return new BuildIntention<IFinalizedScope>(toBuild, () =>
            {
                maker.Build(
                    members.Select(x=>new Tac.Model.Instantiated.Scope.IsStatic(x.Value.Single().Definition.GetValue().GetOrThrow().Convert(context),false)).ToArray());
            });
        }
    }
}

