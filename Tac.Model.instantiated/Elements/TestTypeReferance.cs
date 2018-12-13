using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class TypeReferance : ITypeReferance
    {
        public TypeReferance(IVarifiableType typeDefinition)
        {
            TypeDefinition = typeDefinition;
        }

        public IVarifiableType TypeDefinition { get; set; }
    }
}