using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    internal abstract class WeakAbstractBlockDefinition : IFrontendCodeElement, IScoped, IBlockDefinition
    {
        protected WeakAbstractBlockDefinition(
            IFinalizedScope scope,
            IIsPossibly<IFrontendCodeElement>[] body, 
            IEnumerable<IIsPossibly<IFrontendCodeElement>> staticInitailizers){
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }


        public IFinalizedScope Scope { get; }
        public IIsPossibly<IFrontendCodeElement>[] Body { get; }
        public IEnumerable<IIsPossibly<IFrontendCodeElement>> StaticInitailizers { get; }

        IFinalizedScope IAbstractBlockDefinition.Scope
        {
            get
            {
                return Scope;
            }
        }

        #region IBlockDefinition

        ICodeElement[] IAbstractBlockDefinition.Body => Body.Select(x => x.GetOrThrow()).ToArray();
        IEnumerable<ICodeElement> IAbstractBlockDefinition.StaticInitailizers => StaticInitailizers.Select(x => x.GetOrThrow()).ToArray();

        #endregion

        public abstract T Convert<T>(IOpenBoxesContext<T> context);
        
        public IVarifiableType Returns() { return this; }

        IIsPossibly<IVarifiableType> IFrontendCodeElement.Returns()
        {
            return Possibly.Is(this);
        }
    }
}