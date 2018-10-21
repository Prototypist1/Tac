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
            return InterpetedResult.Create(new InterpetedMember(Type.GetValue().Cast<IInterpetedPrimitiveType>().GetDefault(interpetedContext)));
        }
    }
}