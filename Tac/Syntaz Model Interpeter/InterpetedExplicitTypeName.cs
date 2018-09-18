using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedExplicitTypeName : ExplicitTypeName, IInterpeted
    {
        public InterpetedExplicitTypeName(string name) : base(name)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(this);
        }
    }
}