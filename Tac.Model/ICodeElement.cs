
using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model
{

    public interface IConvertable {
        T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking;
    }

    
    public interface ICodeElement: IConvertable
    {
        // I am not sure I need this
        // I am not vetting this model
        IVerifiableType Returns();
    }
}
