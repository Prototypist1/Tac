using Prototypist.Toolbox;
using System;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{

    internal interface IInterpetedMemberDefinition : IAssembledOperation
    {
        IKey Key
        {
            get;
        }
    }

    internal class InterpetedMemberDefinition: IInterpetedMemberDefinition
    {
        public InterpetedMemberDefinition Init(IKey key, InterpetedTypeDefinition type)
        {
            return this;
        }


        public InterpetedTypeDefinition interpetedTypeDefinition;
        public string name;

        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {
            var member = TypeManager.Member(Type);

            if (!interpetedContext.TryAddMember(Key, member)) {
                throw new Exception("bad, shit");
            }

            return InterpetedResult.Create(member);
        }
    }
}