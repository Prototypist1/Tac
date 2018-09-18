using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedGenericTypeDefinition : GenericTypeDefinition, IInterpeted
    {
        public InterpetedGenericTypeDefinition(NameKey key, ObjectScope scope, GenericTypeParameterDefinition[] typeParameterDefinitions) : base(key, scope, typeParameterDefinitions)
        {
        }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(this);
        }
    }
}