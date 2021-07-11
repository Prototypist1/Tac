using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Extensions
{
    class WhoDefinedMember
    {
        // the member and who orignally defined it 
        public readonly IReadOnlyDictionary<IMemberDefinition, IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition, IBlockDefinition, IRootScope, IObjectDefiniton >> closureMember;

        public WhoDefinedMember(IReadOnlyDictionary<IMemberDefinition, IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition, IBlockDefinition, IRootScope, IObjectDefiniton>> closureMember)
        {
            this.closureMember = closureMember ?? throw new ArgumentNullException(nameof(closureMember));
        }
    }
}
