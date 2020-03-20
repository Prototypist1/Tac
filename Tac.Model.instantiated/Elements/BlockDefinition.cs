using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class BlockDefinition : IBlockDefinition, IBlockDefinitionBuilder
    {
        private readonly Buildable<IEnumerable<ICodeElement>> buildableStaticInitailizers = new Buildable<IEnumerable<ICodeElement>>();
        private readonly Buildable<IReadOnlyList<OrType<ICodeElement, IError>>> buildableBody = new Buildable<IReadOnlyList<OrType<ICodeElement, IError>>>();
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();

        private BlockDefinition() 
        {
        }

        #region IBlockDefinition

        public IFinalizedScope Scope { get => buildableScope.Get(); }
        public IReadOnlyList< OrType<ICodeElement, IError>> Body { get => buildableBody.Get(); }
        public IEnumerable<ICodeElement> StaticInitailizers { get => buildableStaticInitailizers.Get(); }
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.BlockDefinition(this);
        }
        public IVerifiableType Returns()
        {
            return new EmptyType();
        }
        
        #endregion

        #region IBlockDefinitionBuilder

        public void Build(IFinalizedScope scope, OrType<ICodeElement, IError>[] body, IEnumerable<ICodeElement> staticInitailizers)
        {
            buildableScope.Set(scope);
            buildableBody.Set(body);
            buildableStaticInitailizers.Set(staticInitailizers);
        }

        #endregion

        #region static

        public static (IBlockDefinition, IBlockDefinitionBuilder) Create()
        {
            var res = new BlockDefinition();
            return (res, res);
        }

        public static IBlockDefinition CreateAndBuild(IFinalizedScope scope, OrType<ICodeElement, IError>[] body, IEnumerable<ICodeElement> staticInitailizers) {
            var (x, y) = Create();
            y.Build(scope, body, staticInitailizers);
            return x;
        }

        #endregion

    }

    public interface IBlockDefinitionBuilder
    {
        void Build(IFinalizedScope scope, OrType<ICodeElement,IError>[] body, IEnumerable<ICodeElement> staticInitailizers);
    }
}