using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Interpreted;
using Tac.Frontend;
using Tac.Model;
using Tac.SemanticModel.CodeStuff;
using static Tac.Backend.Interpreted.Public.AssemblyBuilder;

// yeah, I really do not need a project for this...
namespace Tac.Interpreted.Runner
{
    public static class Runner
    {

        public static void RunInterpeted(string name, IReadOnlyList<IAssembly<InterpetedAssemblyBacking>> dependencies,string toRun)
        {
            var module = new TokenParser().Parse<IAssembly<InterpetedAssemblyBacking>, InterpetedAssemblyBacking>(toRun, dependencies, name);
            Interpeter.Run(module);
        }
    }
}
