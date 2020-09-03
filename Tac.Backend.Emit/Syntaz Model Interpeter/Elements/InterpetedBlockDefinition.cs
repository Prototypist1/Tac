using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedBlockDefinition :  IAssembledOperation
    {
        public void Init(IAssembledOperationRequiresGenerator[] body, IInterpetedScopeTemplate scope)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IAssembledOperationRequiresGenerator[]? body; 
        public IAssembledOperationRequiresGenerator[] Body { get=>body?? throw new NullReferenceException(nameof(body)); private set=>body= value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {
            var scope = interpetedContext.Child(Scope.Create());

            foreach (var line in Body)
            {
                var result = line.Assemble(scope);
                if (result.IsReturn(out var res, out var _))
                {
                    return InterpetedResult.Return<IInterpetedMember>(res!);
                }
            }

            return InterpetedResult.Create();
        }
    }
}