using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedEntryPointDefinition: IInterpetedOperation<IInterpetedAnyType>
    {

        public IInterpetedOperation<IInterpetedAnyType>[] Body { get; private set; }
        public IInterpetedScopeTemplate Scope { get; private set; }

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
                if (result.IsReturn(out var res, out var value))
                {
                    return InterpetedResult.Return<IInterpetedMember<IInterpedEmpty>>(res);
                }
            }

            return InterpetedResult.Create();
        }
    }
}