using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedAssignOperation : AssignOperation, IInterpeted
    {
        public InterpetedAssignOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var res = left.Cast<IInterpeted>().Interpet(interpetedContext);
            right.Cast<IInterpeted>().Interpet(interpetedContext).Get<InterpetedMember>().Value = res;
            return new InterpetedResult(res);
        }
    }
}