using System;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Model.Elements;
using System.Reflection;
using static Tac.Backend.Emit.Public.AssemblyBuilder;
using System.Reflection.Emit;

namespace Tac.Backend.Emit
{
    public static class Compiler
    {
        public static void Build(IProject<InterpetedAssemblyBacking> moduleDefinition)
        {
            var conversionContext = new Definitions();


            var assemblyName = new AssemblyName();
            assemblyName.Name = "HelloReflectionEmit";
            var assembly = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);


            var module = assembly.DefineDynamicModule(assemblyName.Name);




            var interpetedContext = AssemblyContext.Root();
            foreach (var reference in moduleDefinition.References)
            {
                interpetedContext.TryAddMember(reference.Key, reference.Backing.CreateMember(interpetedContext));
            }

            if (conversionContext.ModuleDefinition(moduleDefinition.ModuleDefinition).Assemble(interpetedContext).IsReturn(out var _, out var _))
            {
                throw new Exception("this should not really return");
            }

            if (conversionContext.EntryPoint == null) {
                throw new NullReferenceException();
            }
            conversionContext.EntryPoint.Assemble(interpetedContext);
        }
    }
}
