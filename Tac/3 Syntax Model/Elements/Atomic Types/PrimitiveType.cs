using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{


    public class StringType :  IStringType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.StringType(this);
        }
    }
    public class EmptyType : IEmptyType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.EmptyType(this);
        }
    }
    public class NumberType : INumberType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.NumberType(this);
        }
    }
    public class AnyType : IAnyType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.AnyType(this);
        }
    }
    public class BooleanType : IBooleanType
    {
        public T Convert<T>(ITypeConverter<T> context)
        {
            return context.BooleanType(this);
        }
    }
}
