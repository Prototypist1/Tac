using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{
    public interface IPrimitiveType : IType
    {
        
    }

    public class StringType : IPrimitiveType, IStringType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.StringType(this);
        }
    }
    public class EmptyType : IPrimitiveType, IEmptyType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.EmptyType(this);
        }
    }
    public class NumberType : IPrimitiveType, INumberType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.NumberType(this);
        }
    }
    public class AnyType : IPrimitiveType, IAnyType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.AnyType(this);
        }
    }
    public class BooleanType : IPrimitiveType, IBooleanType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.BooleanType(this);
        }
    }
}
