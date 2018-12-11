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

    internal abstract class WeakAbstractBlockDefinition : IFrontendCodeElement, IScoped, IVarifiableType, IFrontendType
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
        
        public IVarifiableType Returns() { return this; }

        IIsPossibly<IFrontendType> IFrontendCodeElement.Returns()
        {
            return Possibly.Is(this);
        }
    }
}