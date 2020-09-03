using System;
using Tac.Model;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal interface IInterpetedMemberReferance
    {
        IKey Key { get; }
    }

    internal class InterpetedMemberReferance : IAssembledOperation, IInterpetedMemberReferance
    {
        public InterpetedMemberReferance Init(InterpetedMemberDefinition memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
            return this;
        }

        public IKey Key => MemberDefinition.Key;

        private InterpetedMemberDefinition? memberDefinition;
        public InterpetedMemberDefinition MemberDefinition { get => memberDefinition ?? throw new NullReferenceException(nameof(memberDefinition)); private set => memberDefinition = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {
            return InterpetedResult.Create(interpetedContext.GetMember(MemberDefinition.Key));
        }
    }
}