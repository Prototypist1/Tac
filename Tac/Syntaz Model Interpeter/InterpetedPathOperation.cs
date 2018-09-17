using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : PathOperation, IInterpeted
    {
        public InterpetedPathOperation(ICodeElement left, MemberDefinition right) : base(left, right)
        {
        }
    }
}