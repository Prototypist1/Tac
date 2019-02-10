using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMember<T> : IInterpetedData
    {
        T Value { get; set; }
    }

    public static class InterpetedMember{
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
        
    }

    internal class InterpetedMember<T> : IInterpetedMember<T>
        where T : class,IInterpetedData
    {
        private T _value;
        
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