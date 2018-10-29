﻿using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedAssignOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var res = Right.Interpet(interpetedContext).Get<InterpetedMember>();

            res.Value = Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>();
            return InterpetedResult.Create(res);
        }
    }
}