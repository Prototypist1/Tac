using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model
{

    internal abstract class WeakAbstractBlockDefinition<T> : IFrontendCodeElement<T>, IScoped
        where T: class, ICodeElement
    {
        protected WeakAbstractBlockDefinition(
            IResolvableScope scope,
            IIsPossibly<IFrontendCodeElement<ICodeElement>>[] body, 
            IEnumerable<IIsPossibly<IFrontendCodeElement<ICodeElement>>> staticInitailizers){
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }


        public IResolvableScope Scope { get; }
        public IIsPossibly<IFrontendCodeElement<ICodeElement>>[] Body { get; }
        public IEnumerable<IIsPossibly<IFrontendCodeElement<ICodeElement>>> StaticInitailizers { get; }

        public abstract IBuildIntention<T> GetBuildIntention(ConversionContext context);
        public abstract IIsPossibly<IFrontendType<IVarifiableType>> Returns();
    }
}