using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedBlockDefinition :  IInterpetedOperation<IInterpedEmpty>
    {
        public void Init(IInterpetedOperation<object>[] body, IInterpetedScopeTemplate scope)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IInterpetedOperation<object>[] Body { get; private set; } 
        public IInterpetedScopeTemplate Scope { get; private set; }

        public IInterpetedResult<IInterpetedMember<IInterpedEmpty>> Interpet(InterpetedContext interpetedContext)
        {
            var scope = interpetedContext.Child(Scope.Create());

            foreach (var line in Body)
            {
                var result = line.Interpet(scope);
                if (result.IsReturn)
                {
                    return result;
                }
            }

            return InterpetedResult.Create();
        }
    }
}