using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedLastCallOperation : InterpetedBinaryOperation<IInterpetedData,IInterpetedData,IInterpetedData>
    {
        public override IInterpetedResult<IInterpetedData> Interpet(InterpetedContext interpetedContext)
        {
            var toCall = Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded(interpetedContext);
            var parameter = Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<Run_Time_Objects.IInterpeted>(interpetedContext);
            
            // TODO
            // maybe there is a "callable" interface here?
            // yeah def

            if (toCall is InterpetedMethod method)
            {
                var res = method.Invoke(parameter);
                if (res.HasValue)
                {
                    return InterpetedResult<IInterpetedData>.Create(res.Get());
                }
                else
                {
                    return InterpetedResult<IInterpetedData>.Create();
                }
            }

            if (toCall is InterpetedImplementation implementation)
            {
                var res = implementation.Invoke(parameter);
                if (res.HasValue)
                {
                    return InterpetedResult<IInterpetedData>.Create(res.Get());
                }
                else
                {
                    return InterpetedResult<IInterpetedData>.Create();
                }
            }

            throw new Exception("we can only call things that are callable");

        }
    }

    internal class InterpetedNextCallOperation : InterpetedBinaryOperation<IInterpetedData,IInterpetedData,IInterpetedData>
    {
        public override IInterpetedResult<IInterpetedData> Interpet(InterpetedContext interpetedContext)
        {
            var toCall = Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded(interpetedContext);
            var parameter = Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IInterpetedData>(interpetedContext);


            // maybe there is a "callable" interface here?

            if (toCall is InterpetedMethod method)
            {
                var res = method.Invoke(parameter);
                if (res.HasValue)
                {
                    return InterpetedResult<IInterpetedData>.Create(res.Get());
                }
                else
                {
                    return InterpetedResult<IInterpetedData>.Create();
                }
            }

            if (toCall is InterpetedImplementation implementation)
            {
                var res = implementation.Invoke(parameter);
                if (res.HasValue)
                {
                    return InterpetedResult<IInterpetedData>.Create(res.Get());
                }
                else
                {
                    return InterpetedResult<IInterpetedData>.Create();
                }
            }

            throw new Exception("we can only call things that are callable");

        }
        
    }
}