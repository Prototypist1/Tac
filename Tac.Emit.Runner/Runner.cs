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
        public static Tout CompileAndRun<Tin,Tout>(string name, string toRun, Tin input, IReadOnlyList<Assembly> assemblies)
        {
            var module = new TokenParser().Parse<Assembly, object>(toRun, assemblies, name);

            // ok so you are definately not done.
            // we need to assign the object in Backing of the assemblies
            // to the matching key in the dependency scope 
            // after you have created an obejct for the run
            //

            return Compiler.BuildAndRun<Tin, Tout>(module, input);
        }
    }
}
