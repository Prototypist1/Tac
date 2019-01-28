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
            
            // TODO
            // maybe there is a "callable" interface here?
            // yeah def

            if (toCall is InterpetedMethod method)
            {
                var res = method.Invoke(parameter);
                if (res.HasValue)
                {
                    return InterpetedResult.Create(res.Get());
                }
                else
                {
                    return InterpetedResult.Create();
                }
            }

            if (toCall is InterpetedImplementation implementation)
            {
                var res = implementation.Invoke(parameter);
                if (res.HasValue)
                {
                    return InterpetedResult.Create(res.Get());
                }
                else
                {
                    return InterpetedResult.Create();
                }
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
                var res = method.Invoke(parameter);
                if (res.HasValue)
                {
                    return InterpetedResult.Create(res.Get());
                }
                else
                {
                    return InterpetedResult.Create();
                }
            }

            if (toCall is InterpetedImplementation implementation)
            {
                var res = implementation.Invoke(parameter);
                if (res.HasValue)
                {
                    return InterpetedResult.Create(res.Get());
                }
                else
                {
                    return InterpetedResult.Create();
                }
            }

            throw new Exception("we can only call things that are callable");

        }
        
    }
}