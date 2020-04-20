
using Prototypist.Toolbox;
using Tac.Model.Elements;

namespace Tac.Model
{

    public interface IConvertable {
        T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
        where TBacking : IBacking;
    }

    public interface ICodeElement: IConvertable
    {
        IVerifiableType Returns();
    }
}
