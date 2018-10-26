using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedLastCallOperation : WeakLastCallOperation, IInterpeted
    {
        public InterpetedLastCallOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var toCall = left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded();
            var parameter = right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>();


            // maybe there is a "callable" interface here?

            if (toCall is InterpetedMethod method) {
                return InterpetedResult.Create(method.Invoke(interpetedContext,parameter));
            }

            if (toCall is InterpetedImplementation implementation)
            {
                return InterpetedResult.Create(implementation.Invoke(parameter));
            }

            throw new Exception("we can only call things that are callable");

        }

        internal static WeakLastCallOperation MakeNew(IWeakCodeElement left, IWeakCodeElement right)
        {
            return new InterpetedLastCallOperation(left, right);
        }
    }

    public class InterpetedNextCallOperation : WeakNextCallOperation, IInterpeted
    {
        public InterpetedNextCallOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var toCall = right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded();
            var parameter = left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>();


            // maybe there is a "callable" interface here?

            if (toCall is InterpetedMethod method)
            {
                return InterpetedResult.Create(method.Invoke(interpetedContext,parameter));
            }

            if (toCall is InterpetedImplementation implementation)
            {
                return InterpetedResult.Create(implementation.Invoke(parameter));
            }

            throw new Exception("we can only call things that are callable");

        }

        internal static WeakNextCallOperation MakeNew(IWeakCodeElement left, IWeakCodeElement right)
        {
            return new InterpetedNextCallOperation(left, right);
        }
    }
}