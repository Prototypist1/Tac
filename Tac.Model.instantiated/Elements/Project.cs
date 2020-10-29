using System;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class Project<TBacking> : IProject<TBacking>
    {
        public Project(IRootScope rootScope, IReadOnlyList<IAssembly<TBacking>> references)
        {
            RootScope = rootScope ?? throw new ArgumentNullException(nameof(rootScope));
            References = references ?? throw new ArgumentNullException(nameof(references));
        }

        // maybe this will become a list modules??
        // maybe you only get one root module
        public IRootScope RootScope { get; }
        public IReadOnlyList<IAssembly<TBacking>> References { get; }

    }
}
