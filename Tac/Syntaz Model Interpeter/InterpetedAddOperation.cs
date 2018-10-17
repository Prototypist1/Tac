﻿using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedAddOperation : AddOperation, IInterpeted
    {
        public InterpetedAddOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(
                left.Cast<IInterpeted>().Interpet(interpetedContext).Get<double>() +
                right.Cast<IInterpeted>().Interpet(interpetedContext).Get<double>());
        }

        internal static AddOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedAddOperation(left, right);
        }
    }
}