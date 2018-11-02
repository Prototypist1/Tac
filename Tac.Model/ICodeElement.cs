
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface ICodeElement {
        // maybe we should encode return types in the type?
        IVarifiableType Returns();
        T Convert<T>(IOpenBoxesContext<T> context);
    }
}
