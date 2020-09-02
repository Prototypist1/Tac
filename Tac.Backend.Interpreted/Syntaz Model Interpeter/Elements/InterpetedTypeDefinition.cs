
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{
    internal class InterpetedTypeDefinition: IInterpetedOperation
    {
        public void Init() { }
        
        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.EmptyMember(TypeManager.Empty()));
        }
    }
}