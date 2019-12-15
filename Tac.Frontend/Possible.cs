using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Frontend
{
    public static class Possibly {

        private class PrivateIs<T> : IIsDefinitely<T>
        {
            public PrivateIs(T value)
            {
                Value = value;
            }

            public T Value {get;}
        }

        private class PrivateIsNot<T> : IIsDefinitelyNot<T>
        {
            public PrivateIsNot(IReadOnlyList<IIsDefinitelyNot> reasons)
            {
                Reasons = reasons ?? throw new ArgumentNullException(nameof(reasons));
            }

            public IReadOnlyList<IIsDefinitelyNot> Reasons { get; }
        }

        public static IIsDefinitely<T> Is<T>(T t) {
            return new PrivateIs<T>(t);
        }


        public static IIsDefinitelyNot<T> IsNot<T>(params IIsDefinitelyNot[] nots)
        {
            return new PrivateIsNot<T>(nots);
        }

    }
    
    public interface IIsPossibly<out T>
    {
    }

    public static class IsPossiblyExtenstions {
        public static bool IsDefinitely<T>(this IIsPossibly<T> self, out IIsDefinitely<T> yes, out IIsDefinitelyNot<T> no) {
            if (self is IIsDefinitely<T> isYes) {
                yes = isYes;
                no = default;
                return true;
            }
            if (self is IIsDefinitelyNot<T> isNo)
            {
                no = isNo;
                yes = default;
                return false;
            }
            throw new Exception("bug! this should be a dichotomy!");
        }

        public static IIsPossibly<TT> IfIs<T, TT>(this IIsPossibly<T> self, Func<T, IIsPossibly<TT>> func) {
            if (self is IIsDefinitely<T> isYes)
            {
                return func(isYes.Value);
            }
            if (self is IIsDefinitelyNot<T> isNo)
            {
                return Possibly.IsNot<TT>(isNo);
            }
            throw new Exception("bug! this should be a dichotomy!");
        }

        public static T GetOrThrow<T>(this IIsPossibly<T> self)
        {
            if (self is IIsDefinitely<T> isYes)
            {
                return isYes.Value;
            }
            throw new Exception();
        }

    }
    
    public interface IIsDefinitely<out T> : IIsPossibly<T>
    {
        T Value { get; }
    }

    public interface IIsDefinitelyNot {

    }

    public interface IIsDefinitelyNot<out T>: IIsPossibly<T>, IIsDefinitelyNot
    {
        IReadOnlyList<IIsDefinitelyNot> Reasons { get; }
    }

    public interface IReason {
        string Message { get; }
    }
}
