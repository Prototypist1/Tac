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

        public IVerifiableType VerifiableType => buildableTypeDefinition.Get();


        public static (ITypeReferance, ITypeReferanceBuilder) Create()
        {
            var res = new TypeReference();
            return (res, res);
        }

        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context) where TBacking : IBacking => context.TypeReferance(this);

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