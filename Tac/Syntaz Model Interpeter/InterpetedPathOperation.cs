﻿using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedPathOperation : PathOperation, IInterpeted
    {
        public InterpetedPathOperation(Member left, PathPart right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = left.Cast<IInterpeted>().Interpet(interpetedContext).Cast<IInterpetedScope>();

            return  InterpetedResult.Create(scope.GetMember(right.Key));
        }

        internal static PathOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedPathOperation(left, right);
        }
    }
}