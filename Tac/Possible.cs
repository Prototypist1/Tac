using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Frontend
{
    public static class Possibly {

        private class _Is<T> : IIsDefinately<T>
        {
            public _Is(T value)
            {
                Value = value;
            }

            public T Value {get;}
        }

        private class _IsNot<T> : IIsDefinatelyNot<T>
        {
            public _IsNot(IReadOnlyList<IIsDefinatelyNot> reasons)
            {
                Reasons = reasons ?? throw new ArgumentNullException(nameof(reasons));
            }

            public IReadOnlyList<IIsDefinatelyNot> Reasons { get; }
        }

        public static IIsDefinately<T> Is<T>(T t) {
            return new _Is<T>(t);
        }


        public static IIsDefinatelyNot<T> IsNot<T>(params IIsDefinatelyNot[] nots)
        {
            return new _IsNot<T>(nots);
        }

    }

    public interface IIsPossibly<out T>
    {
    }

    public static class IsPossiblyExtenstions {
        public static bool IsDefinately<T>(this IIsPossibly<T> self, out IIsDefinately<T> yes, out IIsDefinatelyNot<T> no) {
            if (self is IIsDefinately<T> isYes) {
                yes = isYes;
                no = default;
                return true;
            }
            if (self is IIsDefinatelyNot<T> isNo)
            {
                no = isNo;
                yes = default;
                return false;
            }
            throw new Exception("bug! this should be a dichotomy!");
        }

        public static IIsPossibly<TT> IfIs<T, TT>(this IIsPossibly<T> self, Func<T, IIsPossibly<TT>> func) {
            if (self is IIsDefinately<T> isYes)
            {
                return func(isYes.Value);
            }
            if (self is IIsDefinatelyNot<T> isNo)
            {
                return Possibly.IsNot<TT>(isNo);
            }
            throw new Exception("bug! this should be a dichotomy!");
        }

    }
    
    public interface IIsDefinately<out T> : IIsPossibly<T>
    {
        T Value { get; }
    }

    public interface IIsDefinatelyNot {

    }

    public interface IIsDefinatelyNot<out T>: IIsPossibly<T>, IIsDefinatelyNot
    {
        IReadOnlyList<IIsDefinatelyNot> Reasons { get; }
    }

    public interface IReason {
        string Message { get; }
    }
}
