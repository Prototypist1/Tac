using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Frontend
{
    internal interface IFrontendCodeElement<out T>: IClonver<T> where T: class, ICodeElement
    {
        IIsPossibly<IFrontendType> Returns();
    }

    internal interface IFrontendType {
    }
}
