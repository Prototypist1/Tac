using Tac.Parser;

namespace Tac.Tests.Tokenizer
{
    public static class TokenHelp{

        public static ElementToken Ele(params IToken[] tokens)
        {
            return new ElementToken(tokens);
        }
        public static ParenthesisToken Par(params IToken[] tokens)
        {
            return new ParenthesisToken(tokens);
        }
        //public static BrokenBracketToken Broke(params IToken[] tokens)
        //{
        //    return new BrokenBracketToken(tokens);
        //}
        public static CurleyBacketToken Curl(params IToken[] tokens)
        {
            return new CurleyBacketToken(tokens);
        }
        public static FileToken File(params IToken[] tokens)
        {
            return new FileToken(tokens);
        }
        public static LineToken Line(params IToken[] tokens)
        {
            return new LineToken(tokens);
        }
        public static SquareBacketToken Square(params IToken[] tokens)
        {
            return new SquareBacketToken(tokens);
        }
        public static AtomicToken Atom(string s)
        {
            return new AtomicToken(s);
        }
    }
}
