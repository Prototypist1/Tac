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
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model
{

    internal abstract class WeakAbstractBlockDefinition<T> : IFrontendCodeElement<T>, IScoped
        where T: class, ICodeElement
    {
        protected WeakAbstractBlockDefinition(
            IFinalizedScope scope,
            IIsPossibly<IFrontendCodeElement<ICodeElement>>[] body, 
            IEnumerable<IIsPossibly<IFrontendCodeElement<ICodeElement>>> staticInitailizers){
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }


        public IFinalizedScope Scope { get; }
        public IIsPossibly<IFrontendCodeElement<ICodeElement>>[] Body { get; }
        public IEnumerable<IIsPossibly<IFrontendCodeElement<ICodeElement>>> StaticInitailizers { get; }

        public abstract IBuildIntention<T> GetBuildIntention(ConversionContext context);
        public abstract IIsPossibly<IFrontendType> Returns();
    }
}