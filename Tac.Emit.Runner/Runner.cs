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

        private class NotSupported { }

        // TODO some sort of dependency support
        //IReadOnlyList<IAssembly<InterpetedAssemblyBacking>> dependencies,
        public static Tout CompileAndRun<Tin,Tout>(string name, string toRun, Tin input)
        {
            var module = new TokenParser().Parse(toRun, Array.Empty<IAssembly<NotSupported>>(), name);

            return Compiler.BuildAndRun<Tin, Tout>(
                                Model.Instantiated.RootScope.CreateAndBuild(
                                    module.RootScope.Scope,
                                    module.RootScope.Assignments,
                                    module.RootScope.EntryPoint
                                    )
                            , input);
        }
    }
}
