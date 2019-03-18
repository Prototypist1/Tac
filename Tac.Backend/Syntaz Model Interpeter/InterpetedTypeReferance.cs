using System;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter
{
    internal class InterpetedTypeReferance : IInterpetedOperation<IInterpetedAnyType>, ITypeReferance
    {
        public IVerifiableType TypeDefinition
        {
            get; private set;
        }
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.TypeReferance(this);
        }

        public IInterpetedResult<IInterpetedMember<IInterpetedAnyType>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create<IInterpetedMember<IInterpetedAnyType>>(new InterpetedMember<IInterpetedAnyType>( new RunTimeType()));
        }

        public IVerifiableType Returns()
        {
            return this;
        }

        internal void Init(IVerifiableType typeDefinition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
        }
    }
}