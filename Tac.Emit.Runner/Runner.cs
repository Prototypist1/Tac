using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Model;
using Tac.Backend.Emit;

namespace Tac.Emit.Runner
{
    public static  class Run
    {
        public static Tout CompileAndRun<Tin,Tout>(string name, string toRun, Tin input, IReadOnlyList<IAssembly<object>> assemblies)
        {
            var module = new TokenParser().Parse(toRun, assemblies, name);

            return Compiler.BuildAndRun<Tin, Tout>(
                Model.Instantiated.RootScope.CreateAndBuild(
                    module.RootScope.Scope,
                    module.RootScope.Assignments,
                    module.RootScope.EntryPoint),
                input);
        }
    }
}
