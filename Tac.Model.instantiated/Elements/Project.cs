using System;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class Project<TAssembly, TBacking> : IProject<TAssembly, TBacking>
        where TAssembly : IAssembly<TBacking>
    {
        public Project(IRootScope rootScope, IReadOnlyList<TAssembly> references, IFinalizedScope dependencyScope)
        {
            RootScope = rootScope ?? throw new ArgumentNullException(nameof(rootScope));
            References = references ?? throw new ArgumentNullException(nameof(references));
            DependencyScope = dependencyScope ?? throw new ArgumentNullException(nameof(dependencyScope));
        }

        // maybe this will become a list modules??
        // maybe you only get one root module
        public IRootScope RootScope { get; }
        public IReadOnlyList<TAssembly> References { get; }
        public IFinalizedScope DependencyScope { get; }
    }
}
