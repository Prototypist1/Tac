using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Frontend
{ 
    // not every frontend code element is convertable. type definitions are not.
    internal interface IFrontendCodeElement
    {
        // we can do better, but what does it get us?
        // let's wait and see
        IIsPossibly<IFrontendType> Returns();
    }

    internal interface IConvertableFrontendCodeElement<out T>: IFrontendCodeElement, IConvertable<T> 
        where T: ICodeElement
    {
    }

    internal interface IFrontendType
    {
    }

    // TODO, some of these transform to specific types!
    internal interface IConvertableFrontendType<out T>: IFrontendType, IConvertable<T>
        where T: IVerifiableType
    {
    }
}
