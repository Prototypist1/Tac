using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{

    public class InterpetedStaticScope : IInterpetedScope
    {
        protected InterpetedStaticScope(ConcurrentIndexed<NameKey, InterpetedMember> backing)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        internal static InterpetedStaticScope Empty()
        {
            return new InterpetedStaticScope(new ConcurrentIndexed<NameKey, InterpetedMember>());
        }

        // yeah, this is a really slow way to do this
        // we should be able to do this with object[]
        private ConcurrentIndexed<NameKey, InterpetedMember> Backing { get; }


        public bool ContainsMember(NameKey name)
        {
            return Backing.ContainsKey(name);
        }

        public object GetMember(NameKey name)
        {
            return Backing.GetOrThrow(name).Value;
        }

        public void SetMember<T>(NameKey name, T value)
        {
            Backing[name].Value = value;
        }

        public static InterpetedStaticScope Make(IScope scopeDefinition)
        {
            var backing = new ConcurrentIndexed<NameKey, InterpetedMember>();

            var scope = new InterpetedStaticScope(backing);

            foreach (var member in scopeDefinition.Members)
            {
                backing[member.Key.Key] = new InterpetedMember();
            }

            return scope;
        }
    }

    public class InterpetedInstanceScope: InterpetedStaticScope
    {

        private InterpetedInstanceScope(ConcurrentIndexed<NameKey, InterpetedMember> backing, InterpetedStaticScope staticBacking): base(backing)
        {
            StaticBacking = staticBacking ?? throw new ArgumentNullException(nameof(staticBacking));
        }
        
        private InterpetedStaticScope StaticBacking { get; }


        public static InterpetedInstanceScope Make(InterpetedStaticScope staticBacking, IScope scopeDefinition) {
            var backing = new ConcurrentIndexed<NameKey, InterpetedMember>();

            var scope = new InterpetedInstanceScope(backing, staticBacking);
            
            foreach (var member in scopeDefinition.Members)
            {
                backing[member.Key.Key] = new InterpetedMember();
            }
            
            return scope;
        }

        
    }
}
