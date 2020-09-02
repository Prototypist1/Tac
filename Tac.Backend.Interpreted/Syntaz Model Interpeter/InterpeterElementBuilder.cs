using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{
    internal class InterpetedContext
    {
        private InterpetedContext(IInterpetedScope[] scopes)
        {
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }

        private IReadOnlyList<IInterpetedScope> Scopes { get; }

        public InterpetedContext Child(IInterpetedScope scope)
        {
            var scopes = new List<IInterpetedScope> { scope };
            scopes.AddRange(Scopes);
            return new InterpetedContext(scopes.ToArray());
        }

        public static InterpetedContext Root()
        {
            return new InterpetedContext(new IInterpetedScope[] { TypeManager.InstanceScope() });
        }

        internal bool TryAddMember(IKey key, IInterpetedMember member)
        {
            return Scopes.First().TryAddMember(key, member);
        }

        internal IInterpetedMember GetMember(IKey key)
        {
            foreach (var item in Scopes)
            {
                if (item.ContainsMember(key)) {
                    return item.GetMember(key);
                }
            }
            throw new Exception($"key not found: {key}");
        }
    }
    
    public interface IInterpetedResult<out T> : IInterpeted
            where T : IInterpetedAnyType
    {
    }
    internal interface IInterpetedResultNotReturn<out T> : IInterpetedResult<T>
        where T : IInterpetedAnyType
    {
        T Value { get; }
    }

    internal interface IInterpetedResultReturn<out T> : IInterpetedResult<T>
        where T : IInterpetedAnyType
    {
        IInterpetedAnyType Value { get; }
    }

    internal static class InterpetedResultExtensions {

        public static IInterpetedResult<IInterpetedMember> Return<T>(this IInterpetedResult<T> self)
            where T : class, IInterpetedAnyType
        {

            if (self is IInterpetedResultNotReturn<T> notReturn)
            {
                return  InterpetedResult.Return<IInterpetedMember>(notReturn.Value);
            }

            if (self is IInterpetedResultReturn<T> toReturn)
            {
                return InterpetedResult.Return<IInterpetedMember>(toReturn.Value);
            }

            throw new Exception("should be one!");
        }


        public static TT IsReturn<T,TT>(this IInterpetedResult<T> self, Func<IInterpetedAnyType,TT> returned, Func<T, TT> value)
            where T : class, IInterpetedAnyType
        {
            if (self is IInterpetedResultNotReturn<T> && self is IInterpetedResultReturn<T>)
            {
                throw new Exception("should not be both!");
            }

            if (self is IInterpetedResultNotReturn<T> notReturn)
            {
                return value(notReturn.Value);
            }

            if (self is IInterpetedResultReturn<T> toReturn)
            {
                return returned(toReturn.Value);
            }

            throw new Exception("should be one!");
        }

        public static bool IsReturn<T>(this IInterpetedResult<T> self, out IInterpetedAnyType? returned, out T? value)
            where T :class, IInterpetedAnyType
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
            where T :  IInterpetedAnyType
        {
            public NotReturn(T value)
            {
                if (value == null) { throw new ArgumentNullException(nameof(value)); }
                Value = value;
            }
            
            public T Value { get; }

        }

        private class IsReturn<T> : IInterpetedResultReturn<T>
            where T :  IInterpetedAnyType
        {
            public IsReturn(IInterpetedAnyType value)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public IInterpetedAnyType Value { get; }

        }
        //public T GetAndUnwrapMemberWhenNeeded(InterpetedContext context)
        //{
        //    if (Value is IInterpetedMember<T> member)
        //    {
        //        return member.Value;
        //    }
        //    if (Value is InterpetedMemberDefinition memberDefinition)
        //    {
        //        return context.GetMember(memberDefinition.Key).Value.CastTo<T>();
        //    }
        //    return Value;
        //}


        public static IInterpetedResultReturn<T> Return<T>()
            where T : IInterpetedAnyType
        {
            return new IsReturn<T>( TypeManager.Empty());
        }

        public static IInterpetedResultReturn<T> Return<T>(IInterpetedAnyType value)
            where T: IInterpetedAnyType
        {
            return new IsReturn<T>(value);
        }
        
        public static IInterpetedResultNotReturn<T> Create<T>(T value)
            where T : IInterpetedAnyType
        {
            return new NotReturn<T>(value);
        }
        
        public static IInterpetedResult<IInterpetedMember> Create()
        {
            return new NotReturn<IInterpetedMember>(TypeManager.EmptyMember(TypeManager.Empty()));
        }
    }

    internal interface IInterpetedOperation
    {
        IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext);
    }
}