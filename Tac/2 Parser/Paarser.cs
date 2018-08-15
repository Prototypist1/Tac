using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Parser
{

    public interface IParseStateView
    {
        ICodeElement GetCodeElementOrThrow();
        ICodeElement GetCodeElement(Func<ICodeElement> generator);
        bool TryGetNext(out IParseStateView parseStateView);
        bool TryGetLast(out IParseStateView parseStateView);
        IToken Token { get; }
    }
    
    public class ParseState
    {
        public ParseState(IEnumerable<IToken> tokens) => Tokens = tokens?.ToArray() ?? throw new ArgumentNullException(nameof(tokens));

        private IToken[] Tokens { get; }
        private int At { get; set; } = 0;

        public bool TryGetNextToken(out IToken token)
        {
            if (At < Tokens.Length)
            {
                token = Tokens[At];
                At++;
                return true;
            }
            else
            {
                token = default;
                return false;
            }
        }

        public bool TryGetStart(out IParseStateView parseStateView) {
            if (TryGetNextToken(out var token))
            {
                parseStateView = new ParseStateView(this, token, null);
                return true;
            }
            parseStateView = default;
            return false;
        }
        
        private class ParseStateView: IParseStateView
        {
            public ParseStateView(ParseState parseState, IToken token, ParseStateView last)
            {
                ParseState = parseState ?? throw new ArgumentNullException(nameof(parseState));
                Token = token ?? throw new ArgumentNullException(nameof(token));
                Last = last;
            }

            private ParseState ParseState { get; }
            public IToken Token { get; }
            private ICodeElement CodeElement { get; set; }
            private ParseStateView Next { get; set; }
            private ParseStateView Last { get; }

            public ICodeElement GetCodeElement(Func<ICodeElement> generator) {
                if (CodeElement == null) {
                    CodeElement = generator();
                }
                return CodeElement;
            }

            public ICodeElement GetCodeElementOrThrow()
            {
                if (CodeElement == null)
                {
                    throw new Exception("Code element does not exist");
                }
                return CodeElement;
            }

            public bool TryGetNext(out IParseStateView parseStateView)
            {
                if (Next == default && ParseState.TryGetNextToken(out var token))
                {
                    Next = new ParseStateView(ParseState, token, this);
                }
                parseStateView = Next;
                return Next != default;
            }



            public bool TryGetLast(out IParseStateView parseStateView)
            {
                parseStateView = Last;
                return Last != default;
            }
        }

    }
    
}
