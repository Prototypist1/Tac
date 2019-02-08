using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMember<T> : Run_Time_Objects.IInterpeted
        where T : Run_Time_Objects.IInterpeted
    {
        T Value { get; set; }
    }

    internal class InterpetedMember : IInterpetedMember
    {
        private Run_Time_Objects.IInterpeted _value =null;

        public InterpetedMember()
        {
        }

        public InterpetedMember(Run_Time_Objects.IInterpeted value)
        {
            Value = value;
        }

        public IInterpeted Value
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