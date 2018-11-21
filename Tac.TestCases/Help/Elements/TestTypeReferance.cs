namespace Tac.Model.Elements
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