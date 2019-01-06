using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class TypeReference : ITypeReferance, ITypeReferanceBuilder
    {
        private readonly Buildable<IVerifiableType> buildableTypeDefinition = new Buildable<IVerifiableType>();

        private TypeReference() { }

        public void Build(IVerifiableType typeDefinition)
        {
            buildableTypeDefinition.Set(typeDefinition);
        }

        public IVerifiableType TypeDefinition => buildableTypeDefinition.Get();


        public static (ITypeReferance, ITypeReferanceBuilder) Create()
        {
            var res = new TypeReference();
            return (res, res);
        }

        public IVerifiableType Returns() => this;
        public T Convert<T>(IOpenBoxesContext<T> context) => context.TypeReferance(this);

        public static ITypeReferance CreateAndBuild(IVerifiableType typeDefinition) {
            var (x, y) = Create();
            y.Build(typeDefinition);
            return x;
        }

    }

    public interface ITypeReferanceBuilder
    {
        void Build(IVerifiableType typeDefinition);
    }
}