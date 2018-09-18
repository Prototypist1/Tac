﻿using Prototypist.LeftToRight;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Syntaz_Model_Interpeter
{
    class InterpetedMethod {
        public InterpetedMethod(MemberDefinition parameterDefinition, ICodeElement[] body, InterpetedContext context, IScope scope) 
        {
            ParameterDefinition = parameterDefinition ?? throw new System.ArgumentNullException(nameof(parameterDefinition));
            Body = body ?? throw new System.ArgumentNullException(nameof(body));
            Context = context ?? throw new System.ArgumentNullException(nameof(context));
            Scope = scope ?? throw new System.ArgumentNullException(nameof(scope));
        }

        private MemberDefinition ParameterDefinition { get; }
        private ICodeElement[] Body { get; }
        private InterpetedContext Context { get; }
        private IScope Scope { get; }
        private InterpetedStaticScope StaticScope { get; } = InterpetedStaticScope.Empty();


        public InterpetedResult Invoke(object input) {

            var res = InterpetedInstanceScope.Make(StaticScope, Scope);

            res.SetMember(ParameterDefinition.Key.Key, input);

            var scope = Context.Child(res);

            foreach (var line in Body)
            {
                line.Cast<IInterpeted>().Interpet(scope);
            }

            return new InterpetedResult();
        }
    }
}