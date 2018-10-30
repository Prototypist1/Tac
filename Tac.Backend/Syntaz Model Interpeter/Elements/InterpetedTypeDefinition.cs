
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedTypeDefinition: IInterpeted, IInterpetedType
    {
        public void Init() { }

        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RunTimeType();
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeType());
        }
    }
}