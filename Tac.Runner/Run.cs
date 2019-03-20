using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend;
using Tac.Frontend;
using Tac.Model;
using Tac.Semantic_Model.CodeStuff;
using static Tac.Backend.Public.AssemblyBuilder;

// yeah, I really do not need a project for this...
namespace Tac.Runner
{
    public static class Runner
    {

        public static void Run(IReadOnlyList<IAssembly<InterpetedAssemblyBacking>> dependencies,string toRun)
        {
            var module = TokenParser.Parse(toRun,dependencies);
            Interpeter.Run(module);
        }
    }
}
