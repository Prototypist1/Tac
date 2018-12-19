using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Frontend
{
    internal interface IFrontendCodeElement<out T>: IConvertable<T> 
        where T: ICodeElement
    {
        // we can do better, but what does it get us?
        // let's wait and see
        IIsPossibly<IFrontendType<IVarifiableType>> Returns();
    }
    
    // TODO, some of these transform to specific types!
    internal interface IFrontendType<out T>: IConvertable<T>
        where T: IVarifiableType
    {
    }
}
