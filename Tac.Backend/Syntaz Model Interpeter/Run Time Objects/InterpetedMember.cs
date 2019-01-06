using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMember : IRunTime
    {
        private IRunTime _value =null;

        public InterpetedMember()
        {
        }

        public InterpetedMember(IRunTime value)
        {
            Value = value;
        }

        public IRunTime Value
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