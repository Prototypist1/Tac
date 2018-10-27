using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{
    public interface IPrimitiveType : IWeakReturnable {
        
    }

    public class StringType : IPrimitiveType { }
    public class EmptyType : IPrimitiveType { }
    public class NumberType : IPrimitiveType { }
    public class AnyType : IPrimitiveType { }
    public class BooleanType : IPrimitiveType { }
}
