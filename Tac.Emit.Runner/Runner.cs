using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Model;
using Tac.Backend.Emit;
using Tac.Model.Elements;

namespace Tac.Emit.Runner
{

    //public class AsseblyPendingType : IAsseblyPendingType<Assembly, object>
    //{
    //    private readonly object obj;

    //    public AsseblyPendingType(NameKey key, IReadOnlyList<IMemberDefinition> members, object obj)
    //    {
    //        this.obj = obj ?? throw new ArgumentNullException(nameof(obj));
    //        Key = key ?? throw new ArgumentNullException(nameof(key));
    //        Members = members ?? throw new ArgumentNullException(nameof(members));
    //    }

    //    public NameKey Key
    //    {
    //        get; init;
    //    }

    //    public IReadOnlyList<IMemberDefinition> Members
    //    {
    //        get; init;
    //    }

    //    public Assembly Convert(IInterfaceType interfaceType)
    //    {
    //        return new Assembly(Key, interfaceType, obj);
    //    }
    //}

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

            var res = Compiler.BuildAndRun<Tin, Tout>(module, input);
            return res;
        }
    }
}
