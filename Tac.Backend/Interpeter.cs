using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
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
                interpetedContext.TryAddMember(reference.Key,reference.Backing.CreateMember(interpetedContext));
            }
            conversionContext.ModuleDefinition(moduleDefinition.ModuleDefinition).Interpet(interpetedContext).Cast<InterpetedMember<IInterpetedScope>>();

            conversionContext.EntryPoint.Invoke(new InterpetedMember<IInterpedEmpty>(new RunTimeEmpty()));
        }
    }
}
