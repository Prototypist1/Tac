using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedConstantNumber : ConstantNumber, IInterpeted
    {
        public InterpetedConstantNumber(double value) : base(value)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext) => new InterpetedResult(Value);
    }
}