using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedTypeDefinition : TypeDefinition, IInterpeted
    {
        public InterpetedTypeDefinition(IScope scope) : base(scope)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return new InterpetedResult(this);
        }
    }
}