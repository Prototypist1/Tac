using System;
using System.Collections.Generic;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedBlockDefinition :  IInterpeted
    {
        public void Init(IInterpeted[] body, IInterpetedScopeTemplate scope)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IInterpeted[] Body { get; private set; } 
        public IInterpetedScopeTemplate Scope { get; private set; }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
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