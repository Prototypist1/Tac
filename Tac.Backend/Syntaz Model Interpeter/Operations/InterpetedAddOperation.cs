using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedAddOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeNumber(
                Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeNumber>().d +
                Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeNumber>().d
            ));
        }
    }
}