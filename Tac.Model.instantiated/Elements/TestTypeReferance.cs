using Tac.Model.Elements;

namespace Tac.Model.instantiated
{
    public class TestTypeReferance : ITypeReferance
    {
        public TestTypeReferance(IVarifiableType typeDefinition)
        {
            TypeDefinition = typeDefinition;
        }

        public IVarifiableType TypeDefinition { get; set; }
    }
}