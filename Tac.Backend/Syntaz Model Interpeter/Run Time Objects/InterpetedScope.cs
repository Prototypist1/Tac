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
        private readonly InterpetedStaticScope staticScope;
        private readonly IFinalizedScope finalizedScope;

        public InterpetedScopeTemplate(IFinalizedScope finalizedScope) {
            this.staticScope = InterpetedStaticScope.Make();
            this.finalizedScope = finalizedScope ?? throw new ArgumentNullException(nameof(finalizedScope));
        }

        public IInterpetedScope Create()
        {
            return InterpetedInstanceScope.Make(staticScope, finalizedScope);
        }
    }

    internal static partial class TypeManager
    {
        internal static IInterpetedScope EmptyStaticScope()
        {
            return StaticScope(new ConcurrentIndexed<IKey, IInterpetedMember>());
        }


        public static IInterpetedScope StaticScope(ConcurrentIndexed<IKey, IInterpetedMember> backing)
            => new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { StaticScopeIntention(backing) }).Has<IInterpetedScope>();


        public static Func<RunTimeAnyRoot, IInterpetedScope> StaticScopeIntention(ConcurrentIndexed<IKey, IInterpetedMember> backing)
            => root => new InterpetedStaticScope(backing, root);

        // TODO you are here
        // IInterpetedScope is a pretty big mess
        // objects and modules needs to be an interpeted scope

        // I also need to handle primitive types

        private class InterpetedStaticScope : RootedTypeAny, IInterpetedScope
        {
            public InterpetedStaticScope(ConcurrentIndexed<IKey, IInterpetedMember> backing, RunTimeAnyRoot root) : base(root)
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


        public static IInterpetedScope InstanceScope(InterpetedStaticScope staticBacking,
            IFinalizedScope scopeDefinition)
            => new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { StaticScopeIntention(staticBacking, scopeDefinition) }).Has<IInterpetedScope>();


        public static Func<RunTimeAnyRoot, IInterpetedScope> InstanceScopeIntention(
            InterpetedStaticScope staticBacking,
            IFinalizedScope scopeDefinition)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();


            return (RunTimeAnyRoot root) => {
                var scope = new InterpetedInstanceScope(backing, staticBacking, root);

                foreach (var member in scopeDefinition.Members)
                {
                    backing[member.Key] = InterpetedMember.Make(member.Type);
                }
                return scope;
            };
        }

        public static Func<RunTimeAnyRoot, IInterpetedScope> InstanceScopeIntention(
            IFinalizedScope scopeDefinition)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            return (RunTimeAnyRoot root) =>
            {
                var scope = new InterpetedInstanceScope(backing, InterpetedStaticScope.Empty(), root);

                foreach (var member in scopeDefinition.Members)
                {
                    backing[member.Key] = InterpetedMember.Make(member.Type); ;
                }

                return scope;
            };
        }

        public static Func<RunTimeAnyRoot, IInterpetedScope> InstanceScopeIntention(params (IKey, IInterpetedMember)[] members)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            foreach (var memberKey in members)
            {
                backing[memberKey.Item1] = memberKey.Item2;
            }

            return (RunTimeAnyRoot root) =>
            {
                var scope = new InterpetedInstanceScope(backing, InterpetedStaticScope.Empty(), root);

                return scope;
            };
        }

        private class InterpetedInstanceScope : InterpetedStaticScope
        {

            public InterpetedInstanceScope(ConcurrentIndexed<IKey, IInterpetedMember> backing, InterpetedStaticScope staticBacking, RunTimeAnyRoot root) : base(backing, root)
            {
                StaticBacking = staticBacking ?? throw new ArgumentNullException(nameof(staticBacking));
            }

#pragma warning disable IDE0052 // Remove unread private members
            private InterpetedStaticScope StaticBacking { get; }
#pragma warning restore IDE0052 // Remove unread private members

        }
    }
}
