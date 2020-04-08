using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Model
{
    public interface IError
    {
    }

    public static class ErrorExtensions
    {
        public static IOrType<TT, IError> TransformInner<T, TT>(this IOrType<T, IError> self, Func<T, TT> transform)
            =>
            self.SwitchReturns(x => OrType.Make<TT, IError>(transform(x)), y => OrType.Make<TT, IError>(y));

        public static IOrType<T, IError> Flatten<T>(this IOrType<IOrType<T, IError>, IError> self)
            =>
            self.SwitchReturns(x => x.SwitchReturns(x1 => OrType.Make<T, IError>(x1), x2 => OrType.Make<T, IError>(x2)), y => OrType.Make<T, IError>(y));

        public static IOrType<TT, IError> TransformAndFlatten<T, TT>(this IOrType<T, IError> self, Func<T, IOrType<TT, IError>> transform) 
            => self.SwitchReturns(x => transform(x), y => OrType.Make<TT, IError>(y));

        public static IOrType<TT, IError> TransformInner<T, TT>(this IOrType<T, IError> self, Func<T, IOrType<TT, IError>> transform){
            return self.SwitchReturns(x => transform(x),x=>OrType.Make<TT, IError>(x));
        }

        public static IOrType<TT, IError> CastToOr<T, TT>(this T self, string errorMessage) where TT:T{
            if (self is TT tt)
            {
                return OrType.Make<TT, IError>(tt);
            }
            else {
                return OrType.Make<TT, IError>(new Error(errorMessage));
            }
        }

    }

    public class Error : IError
    {
        private IError rightResult;

        public Error(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public Error(string message, IError rightResult)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            this.rightResult = rightResult;
        }

        public string Message { get; }
    }
}
