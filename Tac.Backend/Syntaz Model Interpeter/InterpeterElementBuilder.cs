using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
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

        internal InterpetedMember GetMember(IKey key)
        {
            foreach (var item in Scopes.Reverse())
            {
                if (item.ContainsMember(key)) {
                    return item.GetMember(key);
                }
            }
            throw new Exception($"key not found: {key}");
        }
    }

    public interface IInterpetedResult<out T> : IInterpeted
            where T : IInterpetedData
    {
        T Value { get; }
        bool IsReturn { get; }
    }
    
    internal class InterpetedResult<T> : IInterpetedResult<T>
            where T : class, IInterpetedData
    {
        private InterpetedResult(T value, bool isReturn)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsReturn = isReturn;
        }
        
        public bool IsReturn { get; }
        public T Value { get; }

        public T GetAndUnwrapMemberWhenNeeded(InterpetedContext context)
        {
            if (Value is IInterpetedMember<T> member)
            {
                return member.Value;
            }
            if (Value is InterpetedMemberDefinition memberDefinition)
            {
                return context.GetMember(memberDefinition.Key).Value.Cast<T>();
            }
            return Value;
        }
        
        public static IInterpetedResult<T> Return(T value)
        {
            return new InterpetedResult<T>(value, true);
        }


        public static IInterpetedResult<IInterpedEmpty> Return()
        {
            return new InterpetedResult<RunTimeEmpty>(new RunTimeEmpty(), true);
        }


        public static IInterpetedResult<T> Create(T value)
        {
            return new InterpetedResult<T>(value, false);
        }


        public static IInterpetedResult<IInterpedEmpty> Create()
        {

            return new InterpetedResult<RunTimeEmpty>(new RunTimeEmpty(), false);
        }
    }
    
    internal interface IInterpetedOperation<T>: IInterpeted
    {
        IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext);
    }
}