using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedConstantNumber : ConstantNumber, IInterpeted
    {
        public InterpetedConstantNumber(double value) : base(value)
        {
        }
    }
}