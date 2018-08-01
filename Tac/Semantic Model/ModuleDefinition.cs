using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    public sealed class ModuleDefinition: IReferanced<ModuleName>
    {
        public ModuleName Key { get; }
    }
}
