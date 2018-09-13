using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedImplicitTypeReferance : ImplicitTypeReferance, IInterpeted
    {
        public InterpetedImplicitTypeReferance(ICodeElement codeElement) : base(codeElement)
        {
        }
    }
}