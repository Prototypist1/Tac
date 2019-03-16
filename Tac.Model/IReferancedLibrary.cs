using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface IAssembly<TBacking>
        where TBacking : IBacking
    {
        IKey Key { get; }
        IFinalizedScope Scope { get; }
        IReadOnlyList<IAssembly<TBacking>> References { get; }
        TBacking Backing { get; }
    }

    public interface IBacking { }
}
