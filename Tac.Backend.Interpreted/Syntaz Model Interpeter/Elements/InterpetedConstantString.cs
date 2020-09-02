using System;
using Tac.Model.Elements;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{
    internal class InterpetedConstantString : IInterpetedOperation
    {
        public void Init(string value)
        {
            this.value = value;
        }

        private string? value;
        public string Value { get => value ?? throw new NullReferenceException(nameof(value)); private set => this.value = value; }

        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.StringMember(TypeManager.String(Value)));
        }
    }
}