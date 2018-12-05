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
