﻿using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Interpreted.Syntaz_Model_Interpeter;
using Tac.Backend.Interpreted.Syntaz_Model_Interpeter.Elements;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using static Tac.Backend.Interpreted.Public.AssemblyBuilder;

namespace Tac.Backend.Interpreted
{
    public static class Interpeter
    {
        public static void Run(IProject<InterpetedAssemblyBacking> moduleDefinition)
        {
            var conversionContext = new Definitions();

            var interpetedContext = InterpetedContext.Root();
            foreach (var reference in moduleDefinition.References)
            {
                interpetedContext.TryAddMember(reference.Key, reference.Backing.CreateMember(interpetedContext));
            }

            if (conversionContext.ModuleDefinition(moduleDefinition.ModuleDefinition).Interpet(interpetedContext).IsReturn(out var _, out var _))
            {
                throw new Exception("this should not really return");
            }

            if (conversionContext.EntryPoint == null) {
                throw new NullReferenceException();
            }
            conversionContext.EntryPoint.Interpet(interpetedContext);
        }
    }
}