using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    internal abstract class WeakAbstractBlockDefinition : ICodeElement, IScoped, IBlockDefinition
    {
        protected WeakAbstractBlockDefinition(IFinalizedScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers){
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }


        public IFinalizedScope Scope { get; }
        public ICodeElement[] Body { get; }
        public IEnumerable<ICodeElement> StaticInitailizers { get; }

        IFinalizedScope IAbstractBlockDefinition.Scope
        {
            get
            {
                return Scope;
            }
        }

        public abstract T Convert<T>(IOpenBoxesContext<T> context);
        
        public IVarifiableType Returns() { return this; }
    }
}