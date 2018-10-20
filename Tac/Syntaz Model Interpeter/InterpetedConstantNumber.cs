using System;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedConstantNumber : ConstantNumber, IInterpeted
    {
        public InterpetedConstantNumber(double value) : base(value)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeNumber(Value));
        }

        internal static ConstantNumber MakeNew(double value)
        {
            return new InterpetedConstantNumber(value);
        }
    }
}