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

        public IInterpetedScope Create(InterpetedContext interpetedContext)
        {
            return InterpetedInstanceScope.Make(interpetedContext, staticScope, finalizedScope);
        }
    }

    // TODO you are here
    // IInterpetedScope is a pretty big mess
    // objects and modules needs to be an interpeted scope

    // I also need to handle primitive types

    internal class InterpetedStaticScope : IInterpetedScope
    {
        protected InterpetedStaticScope(ConcurrentIndexed<IKey, InterpetedMember> backing)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        internal static InterpetedStaticScope Empty()
        {
            return new InterpetedStaticScope(new ConcurrentIndexed<IKey, InterpetedMember>());
        }

        // yeah, this is a really slow way to do this
        // we should be able to do this with object[]
        private ConcurrentIndexed<IKey, InterpetedMember> Backing { get; }


        public bool ContainsMember(IKey name)
        {
            return Backing.ContainsKey(name);
        }

        public InterpetedMember GetMember(IKey name)
        {
            return Backing.GetOrThrow(name);
        }
        
        public static InterpetedStaticScope Make(InterpetedContext interpetedContext, IFinalizedScope scopeDefinition)
        {
            var backing = new ConcurrentIndexed<IKey, InterpetedMember>();

            var scope = new InterpetedStaticScope(backing);

            foreach (var memberKey in scopeDefinition.MemberKeys)
            {
                backing[memberKey] = new InterpetedMember();
            }

            return scope;
        }

        public static InterpetedStaticScope Make()
        {
            var backing = new ConcurrentIndexed<IKey, InterpetedMember>();

            var scope = new InterpetedStaticScope(backing);
            
            return scope;
        }
    }

    internal class InterpetedInstanceScope: InterpetedStaticScope
    {

        private InterpetedInstanceScope(ConcurrentIndexed<IKey, InterpetedMember> backing, InterpetedStaticScope staticBacking): base(backing)
        {
            StaticBacking = staticBacking ?? throw new ArgumentNullException(nameof(staticBacking));
        }
        
        private InterpetedStaticScope StaticBacking { get; }


        public static InterpetedInstanceScope Make(
            InterpetedContext interpetedContext, 
            InterpetedStaticScope staticBacking, 
            IFinalizedScope scopeDefinition) {
            var backing = new ConcurrentIndexed<IKey, InterpetedMember>();

            var scope = new InterpetedInstanceScope(backing, staticBacking);
            
            foreach (var memberKey in scopeDefinition.MemberKeys)
            {
                backing[memberKey] = new InterpetedMember();
            }

            return scope;
        }

        public static InterpetedInstanceScope Make(params (IKey, InterpetedMember)[] members)
        {
            var backing = new ConcurrentIndexed<IKey, InterpetedMember>();

            foreach (var memberKey in members)
            {
                backing[memberKey.Item1] = memberKey.Item2;
            }

            var scope = new InterpetedInstanceScope(backing, InterpetedStaticScope.Empty());

            return scope;
        }


    }
}
