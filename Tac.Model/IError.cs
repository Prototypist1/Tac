using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tac.Model
{
    public interface IError
    {
        public Guid Code { get; }
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

        public static IIsPossibly<TT> TransformAndFlatten<T, TT>(this IIsPossibly<T> self, Func<T, IIsPossibly<TT>> transform)
                 => self.IfElseReturn(x => transform(x), () => Possibly.IsNot<TT>());

        public static IOrType<TT, IError> TransformInner<T, TT>(this IOrType<T, IError> self, Func<T, IOrType<TT, IError>> transform) {
            return self.SwitchReturns(x => transform(x), x => OrType.Make<TT, IError>(x));
        }

        public static IOrType<T, IError> IfNotError<T>(this IOrType<T, IError> self, Action<T> transform)
        {
            self.Switch(x => transform(x), x => { });
            return self;
        }


        public static IOrType<TT, IError> OrCastToOr<T, TT>(this IOrType<T, IError> self, IError error)
            where TT : T
        {
            if (self is IOrType<TT, IError> tt)
            {
                return tt;
            }
            else
            {
                return OrType.Make<TT, IError>(error);
            }
        }

        public static IOrType<TT, IError> CastToOr<T, TT>(this T self, IError error)
            where TT : T
        {
            if (self is TT tt)
            {
                return OrType.Make<TT, IError>(tt);
            }
            else
            {
                return OrType.Make<TT, IError>(error);
            }
        }
    }

    public static class ErrorCodes{
        public static readonly Guid TypeNotFound                = Guid.Parse("{BB3C0475-F8CC-4BDC-90B9-4959F14FD1BE}");
        public static readonly Guid Cascaded                    = Guid.Parse("{A22D59DC-088C-433F-BC23-C1797FD5B149}");
        public static readonly Guid Other                       = Guid.Parse("{F465B198-2788-4BF1-B4C0-CDE8FA25FE76}");
        public static readonly Guid AssignmentMustBePossible    = Guid.Parse("{F465B198-2788-4BF1-B4C0-CDE8FA25FE76}");

    }

    public class Error : IError
    {
        public static IError TypeNotFound(string message) => new Error(ErrorCodes.TypeNotFound, message);
        public static IError Other(string message) => new Error(ErrorCodes.Other, message);
        public static IError AssignmentMustBePossible(string message) => new Error(ErrorCodes.AssignmentMustBePossible, message);
        public static IError Cascaded(string message,IError inner) => new Error(ErrorCodes.Cascaded, message, inner);
        public static IError Cascaded(string message, IError[] inner) => new Error(ErrorCodes.Cascaded, message, inner);

        public override string? ToString()
        {
            if (rightResult != null) {
                return string.Join(", ", rightResult.Select(x=>x.ToString()));
            }

            if (Message != null) {
                return Message;
            }

            if (Code != default) {
                return Code.ToString();
            }

            return base.ToString();
        }

        private readonly IError[] rightResult;

        private Error(Guid code) {
            this.Code = code;
        }

        private Error(Guid code,string message): this(code)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        private Error(Guid code, string message, IError rightResult):this(code, message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            this.rightResult = new[] { rightResult };
        }

        private Error(Guid code, string message, IError[] rightResult) : this(code, message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            this.rightResult = rightResult ;
        }

        public string? Message { get; }
        public Guid Code { get; }
    }
}
