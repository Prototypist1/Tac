using Prototypist.LeftToRight;
using System;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public static partial class TypeManager
    {

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> ExternalMethodIntention<TIn, TOut>(Func<TIn, TOut> value, IMethodType methodType)
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType
            => root =>
            {
                var item = new InterpetedExternalMethod<TIn, TOut>(value, methodType, root);
                var res = new RunTimeAnyRootEntry(item, methodType);
                return res;
            };

        public static IInterpetedMethod<TIn, TOut> ExternalMethod<TIn, TOut>(Func<TIn, TOut> backing, IMethodType methodType)
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType 
            => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { ExternalMethodIntention(backing, methodType) }).Has<IInterpetedMethod<TIn, TOut>>();

        private class InterpetedExternalMethod<TIn, TOut> : RootedTypeAny, IInterpetedMethod<TIn, TOut>
        where TIn : IInterpetedAnyType
        where TOut : IInterpetedAnyType
        {
            public IMethodType MethodType { get; }

            public InterpetedExternalMethod(Func<TIn, TOut> backing, IMethodType methodType, IRunTimeAnyRoot root) : base(root)
            {
                this.Backing = backing ?? throw new ArgumentNullException(nameof(backing));
                this.MethodType = methodType ?? throw new ArgumentNullException(nameof(methodType));
            }

            private Func<TIn, TOut> Backing { get; }

            public IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input)
            {
                var thing =Backing(input.Value);
                return InterpetedResult.Create(Member(thing.Convert(TransformerExtensions.NewConversionContext()),thing));
            }
        }
    }
}