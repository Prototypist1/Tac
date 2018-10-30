using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedGenericTypeDefinition : IInterpeted, IInterpetedType
    {
        public void Init() { }
        
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RunTimeGenericType();
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeGenericType());
        }
    }
}