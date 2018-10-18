using Tac._3_Syntax_Model.Elements.Atomic_Types;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedAnyType: AnyType
    {
        internal static readonly AnyType.Make MakeNew = ()=> new InterpetedAnyType();
    }
}