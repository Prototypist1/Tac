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
        public static OrType<TT, IError> Convert<T, TT>(this OrType<T, IError> self, Func<T, TT> transform)
            =>
            self.SwitchReturns(x => new OrType<TT, IError>(transform(x)), y => new OrType<TT, IError>(y));

        public static OrType<T, IError> Flatten<T>(this OrType<OrType<T, IError>, IError> self)
            =>
            self.SwitchReturns(x => x.SwitchReturns(x1=> new OrType<T, IError>(x1), x2 => new OrType<T, IError>(x2)), y => new OrType<T, IError>(y));

    }
}
