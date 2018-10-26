using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{
    public interface IPrimitiveType : IWeakReturnable {
        
    }

    public static class PrimitiveType
    {
        public delegate IPrimitiveType Make();
    }
}
