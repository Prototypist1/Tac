using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend;
using Tac.Frontend;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Runner
{
    public static class Runner
    {

        public static void Run(string toRun) {

            var module = TokenParser.Parse(toRun);

            Interpeter.Run(module);
        }
    }
}
