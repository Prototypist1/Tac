using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel.Run_Time_Objects
{

    internal class InterpetedEmptyInstance : IAssembledOperationRequiresGenerator
    {
        public void Init()
        {
        }

        public void Assemble(AssemblyContextWithGenerator interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.EmptyMember(TypeManager.Empty()));
        }
    }
}
