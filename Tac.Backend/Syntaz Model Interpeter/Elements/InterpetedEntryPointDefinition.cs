using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedEntryPointDefinition: IInterpetedOperation<IInterpetedAnyType>
    {

        private IInterpetedOperation<IInterpetedAnyType>[]? body;
        public IInterpetedOperation<IInterpetedAnyType>[] Body { get => body ?? throw new NullReferenceException(nameof(body)); private set => body = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }


        public void Init(
            IInterpetedOperation<IInterpetedAnyType>[] methodBody,
            IInterpetedScopeTemplate scope)
        {
            Body = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IInterpetedResult<IInterpetedMember<IInterpetedAnyType>> Interpet(InterpetedContext interpetedContext)
        {
            var scope = interpetedContext.Child(Scope.Create());

            foreach (var line in Body)
            {
                var result = line.Interpet(scope);
                if (result.IsReturn(out var res, out var _))
                {
                    return InterpetedResult.Return<IInterpetedMember<IInterpedEmpty>>(res!);
                }
            }

            return InterpetedResult.Create();
        }
    }
}