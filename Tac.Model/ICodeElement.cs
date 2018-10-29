
using Tac.Model.Elements;

namespace Tac.Model
{
    public interface ICodeElement {
        IType Returns();
        T Convert<T>(IOpenBoxesContext<T> context);
    }
}
