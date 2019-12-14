using Prototypist.Toolbox;
using Prototypist.TaskChain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Prototypist.Toolbox.Object;

namespace Tac.Syntaz_Model_Interpeter
{


    internal class InterpetedScopeTemplate : IInterpetedScopeTemplate
    {
        private readonly IInterpetedStaticScope staticScope;
        private readonly IFinalizedScope finalizedScope;

        public InterpetedScopeTemplate(IFinalizedScope finalizedScope, IInterfaceModuleType type) {
            this.staticScope = TypeManager.StaticScope(new ConcurrentIndexed<IKey, IInterpetedMember>(), type);
            this.finalizedScope = finalizedScope ?? throw new ArgumentNullException(nameof(finalizedScope));
        }

        public IInterpetedScope Create()
        {
            return TypeManager.InstanceScope(staticScope, finalizedScope);
        }
    }

    public static partial class TypeManager
    {
        internal static IInterpetedStaticScope EmptyStaticScope()
        {
            return StaticScope(new ConcurrentIndexed<IKey, IInterpetedMember>(), InterfaceType.CreateAndBuild(new List<IMemberDefinition>()));
        }

        // TODO, I think the type just passes through here
        // like everywhere else 
        public static IInterpetedStaticScope StaticScope(ConcurrentIndexed<IKey, IInterpetedMember> backing,IInterfaceModuleType type)
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { StaticScopeIntention(backing, type) }).Has<IInterpetedStaticScope>();


        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> StaticScopeIntention(ConcurrentIndexed<IKey, IInterpetedMember> backing, IInterfaceModuleType type)
            => root => new RunTimeAnyRootEntry(new InterpetedStaticScope(backing, root), type);

        // TODO you are here
        // IInterpetedScope is a pretty big mess
        // objects and modules needs to be an interpeted scope

        // I also need to handle primitive types

        private class InterpetedStaticScope : RootedTypeAny, IInterpetedStaticScope
        {
            public InterpetedStaticScope(ConcurrentIndexed<IKey, IInterpetedMember> backing, IRunTimeAnyRoot root) : base(root)
            {
                Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            }



            // yeah, this is a really slow way to do this
            // we should be able to do this with object[]
            private ConcurrentIndexed<IKey, IInterpetedMember> Backing { get; }


            public bool ContainsMember(IKey name)
            {
                return Backing.ContainsKey(name);
            }
        
            public IInterpetedMember<T> GetMember<T>(IKey name) where T : IInterpetedAnyType
            {
                return Backing.GetOrThrow(name).CastTo<IInterpetedMember<T>>();
            }

            public bool TryAddMember<T>(IKey key, IInterpetedMember<T> member) where T : IInterpetedAnyType
            {
                if (object.ReferenceEquals(member, Backing.GetOrAdd(key, member))){
                    return true;
                }
                return false;
            }
        }


        internal static IInterpetedScope InstanceScope(IInterpetedStaticScope staticBacking,
            IFinalizedScope scopeDefinition)
            => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { InstanceScopeIntention(staticBacking, scopeDefinition, scopeDefinition.ToVerifiableType()) }).Has<IInterpetedScope>();

        internal static IInterpetedScope InstanceScope(params (IKey, IInterpetedMember)[] members)
            => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { InstanceScopeIntention(members.Select(x=>(x.Item1,x.Item2.VerifiableType)).ToArray().ToVerifiableType(), members) }).Has<IInterpetedScope>();


        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> InstanceScopeIntention(
            IInterpetedStaticScope staticBacking,
            IFinalizedScope scopeDefinition,
            IVerifiableType type)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();


            return (IRunTimeAnyRoot root) => {
                var scope = new InterpetedInstanceScope(backing, staticBacking, root);

                foreach (var member in scopeDefinition.Members)
                {
                    backing[member.Key] = TypeManager.MakeMember(member.Type);
                }
                return new RunTimeAnyRootEntry(scope, type);
            };
        }

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> InstanceScopeIntention(
            IFinalizedScope scopeDefinition, IVerifiableType type)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            return (IRunTimeAnyRoot root) =>
            {
                var scope = new InterpetedInstanceScope(backing, EmptyStaticScope(), root);

                foreach (var member in scopeDefinition.Members)
                {
                    backing[member.Key] = TypeManager.MakeMember(member.Type); ;
                }

                return new RunTimeAnyRootEntry(scope, type);
            };
        }

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> InstanceScopeIntention(IVerifiableType type,params (IKey, IInterpetedMember)[] members)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            foreach (var memberKey in members)
            {
                backing[memberKey.Item1] = memberKey.Item2;
            }

            return (IRunTimeAnyRoot root) =>
            {
                var scope = new InterpetedInstanceScope(backing, TypeManager.EmptyStaticScope(), root);

                return new RunTimeAnyRootEntry(scope, type);
            };
        }

        private class InterpetedInstanceScope : InterpetedStaticScope
        {

            public InterpetedInstanceScope(ConcurrentIndexed<IKey, IInterpetedMember> backing, IInterpetedStaticScope staticBacking, IRunTimeAnyRoot root) : base(backing, root)
            {
                StaticBacking = staticBacking ?? throw new ArgumentNullException(nameof(staticBacking));
            }

            private IInterpetedStaticScope StaticBacking { get; }

        }
    }
}
