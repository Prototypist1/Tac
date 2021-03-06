﻿using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{
    internal class InterpetedBlockDefinition :  IInterpetedOperation
    {
        public void Init(IInterpetedOperation[] body, IInterpetedScopeTemplate scope)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IInterpetedOperation[]? body; 
        public IInterpetedOperation[] Body { get=>body?? throw new NullReferenceException(nameof(body)); private set=>body= value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }

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