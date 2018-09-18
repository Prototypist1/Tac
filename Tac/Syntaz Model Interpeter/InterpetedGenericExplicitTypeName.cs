using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedGenericExplicitTypeName : GenericExplicitTypeName, IInterpeted
    {
        public InterpetedGenericExplicitTypeName(string name, params ITypeDefinition[] types) : base(name, types)
        {
        }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(this);
        }
    }
}