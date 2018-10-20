﻿using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedLastCallOperation : LastCallOperation, IInterpeted
    {
        public InterpetedLastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var toCall = left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded();
            var parameter = right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>();


            // maybe there is a "callable" interface here?

            if (toCall is InterpetedMethod method) {
                return InterpetedResult.Create(method.Invoke(parameter));
            }

            if (toCall is InterpetedImplementation implementation)
            {
                return InterpetedResult.Create(implementation.Invoke(parameter));
            }

            throw new Exception("we can only call things that are callable");

        }

        internal static LastCallOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedLastCallOperation(left, right);
        }
    }

    public class InterpetedNextCallOperation : NextCallOperation, IInterpeted
    {
        public InterpetedNextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var toCall = right.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded();
            var parameter = left.Cast<IInterpeted>().Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>();


            // maybe there is a "callable" interface here?

            if (toCall is InterpetedMethod method)
            {
                return InterpetedResult.Create(method.Invoke(parameter));
            }

            if (toCall is InterpetedImplementation implementation)
            {
                return InterpetedResult.Create(implementation.Invoke(parameter));
            }

            throw new Exception("we can only call things that are callable");

        }

        internal static NextCallOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedNextCallOperation(left, right);
        }
    }
}