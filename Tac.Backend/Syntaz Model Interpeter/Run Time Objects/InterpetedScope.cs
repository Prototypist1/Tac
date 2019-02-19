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

    // TODO you are here
    // IInterpetedScope is a pretty big mess
    // objects and modules needs to be an interpeted scope

    // I also need to handle primitive types

    internal class InterpetedStaticScope : IInterpetedScope
    {
        protected InterpetedStaticScope(ConcurrentIndexed<IKey, IInterpetedMember> backing)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        internal static InterpetedStaticScope Empty()
        {
            return new InterpetedStaticScope(new ConcurrentIndexed<IKey, IInterpetedMember>());
        }

        // yeah, this is a really slow way to do this
        // we should be able to do this with object[]
        private ConcurrentIndexed<IKey, IInterpetedMember> Backing { get; }


        public bool ContainsMember(IKey name)
        {
            return Backing.ContainsKey(name);
        }
        
        
        //public static InterpetedStaticScope Make(IFinalizedScope scopeDefinition)
        //{
        //    var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

        //    var scope = new InterpetedStaticScope(backing);

        //    foreach (var memberKey in scopeDefinition.MemberKeys)
        //    {
        //        backing[memberKey] = new InterpetedMember();
        //    }

        //    return scope;
        //}

        public static InterpetedStaticScope Make()
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            var scope = new InterpetedStaticScope(backing);
            
            return scope;
        }

        public IInterpetedMember<T> GetMember<T>(IKey name) 
        {
            return Backing.GetOrThrow(name).Cast<IInterpetedMember<T>>();
        }

        public bool TryAddMember<T>(IKey key, IInterpetedMember<T> member)
        {
            if (object.ReferenceEquals(member, Backing.GetOrAdd(key, member))){
                return true;
            }
            return false;
        }
    }

    internal class InterpetedInstanceScope: InterpetedStaticScope
    {

        private InterpetedInstanceScope(ConcurrentIndexed<IKey, IInterpetedMember> backing, InterpetedStaticScope staticBacking): base(backing)
        {
            StaticBacking = staticBacking ?? throw new ArgumentNullException(nameof(staticBacking));
        }
        
        private InterpetedStaticScope StaticBacking { get; }


        public static InterpetedInstanceScope Make(
            InterpetedStaticScope staticBacking, 
            IFinalizedScope scopeDefinition) {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            var scope = new InterpetedInstanceScope(backing, staticBacking);
            
            foreach (var member in scopeDefinition.Members)
            {
                backing[member.Key] = new InterpetedMember<object>();
            }

            return scope;
        }


        public static InterpetedInstanceScope Make(
            IFinalizedScope scopeDefinition)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            var scope = new InterpetedInstanceScope(backing, InterpetedStaticScope.Make());

            foreach (var member in scopeDefinition.Members)
            {
                backing[member.Key] = new InterpetedMember<object>();
            }

            return scope;
        }

        public static InterpetedInstanceScope Make(params (IKey, IInterpetedMember)[] members)
        {
            var backing = new ConcurrentIndexed<IKey, IInterpetedMember>();

            foreach (var memberKey in members)
            {
                backing[memberKey.Item1] = memberKey.Item2;
            }

            var scope = new InterpetedInstanceScope(backing, InterpetedStaticScope.Empty());

            return scope;
        }


    }
}
