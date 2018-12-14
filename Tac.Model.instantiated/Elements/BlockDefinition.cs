using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class BlockDefinition : IBlockDefinition, IBlockDefinitionBuilder
    {
        private readonly Buildable<IEnumerable<ICodeElement>> buildableStaticInitailizers = new Buildable<IEnumerable<ICodeElement>>();
        private readonly Buildable<ICodeElement[]> buildableBody = new Buildable<ICodeElement[]>();
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();

        private BlockDefinition() 
        {
        }

        #region IBlockDefinition

        public IFinalizedScope Scope { get => buildableScope.Get(); }
        public ICodeElement[] Body { get => buildableBody.Get(); }
        public IEnumerable<ICodeElement> StaticInitailizers { get => buildableStaticInitailizers.Get(); }


        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.BlockDefinition(this);
        }
        public IVarifiableType Returns()
        {
            return new EmptyType();
        }


        #endregion

        #region IBlockDefinitionBuilder

        public void Build(IFinalizedScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers)
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

        #endregion

    }

    public interface IBlockDefinitionBuilder
    {
        void Build(IFinalizedScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers);
    }
}