using System;
using Tac.Model.Elements;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedConstantBool : IInterpetedOperation
    {
        public void Init(bool value)
        {
            this.Value = value;
        }

        public bool Value { get; private set; }

        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(Value)));
        }
    }
}