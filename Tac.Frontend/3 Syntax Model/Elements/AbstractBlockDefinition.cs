using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Parser;

namespace Tac.SemanticModel
{

    internal abstract class WeakAbstractBlockDefinition<T> : IConvertableFrontendCodeElement<T>, IScoped
        where T: class, ICodeElement
    {
        protected WeakAbstractBlockDefinition(
            IBox<WeakScope> scope,
            IReadOnlyList<OrType<IBox<IFrontendCodeElement>, IError>> body, 
            IReadOnlyList<IIsPossibly<IFrontendCodeElement>> staticInitailizers){
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }


        public IBox<WeakScope> Scope { get; }
        public IReadOnlyList<OrType<IBox<IFrontendCodeElement>, IError>> Body { get; }
        // I think I am gettting well ahead of myself with these...
        // I think I should build this I plan on using soonish
        public IReadOnlyList<IIsPossibly<IFrontendCodeElement>> StaticInitailizers { get; }
        public abstract IBuildIntention<T> GetBuildIntention(IConversionContext context);
    }
}