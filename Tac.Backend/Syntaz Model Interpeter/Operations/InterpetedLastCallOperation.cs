using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedLastCallOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var toCall = Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded(interpetedContext);
            var parameter = Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>(interpetedContext);


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
    }

    internal class InterpetedNextCallOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var toCall = Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded(interpetedContext);
            var parameter = Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>(interpetedContext);


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
        
    }
}