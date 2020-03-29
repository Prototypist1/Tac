
using Prototypist.Toolbox;
using Tac.Model.WithErrors.Elements;

namespace Tac.Model.WithErrors
{

    public interface IConvertable {
        T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
        where TBacking : IBacking;
    }

    public interface ICodeElement: IConvertable
    {
        IOrType<IVerifiableType, IError> Returns();
    }
}
