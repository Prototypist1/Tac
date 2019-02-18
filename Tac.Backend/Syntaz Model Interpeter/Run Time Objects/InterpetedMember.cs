using Prototypist.LeftToRight;
using System;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMember : IInterpetedData { }

    public interface IInterpetedMember<T> : IInterpetedMember
    {
        T Value { get; set; }
    }

    public static class InterpetedMember{
        // really I would like this to be only reachable 
        // from inside memberDefinition
        // and memberDefinition should onlyexists in an assignment
        public static IInterpetedMember<T> Make<T>()
        {

            if (typeof(T) == typeof(double))
            {
                return new RuntimeNumber(default).Cast<IInterpetedMember<T>>();
            }
            if (typeof(T) == typeof(string))
            {
                return new RunTimeString(default).Cast<IInterpetedMember<T>>();
            }
            if (typeof(T) == typeof(bool))
            {
                return new RunTimeBoolean(default).Cast<IInterpetedMember<T>>();
            }

            if (typeof(IInterpetedData).IsAssignableFrom(typeof(T))  && default(T) == null)
            {
                // 💩💩💩 I seem to have wondered off the edge of the type system
                // problems:
                // 1 - can't cast to class
                // 2 - can't cast to class and IInterpetedData at the same time
                // 3 - can't use the new InterpetedMember<T> because I can't cast T to class,IInterpetedData
                return Activator.CreateInstance(typeof(InterpetedMember<>).MakeGenericType(typeof(T)), false).Cast<IInterpetedMember<T>>();
            }

            throw new Exception("");
        }

        public static IInterpetedMember<T> Make<T>(T t) {
            if (t is double tNum) {
                return new RuntimeNumber(tNum).Cast<IInterpetedMember<T>>();
            }
            if (t is string tString)
            {
                return new RunTimeString(tString).Cast<IInterpetedMember<T>>();
            }
            if (t is bool tBool)
            {
                return new RunTimeBoolean(tBool).Cast<IInterpetedMember<T>>();
            }

            if (t is IInterpetedData && default(T) == null) {
                // 💩💩💩 I seem to have wondered off the edge of the type system
                // problems:
                // 1 - can't cast to class
                // 2 - can't cast to class and IInterpetedData at the same time
                // 3 - can't use the new InterpetedMember<T> because I can't cast T to class,IInterpetedData
                return Activator.CreateInstance(typeof(InterpetedMember<>).MakeGenericType(typeof(T)), false, t).Cast<IInterpetedMember<T>>();
            }

            throw new Exception("");
        }

        // pretty sure I don't need this.
        // just make IInterpetedMember<object> ... 
        internal static IInterpetedMember MakeOfType(IVerifiableType type)
        {
            if (type is INumberType)
            {

            }
            else if (type is IBooleanType)
            {

            }
            else if (type is IStringType)
            {

            }
            else if (type is IMethodType)
            {

            }
            else if (type is IImplementationType) {

            }

            throw new NotImplementedException();
        }
    }

    internal class InterpetedMember<T> : IInterpetedMember<T>
        where T : class,IInterpetedData
    {
        private T _value;

        public InterpetedMember()
        {
        }

        public InterpetedMember(T value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public T Value
        {
            get
            {
                if (_value == null) {
                    throw new Exception($"members must be initialized before they are read");
                }

                return _value;
            }
            set
            {
                _value = value ?? throw new ArgumentNullException(nameof(value)); ;
            }
        }

    }
}