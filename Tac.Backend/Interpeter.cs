using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;
using static Tac.Backend.Public.AssemblyBuilder;

namespace Tac.Backend
{
    public static class Interpeter
    {
        public static void Run(IProject<InterpetedAssemblyBacking> moduleDefinition)
        {
            var conversionContext = new Definitions();


            var interpetedContext = InterpetedContext.Root();
            foreach (var reference in moduleDefinition.References)
            {
                interpetedContext.TryAddMember(reference.Key,reference.Backing.CreateMember());
            }
            moduleDefinition.ModuleDefinition.Convert(conversionContext).Interpet(interpetedContext);

            // todo find the entry point and run 
        }
    }
}
