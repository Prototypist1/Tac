using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Frontend
{
    internal interface IFrontendCodeElement
    {
        IIsPossibly<IFrontendType> Returns();
    }

    internal interface IFrontendType {
    }
}
