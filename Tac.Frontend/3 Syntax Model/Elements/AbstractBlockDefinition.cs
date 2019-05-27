using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model
{

    internal abstract class WeakAbstractBlockDefinition<T> : IConvertableFrontendCodeElement<T>, IScoped
        where T: class, ICodeElement
    {
        protected WeakAbstractBlockDefinition(
            IResolvableScope scope,
            IIsPossibly<IFrontendCodeElement>[] body, 
            IEnumerable<IIsPossibly<IFrontendCodeElement>> staticInitailizers){
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }


        public IResolvableScope Scope { get; }
        public IIsPossibly<IFrontendCodeElement>[] Body { get; }
        public IEnumerable<IIsPossibly<IFrontendCodeElement>> StaticInitailizers { get; }
        public abstract IBuildIntention<T> GetBuildIntention(ConversionContext context);
        public abstract IIsPossibly<IFrontendType> Returns();
    }
}