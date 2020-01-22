﻿using Prototypist.Toolbox;
using System;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedMemberDefinition<out T> : IInterpetedOperation<T>
            where T : IInterpetedAnyType
    {
        IKey Key
        {
            get;
        }
    }

    internal class InterpetedMemberDefinition<T>: IInterpetedMemberDefinition<T>
        where T: IInterpetedAnyType
    {
        public InterpetedMemberDefinition<T> Init(IKey key, IVerifiableType type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            return this;
        }

        private IVerifiableType? type;
        public IVerifiableType Type { get => type ?? throw new NullReferenceException(nameof(type)); private set => type = value ?? throw new NullReferenceException(nameof(value)); }
        private IKey? key;
        public IKey Key { get => key ?? throw new NullReferenceException(nameof(key)); private set => key = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext)
        {
            var member = TypeManager.Member<T>(Type);

            if (!interpetedContext.TryAddMember(Key, member)) {
                throw new Exception("bad, shit");
            }

            return InterpetedResult.Create(member);
        }
    }
}