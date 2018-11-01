
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface ICodeElement {
        IVarifiableType Returns();
        T Convert<T>(IOpenBoxesContext<T> context);
    }
}
