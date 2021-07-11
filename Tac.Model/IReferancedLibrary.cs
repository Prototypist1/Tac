using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface IAssembly<out TBacking>
    {
        NameKey Key { get; }
        IInterfaceType Scope { get; }
        // probably need this someday 
        //IReadOnlyList<IAssembly<TBacking>> References { get; }
        TBacking Backing { get; }
    }

    //public interface IBacking { }
}
