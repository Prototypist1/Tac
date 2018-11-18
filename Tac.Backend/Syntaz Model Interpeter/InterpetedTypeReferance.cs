using System;
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

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeType());
        }

        internal void Init(IVarifiableType typeDefinition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
        }
    }
}