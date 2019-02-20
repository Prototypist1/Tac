﻿using System.Collections.Generic;
using Prototypist.TaskChain.DataTypes;
using System.Linq;
using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedObjectDefinition :  IInterpetedOperation<IInterpetedScope>
    {
        public void Init(IInterpetedScopeTemplate scope, IEnumerable<IInterpetedAssignOperation<object>> assignments)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assignments ?? throw new ArgumentNullException(nameof(assignments));
        }

        public IInterpetedScopeTemplate Scope { get; private set; }
        public IEnumerable<IInterpetedAssignOperation<object>> Assignments { get; private set; }
        
        public IInterpetedResult<IInterpetedMember<IInterpetedScope>> Interpet(InterpetedContext interpetedContext)
        {
            var scope = Scope.Create();

            var context = interpetedContext.Child(scope);

            foreach (var line in Assignments)
            {
                line.Interpet(context);
            }

            return InterpetedResult.Create(new InterpetedMember<IInterpetedScope>(scope));
        }
        
        public IInterpetedScope GetDefault(InterpetedContext interpetedContext)
        {
            return InterpetedInstanceScope.Make();
        }
    }
}