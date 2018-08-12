using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ModuleDefinition : IReferanced, IScoped<StaticScope>
    {
        public ModuleDefinition(AbstractName key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public AbstractName Key { get; }

        public StaticScope Scope { get; } = new StaticScope();
    }
}
