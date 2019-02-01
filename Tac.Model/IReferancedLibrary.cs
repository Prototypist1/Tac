using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface IAssembly: IModuleDefinition
    {
        IReadOnlyList<IAssembly> Referances { get; }
    }
}
