using Prototypist.LeftToRight;
using System;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMemberDefinition: IInterpeted
    {
        public void Init(IType type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IType Type { get; private set; }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedMember(
                Type.Cast<IInterpetedPrimitiveType>().GetDefault(interpetedContext)));
        }
    }
}