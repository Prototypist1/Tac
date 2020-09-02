using System;
using Tac.Model.Elements;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedConstantNumber : IInterpetedOperation
    {
        public void Init(double value) {
            this.Value = value;
        }

        public double Value { get; private set; }

        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.NumberMember(TypeManager.Double(Value)));
        }
    }
}