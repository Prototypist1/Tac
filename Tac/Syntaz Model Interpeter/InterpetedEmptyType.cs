using Tac._3_Syntax_Model.Elements.Atomic_Types;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedEmptyType: EmptyType
    {

        internal static readonly EmptyType.Make MakeNew= ()=> new InterpetedEmptyType();
    }
}