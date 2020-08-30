using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedEntryPointDefinition: IInterpetedOperation
    {

        private IInterpetedOperation[]? body;
        public IInterpetedOperation[] Body { get => body ?? throw new NullReferenceException(nameof(body)); private set => body = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }


        public void Init(
            IInterpetedOperation[] methodBody,
            IInterpetedScopeTemplate scope)
        {
            Body = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            var scope = interpetedContext.Child(Scope.Create());

            foreach (var line in Body)
            {
                var result = line.Interpet(scope);
                if (result.IsReturn(out var res, out var _))
                {
                    return InterpetedResult.Return<IInterpetedMember>(res!);
                }
            }

            return InterpetedResult.Create();
        }
    }
}