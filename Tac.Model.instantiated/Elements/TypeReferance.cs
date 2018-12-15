using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class TypeReferance : ITypeReferance, ITypeReferanceBuilder
    {
        private readonly Buildable<IVarifiableType> buildableTypeDefinition = new Buildable<IVarifiableType>();

        private TypeReferance() { }

        public void Build(IVarifiableType typeDefinition)
        {
            buildableTypeDefinition.Set(typeDefinition);
        }

        public IVarifiableType TypeDefinition => buildableTypeDefinition.Get();


        public static (ITypeReferance, ITypeReferanceBuilder) Create()
        {
            var res = new TypeReferance();
            return (res, res);
        }

    }

    public interface ITypeReferanceBuilder
    {
        void Build(IVarifiableType typeDefinition);
    }
}