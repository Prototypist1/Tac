using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tac.Model;
using Tac.Model.Instantiated;
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
                scope.Build(membersList.Select(x=>new IsStatic(x.GetValue().Convert(context),false)).ToArray());
             });
        }

        internal IEnumerable<IError> Validate()
        {
            return membersList.SelectMany(x => x.GetValue().Validate());
        }
    }
}