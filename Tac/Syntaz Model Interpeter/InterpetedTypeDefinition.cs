using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedTypeDefinition: WeakTypeDefinition, IInterpeted, IInterpetedPrimitiveType
    {
        internal static readonly WeakTypeDefinition.Make MakeNew = (scope,key)=> new InterpetedTypeDefinition(scope,key);

        public InterpetedTypeDefinition(IWeakFinalizedScope scope, IKey key) : base(scope, key)
        {
        }

        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RunTimeType();
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeType());
        }
    }
}