using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedPrimitiveType: IPrimitiveType
    {
        IRunTime GetDefault();
    }
}