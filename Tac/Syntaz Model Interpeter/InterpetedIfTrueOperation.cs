﻿using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedIfTrueOperation : IfTrueOperation, IInterpeted
    {
        public InterpetedIfTrueOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }


        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            if (left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeBoolean>().b) {
                right.Cast<IInterpeted>().Interpet(interpetedContext);
                return InterpetedResult.Create(new RunTimeBoolean(true));
            }
            return InterpetedResult.Create(new RunTimeBoolean(false));
        }

        internal static IfTrueOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedIfTrueOperation(left, right);
        }
    }
}