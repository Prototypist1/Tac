using System;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Parser
{
    public interface IToken
    {
        CodeElement GetCodeElement(IParseStateView parseStateView);
    }

}
