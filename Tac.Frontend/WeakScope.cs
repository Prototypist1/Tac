using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.SemanticModel;

namespace Tac.Frontend
{
    internal class WeakScope : IConvertable<IFinalizedScope>
    {
        public readonly List<IBox<WeakMemberDefinition>> membersList;

        public WeakScope(List<IBox<WeakMemberDefinition>> membersList)
        {
            this.membersList = membersList;
        }

        public IBuildIntention<IFinalizedScope> GetBuildIntention(IConversionContext context)
        {
            var scope = new Tac.Model.Instantiated.Scope();
            return new BuildIntention<Model.Instantiated.Scope>(scope, () => {
                scope.Build(membersList.Select(x=>new Model.Instantiated.Scope.IsStatic(x.GetValue().Convert(context),false)).ToArray());
             });
        }
    }
}