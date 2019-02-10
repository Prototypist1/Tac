using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedGenericTypeDefinition : IInterpetedOperation<IInterpedEmpty>
    {
        public void Init() { }
        
        public IInterpetedResult<IInterpetedMember<IInterpedEmpty>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedMember<IInterpedEmpty>(new RunTimeEmpty()));
        }
    }
}