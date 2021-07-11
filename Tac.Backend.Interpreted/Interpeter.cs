using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Elements;
using Tac.Model.Elements;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;
using static Tac.Backend.Interpreted.Public.AssemblyBuilder;
using Tac.Model;

namespace Tac.Backend.Interpreted
{
    public static class Interpeter
    {
        public static void Run(IProject<IAssembly<InterpetedAssemblyBacking>,InterpetedAssemblyBacking> moduleDefinition)
        {
            var conversionContext = new Definitions();

            var interpetedContext = InterpetedContext.Root();
            foreach (var reference in moduleDefinition.References)
            {
                interpetedContext.TryAddMember(reference.Key, reference.Backing.CreateMember(interpetedContext));
            }

            if (conversionContext.RootScope(moduleDefinition.RootScope).Interpet(interpetedContext).IsReturn(out var _, out var _))
            {
                throw new Exception("this should not really return");
            }
        }
    }
}
