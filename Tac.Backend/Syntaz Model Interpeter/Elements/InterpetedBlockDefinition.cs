using System;
using System.Collections.Generic;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedBlockDefinition :  IInterpeted
    {
        public void Init(IInterpeted[] body, IFinalizedScope scope)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IInterpeted[] Body { get; private set; } 
        public IFinalizedScope Scope { get; private set; }
        
        private InterpetedStaticScope StaticStuff { get; } = InterpetedStaticScope.Empty();

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var res = InterpetedInstanceScope.Make(interpetedContext,StaticStuff, Scope);

            var scope = interpetedContext.Child(res);

            foreach (var line in Body)
            {
                var result = line.Interpet(scope);
                if (result.IsReturn)
                {
                    if (result.HasValue)
                    {
                        return InterpetedResult.Create(result.Get());
                    }
                    else
                    {
                        return InterpetedResult.Create();
                    }
                }
            }

            return InterpetedResult.Create();
        }
    }
}