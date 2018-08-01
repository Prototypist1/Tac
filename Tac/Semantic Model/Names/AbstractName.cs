using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.Names
{
    public abstract class AbstractName {

    }

    public sealed class ExplicitName: AbstractName
    {
        public ExplicitName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }
    }

    public sealed class Anonymous : AbstractName {

    }
}
