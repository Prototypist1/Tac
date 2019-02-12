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
    }
    public interface IInterpetedResultNotReturn<out T> : IInterpetedResult<T>
        where T : IInterpetedData
    {
        T Value { get; }
    }

    public interface IInterpetedResultReturn<out T> : IInterpetedResult<T>
        where T : IInterpetedData
    {
        IInterpetedData Value { get; }
    }

    public static class InterpetedResultExtensions {
        
        public static bool IsReturn<T>(this IInterpetedResult<T> self, out IInterpetedData returned, out T value)
            where T : class, IInterpetedData
        {
            if (self is IInterpetedResultNotReturn<T> && self is IInterpetedResultReturn<T>) {
                throw new Exception("should not be both!");
            }

            returned = default;
            value = default;
            
            if (self is IInterpetedResultNotReturn<T> notReturn)
            {
                value = notReturn.Value;
                return false;
            }

            if (self is IInterpetedResultReturn<T> toReturn)
            {
                returned = toReturn.Value;
                return true;
            }
            
            throw new Exception("should be one!");
        }
        
    }

    internal static class InterpetedResult
    {
        private class NotReturn<T> : IInterpetedResultNotReturn<T>
            where T : class, IInterpetedData
        {
            public NotReturn(T value)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }
            
            public T Value { get; }

        }

        private class IsReturn<T> : IInterpetedResultReturn<T>
            where T : class, IInterpetedData
        {
            public IsReturn(IInterpetedData value)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public IInterpetedData Value { get; }

        }
        //public T GetAndUnwrapMemberWhenNeeded(InterpetedContext context)
        //{
        //    if (Value is IInterpetedMember<T> member)
        //    {
        //        return member.Value;
        //    }
        //    if (Value is InterpetedMemberDefinition memberDefinition)
        //    {
        //        return context.GetMember(memberDefinition.Key).Value.Cast<T>();
        //    }
        //    return Value;
        //}


        public static IInterpetedResultReturn<T> Return<T>()
            where T : class, IInterpetedData
        {
            return new IsReturn<T>(new RunTimeEmpty());
        }

        public static IInterpetedResultReturn<T> Return<T>(IInterpetedData value)
            where T: class,IInterpetedData
        {
            return new IsReturn<T>(value);
        }
        
        public static IInterpetedResultNotReturn<T> Create<T>(T value)
            where T : class, IInterpetedData
        {
            return new NotReturn<T>(value);
        }
        
        public static IInterpetedResult<IInterpetedMember<IInterpedEmpty>> Create()
        {
            return new NotReturn<IInterpetedMember<IInterpedEmpty>>(new InterpetedMember<IInterpedEmpty>(new RunTimeEmpty()));
        }
    }

    internal interface IInterpetedOperation : IInterpeted {
        void Interpet(InterpetedContext interpetedContext);
    }

    internal interface IInterpetedOperation<T>: IInterpetedOperation
    {
        new IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext);
    }
}