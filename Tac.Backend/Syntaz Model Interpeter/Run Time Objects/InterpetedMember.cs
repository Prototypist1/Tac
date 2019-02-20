using Prototypist.LeftToRight;
using System;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMember : IInterpetedData { }

    public interface IInterpetedMember<out T> : IInterpetedMember
    {
        T Value { get; }
    }

    public static class InterpetedMember{
        
    }

    internal class InterpetedMember<T> : IInterpetedMember<T>
    {
        private T _value;

        public InterpetedMember()
        {
        }

        public InterpetedMember(T value)
        {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
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
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _value = value;
            }
        }

    }
}