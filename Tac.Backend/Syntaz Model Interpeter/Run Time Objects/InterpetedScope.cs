using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{


    internal class InterpetedScopeTemplate : IInterpetedScopeTemplate
    {
        private readonly IInterpetedStaticScope staticScope;
        private readonly IFinalizedScope finalizedScope;

        public InterpetedScopeTemplate(IFinalizedScope finalizedScope) {
            this.staticScope = TypeManager.StaticScope(new ConcurrentIndexed<IKey, IInterpetedMember>());
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
            return StaticScope(new ConcurrentIndexed<IKey, IInterpetedMember>());
        }


        public static IInterpetedStaticScope StaticScope(ConcurrentIndexed<IKey, IInterpetedMember> backing)
            => Root(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { StaticScopeIntention(backing) }).Has<IInterpetedStaticScope>();


        public static Func<IRunTimeAnyRoot, IInterpetedStaticScope> StaticScopeIntention(ConcurrentIndexed<IKey, IInterpetedMember> backing)
            => root => new InterpetedStaticScope(backing, root);

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
                return Backing.GetOrThrow(name).Cast<IInterpetedMember<T>>();
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
            => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { InstanceScopeIntention(staticBacking, scopeDefinition) }).Has<IInterpetedScope>();

        internal static IInterpetedScope InstanceScope(params (IKey, IInterpetedMember)[] members)
            => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { InstanceScopeIntention(members) }).Has<IInterpetedScope>();


        public static Func<IRunTimeAnyRoot, IInterpetedScope> InstanceScopeIntention(
            IInterpetedStaticScope staticBacking,
            IFinalizedScope scopeDefinition)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();


            return (IRunTimeAnyRoot root) => {
                var scope = new InterpetedInstanceScope(backing, staticBacking, root);

                foreach (var member in scopeDefinition.Members)
                {
                    backing[member.Key] = TypeManager.MakeMember(member.Type);
                }
                return scope;
            };
        }

        public static Func<IRunTimeAnyRoot, IInterpetedScope> InstanceScopeIntention(
            IFinalizedScope scopeDefinition)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            return (IRunTimeAnyRoot root) =>
            {
                var scope = new InterpetedInstanceScope(backing, EmptyStaticScope(), root);

                foreach (var member in scopeDefinition.Members)
                {
                    backing[member.Key] = TypeManager.MakeMember(member.Type); ;
                }

                return scope;
            };
        }

        public static Func<IRunTimeAnyRoot, IInterpetedScope> InstanceScopeIntention(params (IKey, IInterpetedMember)[] members)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            foreach (var memberKey in members)
            {
                backing[memberKey.Item1] = memberKey.Item2;
            }

            return (IRunTimeAnyRoot root) =>
            {
                var scope = new InterpetedInstanceScope(backing, TypeManager.EmptyStaticScope(), root);

                return scope;
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
