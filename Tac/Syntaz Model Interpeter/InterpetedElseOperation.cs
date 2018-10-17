using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedElseOperation : ElseOperation, IInterpeted
    {
        public InterpetedElseOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext) {
            if (!left.Cast<IInterpeted>().Interpet(interpetedContext).Get<bool>())
            {
                right.Cast<IInterpeted>().Interpet(interpetedContext);
                return InterpetedResult.Create(true);
            }
            return InterpetedResult.Create(false);
        }

        internal static ElseOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedElseOperation(left, right);
        }
    }
}