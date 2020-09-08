using System;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class Project<TBacking> : IProject<TBacking>
    {
        public Project(IModuleDefinition moduleDefinition, IReadOnlyList<IAssembly<TBacking>> references)
        {
            ModuleDefinition = moduleDefinition ?? throw new ArgumentNullException(nameof(moduleDefinition));
            References = references ?? throw new ArgumentNullException(nameof(references));
        }

        // maybe this will become a list modules??
        // maybe you only get one root module
        public IModuleDefinition ModuleDefinition { get; }
        public IReadOnlyList<IAssembly<TBacking>> References { get; }

    }
}
