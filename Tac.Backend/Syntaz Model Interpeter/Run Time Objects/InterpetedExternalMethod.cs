using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal static partial class TypeManager
    {

        public static Func<RunTimeAnyRoot, IInterpetedMethod<TIn, TOut>> ExternalMethodIntention<TIn, TOut>(Func<TIn, TOut> value)
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType 
            => root => new InterpetedExternalMethod<TIn, TOut>(value, root);

        public static IInterpetedMethod<TIn, TOut> ExternalMethod<TIn, TOut>(Func<TIn, TOut> backing)
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType 
            => new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { ExternalMethodIntention(backing) }).Has<IInterpetedMethod<TIn, TOut>>();

        private class InterpetedExternalMethod<TIn, TOut> : RootedTypeAny, IInterpetedMethod<TIn, TOut>
        where TIn : IInterpetedAnyType
        where TOut : IInterpetedAnyType
        {
            public InterpetedExternalMethod(Func<TIn, TOut> backing, RunTimeAnyRoot root) : base(root)
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
}