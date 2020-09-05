using System;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedEntryPointDefinition: IAssembledOperation
    {

        private IAssembledOperation[]? body;
        public IAssembledOperation[] Body { get => body ?? throw new NullReferenceException(nameof(body)); private set => body = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedScope? scope;
        public IInterpetedScope Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }


        public void Init(
            IAssembledOperation[] methodBody,
            IInterpetedScope scope)
        {
            Body = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

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