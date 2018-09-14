using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedIfTrueOperation : IfTrueOperation, IInterpeted
    {
        public InterpetedIfTrueOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }
}