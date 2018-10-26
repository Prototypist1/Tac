using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedReturnOperation : WeakReturnOperation, IInterpeted
    {
        public InterpetedReturnOperation(IWeakCodeElement result) : base(result)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Return(Result.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded());
        }

        internal static WeakReturnOperation MakeNew(IWeakCodeElement left)
        {
            return new InterpetedReturnOperation(left);
        }
    }
}