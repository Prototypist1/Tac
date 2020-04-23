using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
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
            IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>> body, 
            IReadOnlyList<IIsPossibly<IFrontendCodeElement>> staticInitailizers){
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }


        public IBox<WeakScope> Scope { get; }
        public IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>> Body { get; }
        // I think I am gettting well ahead of myself with these...
        // I think I should build this I plan on using soonish
        // why are these IIsPossibly?? 
        public IReadOnlyList<IIsPossibly<IFrontendCodeElement>> StaticInitailizers { get; }
        public abstract IBuildIntention<T> GetBuildIntention(IConversionContext context);

        public virtual IEnumerable<IError> Validate()
        {
            foreach (var item in Scope.GetValue().Validate())
            {
                yield return item;
            }
            foreach (var line in Body)
            {
                foreach (var error in line.SwitchReturns(x=>x.GetValue().Validate(),x=>new IError[] { x}))
                {
                    yield return error;
                }
            }
            foreach (var line in StaticInitailizers.OfType<IIsDefinately<IFrontendCodeElement>>())
            {
                foreach (var error in line.Value.Validate())
                {
                    yield return error;
                }
            }
        }
    }
}