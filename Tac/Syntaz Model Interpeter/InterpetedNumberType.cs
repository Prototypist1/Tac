using Tac._3_Syntax_Model.Elements.Atomic_Types;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedNumberType: NumberType
    {
        internal static readonly NumberType.Make MakeNew = () => new InterpetedNumberType();
    }
}