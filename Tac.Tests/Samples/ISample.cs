using System.Collections.Generic;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Tests.Samples
{
    public interface ISample
    {
        string Text { get; }
        IToken Token { get; }
        IEnumerable<ICodeElement> CodeElements { get; }
    }
}