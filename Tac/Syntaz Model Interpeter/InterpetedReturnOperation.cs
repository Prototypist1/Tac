using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedReturnOperation : ReturnOperation, IInterpeted
    {
        public InterpetedReturnOperation(ICodeElement result) : base(result)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Return(Result.Cast<IInterpeted>().Interpet(interpetedContext).Get());
        }

        internal static ReturnOperation MakeNew(ICodeElement left)
        {
            return new InterpetedReturnOperation(left);
        }
    }
}