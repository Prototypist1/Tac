using Prototypist.LeftToRight;
using System;
using System.Linq;
using System.Reflection;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMember : IInterpetedAnyType { }

    public interface IInterpetedMember<out T> : IInterpetedMember
        where T: IInterpetedAnyType
    {
        T Value { get;  }
        bool TrySet(object o);
    }

    internal static class InterpetedMember {

        internal static IInterpetedMember Make(IVerifiableType type)
        {
            var method = typeof(InterpetedMember).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x =>
              x.Name == nameof(Make) && x.IsGenericMethod);
            var made = method.MakeGenericMethod(new Type[] { TypeMap.MapType(type) });
            return made.Invoke(null,new object[] { }).Cast<IInterpetedMember>();
        }

        private static IInterpetedMember<T> Make<T>()
            where T : IInterpetedAnyType
        {
            return new InterpetedMember<T>();
        }
    }

    internal class InterpetedMember<T> : RunTimeAny, IInterpetedMember<T>
        where T : IInterpetedAnyType
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

        public bool TrySet(object o)
        {
            // the shit always ends up somewhere 💩💩💩

            if (typeof(T).IsAssignableFrom(o.GetType())) {
                Value = o.Cast<T>();
                return true;
            }
            return false;
        }

    }
}