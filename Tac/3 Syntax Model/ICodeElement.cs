using Tac.Parser;

namespace Tac.Semantic_Model.CodeStuff
{
    public interface ICodeElement {

    }

    public interface IWeakCodeElement {
        IWeakReturnable Returns();
    }
}
