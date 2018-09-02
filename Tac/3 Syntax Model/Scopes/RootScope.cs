using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public static partial class RootScope 
    {
        public static IScope Root { get => Root; }
        private static StaticScope root = new StaticScope();
    }
}