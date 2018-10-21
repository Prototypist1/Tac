using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedGenericTypeDefinition : GenericTypeDefinition, IInterpeted, IInterpetedPrimitiveType
    {
        internal static readonly GenericTypeDefinition.Make MakeNew = (key,scope, typeParameterDefinitions) => new InterpetedGenericTypeDefinition(key,scope,typeParameterDefinitions);

        public InterpetedGenericTypeDefinition(NameKey key, IResolvableScope scope, GenericTypeParameterDefinition[] typeParameterDefinitions) : base(key, scope, typeParameterDefinitions)
        {
        }

        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RunTimeGenericType();
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeGenericType());
        }
    }
}