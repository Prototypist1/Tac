
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface ICodeElement {
        // maybe we should encode return types in the type?
        IVerifiableType Returns();
        T Convert<T>(IOpenBoxesContext<T> context);
    }
}
