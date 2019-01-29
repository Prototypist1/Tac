﻿using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;

namespace Tac.Backend
{
    public static class Interpeter
    {
        public static void Run(IModuleDefinition moduleDefinition)
        {
            var conversionContext = new Definitions(new ExternalMethodSource(new Dictionary<Guid, InterpetedExternalMethodDefinition>()));
            moduleDefinition.Convert(conversionContext).Interpet(InterpetedContext.Root());

            // todo find the entry point and run 
        }
    }
}
