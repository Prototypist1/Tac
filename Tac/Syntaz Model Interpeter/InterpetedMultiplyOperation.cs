﻿using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMultiplyOperation : MultiplyOperation, IInterpeted
    {
        public InterpetedMultiplyOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(
                left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<double>() *
                right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<double>());
        }

        internal static MultiplyOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedMultiplyOperation(left, right);
        }
    }
}