using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Extensions
{
    class ClosureLookup
    {
        public readonly IReadOnlyList<IMemberDefinition> closureMember;

        public ClosureLookup(IReadOnlyList<IMemberDefinition> closureMember)
        {
            this.closureMember = closureMember ?? throw new ArgumentNullException(nameof(closureMember));
        }
    }
}
