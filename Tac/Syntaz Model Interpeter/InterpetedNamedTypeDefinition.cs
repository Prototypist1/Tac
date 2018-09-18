using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedNamedTypeDefinition : NamedTypeDefinition, IInterpeted
    {
        public InterpetedNamedTypeDefinition(IKey key, IScope scope) : base(key, scope)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return new InterpetedResult(this);
        }
    }
}