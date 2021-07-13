using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Interpreted;
using Tac.Backend.Interpreted.Public;
using Tac.Frontend;
using Tac.Model;
using Tac.SemanticModel.CodeStuff;
using static Tac.Backend.Interpreted.Public.AssemblyBuilder;

// yeah, I really do not need a project for this...
namespace Tac.Interpreted.Runner
{


    public static class Runner
    {

        public static void RunInterpeted(string name, IReadOnlyList<AssemblyBuilder> dependencies,string toRun)
        {
            var module = new TokenParser().Parse(toRun, dependencies, name);
            Interpeter.Run(module);
        }
    }
}
