using Prototypist.LeftToRight;
using System;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedMemberDefinition: IInterpetedOperation {
    }
    internal class InterpetedMemberDefinition<T>: IInterpetedOperation<T>, IInterpetedMemberDefinition
    {
        public InterpetedMemberDefinition<T> Init(IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            return this;
        }
        
        public IKey Key { get; private set; }

        public IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext)
        {
            var member = InterpetedMember.Make<T>();

            if (!interpetedContext.TryAddMember(Key, member)) {
                throw new Exception("bed, shit");
            }

            return InterpetedResult.Create(member);
        }
        
        void IInterpetedOperation.Interpet(InterpetedContext interpetedContext)
        {
            Interpet(interpetedContext);
        }
    }
}