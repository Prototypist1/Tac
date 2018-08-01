using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.Names
{
    public abstract class AbstractName : IName
    {
        protected AbstractName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }
    }
}
