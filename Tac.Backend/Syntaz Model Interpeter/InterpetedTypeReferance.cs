using System;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter
{
    internal class InterpetedTypeReferance : IInterpeted, ITypeReferance
    {
        public IVarifiableType TypeDefinition
        {
            get; private set;
        }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.TypeReferance(this);
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeType());
        }

        public IVarifiableType Returns()
        {
            return this;
        }

        internal void Init(IVarifiableType typeDefinition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
        }
    }
}