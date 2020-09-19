using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Extensions
{
    class ClosureExtension
    {
        public readonly IReadOnlyList<IMemberDefinition> closureMember;

        public ClosureExtension(IReadOnlyList<IMemberDefinition> closureMember)
        {
            this.closureMember = closureMember ?? throw new ArgumentNullException(nameof(closureMember));
        }
    }
}
