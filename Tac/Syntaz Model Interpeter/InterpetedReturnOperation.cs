using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedReturnOperation : ReturnOperation, IInterpeted
    {
        public InterpetedReturnOperation(ICodeElement result) : base(result)
        {
        }
    }
}