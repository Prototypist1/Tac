using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.SemanticModel;

namespace Tac.Frontend
{
    // see: EXTERNAL TYPES | 6D97B5C3-BFB5-4F1F-AE91-8955AD8277AD
    //internal class ExternalScope : WeakScope
    //{
    //    private readonly IFinalizedScope scope;

    //    public ExternalScope(IFinalizedScope scope,IReadOnlyList<WeakExternslMemberDefinition> membersList): base(membersList)
    //    {
    //        this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
    //    }

    //    public override IBuildIntention<IFinalizedScope> GetBuildIntention(IConversionContext context)
    //    {
    //        return new BuildIntention<IFinalizedScope>(scope, () => {});
    //    }
    //}

    internal class WeakScope : IConvertable<IFinalizedScope>
    {
        public readonly IReadOnlyList<WeakMemberDefinition> membersList;

        public WeakScope(IReadOnlyList<WeakMemberDefinition> membersList)
        {
            this.membersList = membersList;
        }

        public IBuildIntention<IFinalizedScope> GetBuildIntention(IConversionContext context)
        {
            var scope = new Tac.Model.Instantiated.Scope();
            return new BuildIntention<Model.Instantiated.Scope>(scope, () => {
                scope.Build(membersList.Select(x=>new IsStatic(x.Convert(context),false)).ToArray());
             });
        }

        internal IEnumerable<IError> Validate()
        {
            return membersList.SelectMany(x => x.Validate());
        }
    }
}