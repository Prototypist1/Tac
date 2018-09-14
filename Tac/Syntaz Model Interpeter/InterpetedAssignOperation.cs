using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedAssignOperation : AssignOperation, IInterpeted
    {
        public InterpetedAssignOperation(ICodeElement left, IMemberSource right) : base(left, right)
        {
        }
    }
}