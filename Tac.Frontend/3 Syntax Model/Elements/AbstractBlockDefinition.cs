using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Semantic_Model
{

    internal abstract class WeakAbstractBlockDefinition<T> : IConvertableFrontendCodeElement<T>, IScoped
        where T: class, ICodeElement
    {
        protected WeakAbstractBlockDefinition(
            IBox<WeakScope> scope,
            IBox<IFrontendCodeElement>[] body, 
            IEnumerable<IIsPossibly<IFrontendCodeElement>> staticInitailizers){
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }


        public IBox<WeakScope> Scope { get; }
        public IBox<IFrontendCodeElement>[] Body { get; }
        // I think I am gettting well ahead of myself with these...
        // I think I should build this I plan on using soonish
        public IEnumerable<IIsPossibly<IFrontendCodeElement>> StaticInitailizers { get; }
        public abstract IBuildIntention<T> GetBuildIntention(IConversionContext context);
        public abstract IIsPossibly<IFrontendType> Returns();
    }
}