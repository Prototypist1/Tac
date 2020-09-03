using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedGenericTypeDefinition : IAssembledOperation
    {
        public void Init() { }

        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.EmptyMember(TypeManager.Empty()));
        }
    }
}