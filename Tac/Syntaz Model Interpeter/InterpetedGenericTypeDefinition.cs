using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedGenericTypeDefinition : GenericTypeDefinition, IInterpeted
    {
        internal static readonly GenericTypeDefinition.Make MakeNew = (key,scope, typeParameterDefinitions) => new InterpetedGenericTypeDefinition(key,scope,typeParameterDefinitions);

        public InterpetedGenericTypeDefinition(NameKey key, IResolvableScope scope, GenericTypeParameterDefinition[] typeParameterDefinitions) : base(key, scope, typeParameterDefinitions)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {

        }
    }
}