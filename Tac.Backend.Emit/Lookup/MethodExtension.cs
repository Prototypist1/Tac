using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Extensions
{
    class ClosureLookup
    {
        // the member and who orignally defined it 
        public readonly IReadOnlyDictionary<IMemberDefinition, IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition, IObjectDefiniton >> closureMember;

        public ClosureLookup(IReadOnlyDictionary<IMemberDefinition, IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition, IObjectDefiniton>> closureMember)
        {
            this.closureMember = closureMember ?? throw new ArgumentNullException(nameof(closureMember));
        }
    }
}
