
using Tac.Model.Elements;
using Tac.New;

namespace Tac.Model
{
    public interface ICodeElement {
        IType Returns();
        T Convert<T>(IOpenBoxesContext<T> context);
    }
}
