using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface IAssembly
    {
        IReadOnlyList<IAssembly> Referances { get; }
        IKey Key { get; }
        IFinalizedScope Scope { get; }
    }
}
