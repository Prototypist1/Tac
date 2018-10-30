using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedContext
    {
        private InterpetedContext(IInterpetedScope[] scopes)
        {
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }

        public IReadOnlyList<IInterpetedScope> Scopes { get; }

        public InterpetedContext Child(IInterpetedScope scope)
        {
            var scopes = new List<IInterpetedScope> { scope };
            scopes.AddRange(Scopes);
            return new InterpetedContext(scopes.ToArray());
        }

        public static InterpetedContext Root()
        {
            return new InterpetedContext(new IInterpetedScope[0]);
        }
    }

    public class InterpetedResult
    {
        private InterpetedResult(object value, bool isReturn, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
            IsReturn = isReturn;
        }

        public bool HasValue { get; }
        public bool IsReturn { get; }
        private object Value { get; }

        public T Get<T>()
        {
            if (HasValue)
            {
                throw new Exception($"{nameof(InterpetedResult)} does not have a value");
            }
            return Value.Cast<T>();
        }

        public T GetAndUnwrapMemberWhenNeeded<T>()
        {
            if (HasValue)
            {
                throw new Exception($"{nameof(InterpetedResult)} does not have a value");
            }
            if (Value is InterpetedMember member) {
                return member.Value.Cast<T>();
            }
            return Value.Cast<T>();
        }


        public object GetAndUnwrapMemberWhenNeeded()
        {
            return GetAndUnwrapMemberWhenNeeded<object>();
        }

        public object Get()
        {
            return Get<object>();
        }

        public static InterpetedResult Return(object value)
        {
            return new InterpetedResult(value, true, true);
        }


        public static InterpetedResult Return()
        {
            return new InterpetedResult(null, true, false);
        }


        public static InterpetedResult Create(object value)
        {
            return new InterpetedResult(value, false, true);
        }


        public static InterpetedResult Create()
        {

            return new InterpetedResult(null, false, false);
        }

    }

    internal interface IInterpeted
    {
        InterpetedResult Interpet(InterpetedContext interpetedContext);
    }
}