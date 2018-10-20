using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMemberDefinition: MemberDefinition, IInterpeted
    {
        public InterpetedMemberDefinition(bool readOnly, NameKey key, IBox<IReturnable> type) : base(readOnly, key, type)
        {
        }

        internal static MemberDefinition MakeNew(bool readOnly, NameKey key, IBox<IReturnable> type)
        {
            return new InterpetedMemberDefinition(readOnly, key, type);
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            // TODO, oh this is not so easy, 
            // I need the default value 
            // this comes from IReturnable
            // .. the problem is IReturnable does not even know about IRunTime
            // given that we are in Interpeted land
            // it is safe to assume they are Interpeted
            string s = 5;
            // TOOD this means that 
            // if you create a member of type int
            // it needs to flow throw
            // idk maybe this is a bit crap
            // not sure a type should inturpt in to a instancation of that type
            // but it is reasonible to have some mapping somewhere
            if (Type.GetValue() is IInterpetedPrimitiveType primitiveType) {

            }

            return InterpetedResult.Create(new InterpetedMember(Type.GetValue().Cast<IInterpeted>().Interpet(interpetedContext).Cast<IRunTime>()));
        }
    }
}