using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedType
    {
        IRunTime GetDefault(InterpetedContext interpetedContext);
    }
}