using Prototypist.Toolbox;
using System;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    public static partial class TypeManager
    {

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> ExternalMethodIntention(Func<IInterpetedAnyType, IInterpetedAnyType> value, IMethodType methodType)
            => root =>
            {
                var item = new InterpetedExternalMethod(value, methodType, root);
                var res = new RunTimeAnyRootEntry(item, methodType);
                return res;
            };

        public static IInterpetedMethod ExternalMethod(Func<IInterpetedAnyType, IInterpetedAnyType> backing, IMethodType methodType)
            => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { ExternalMethodIntention(backing, methodType) }).Has<IInterpetedMethod>();

        private class InterpetedExternalMethod : RootedTypeAny, IInterpetedMethod
        {
            public IMethodType MethodType { get; }

            public InterpetedExternalMethod(Func<IInterpetedAnyType, IInterpetedAnyType> backing, IMethodType methodType, IRunTimeAnyRoot root) : base(root)
            {
                this.Backing = backing ?? throw new ArgumentNullException(nameof(backing));
                this.MethodType = methodType ?? throw new ArgumentNullException(nameof(methodType));
            }

            private Func<IInterpetedAnyType, IInterpetedAnyType> Backing { get; }

            public IInterpetedResult<IInterpetedMember> Invoke(IInterpetedMember input)
            {
                var thing =Backing(input.Value);
                return InterpetedResult.Create(Member(thing.Convert(TransformerExtensions.NewConversionContext()),thing));
            }
        }
    }
}