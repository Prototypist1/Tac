using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMember<T> : IInterpetedData
    {
        T Value { get; set; }
    }
    
    internal class InterpetedMember<T> : IInterpetedMember<T>
        where T : class,IInterpetedData
    {
        private T _value =null;

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
                _value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

    }
}