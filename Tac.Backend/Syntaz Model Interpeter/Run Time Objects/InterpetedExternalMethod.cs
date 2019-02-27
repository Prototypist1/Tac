using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedExternalMethod<TIn, TOut> : RunTimeAny, IInterpetedMethod<TIn,TOut>
        where TIn: class, IInterpetedAnyType
    {
        public InterpetedExternalMethod(
            Func<TIn, TOut> backing)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        private Func<TIn, TOut> Backing { get; }
        
        public IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input)
        {
            return InterpetedResult.Create(new InterpetedMember<TOut>(Backing(input.Value)));
        }
    }
}