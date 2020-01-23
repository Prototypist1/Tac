using System;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedMemberReferance<out T>
    {
        IKey Key { get; }
    }

    internal class InterpetedMemberReferance<T> : IInterpetedOperation<T>, IInterpetedMemberReferance<T>
        where T : IInterpetedAnyType
    {
        public InterpetedMemberReferance<T> Init(InterpetedMemberDefinition<T> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
            return this;
        }

        public IKey Key => MemberDefinition.Key;

        private InterpetedMemberDefinition<T>? memberDefinition;
        public InterpetedMemberDefinition<T> MemberDefinition { get => memberDefinition ?? throw new NullReferenceException(nameof(memberDefinition)); private set => memberDefinition = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(interpetedContext.GetMember<T>(MemberDefinition.Key));
        }
    }
}