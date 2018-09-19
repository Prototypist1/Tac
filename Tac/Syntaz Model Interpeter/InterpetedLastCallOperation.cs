using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedLastCallOperation : LastCallOperation, IInterpeted
    {
        public InterpetedLastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var toCall = left.Cast<IInterpeted>().Interpet(interpetedContext).Get();
            var parameter = right.Cast<IInterpeted>().Interpet(interpetedContext).Get();


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
    }

    public class InterpetedNextCallOperation : NextCallOperation, IInterpeted
    {
        public InterpetedNextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var toCall = right.Cast<IInterpeted>().Interpet(interpetedContext).Get();
            var parameter = left.Cast<IInterpeted>().Interpet(interpetedContext).Get();


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
    }
}