using System;
using System.Collections.Generic;

namespace Tac.Parser
{
    public class TokenFactory
    {
        public TokenFactory(Func<IEnumerable<IToken>, IToken> makeToken, TryExitDelegate tryExit, MustExitDelegate mustExit)
        {
            MakeToken = makeToken ?? throw new ArgumentNullException(nameof(makeToken));
            TryExit = tryExit ?? throw new ArgumentNullException(nameof(tryExit));
            MustExit = mustExit ?? throw new ArgumentNullException(nameof(mustExit));
        }

        public Func<IEnumerable<IToken>, IToken> MakeToken { get; }
        public TryExitDelegate TryExit { get; }
        public MustExitDelegate MustExit { get; }
        
        public delegate bool MustExitDelegate(out string error);
        public delegate bool TryExitDelegate(char current);

        public static TryExitDelegate CharacterExit(char exitChar) {
            return (char current) => exitChar == current; 
        }

        public static bool NoExit(char current)
        {
            return false;
        }

        public static MustExitDelegate ExitRequired(string errorMessage) {
            bool MustExitDelegate(out string error)
            {
                error = errorMessage;
                return true;
            }
            return MustExitDelegate;
        }

        public static bool DoesNotExit(out string error) { 
                error = default;
                return false;
        }
        
        public static TokenFactory CurlyBracketTokenFactory()
        {
            return new TokenFactory(x => new BacketCompositToken('{', '}', x), CharacterExit('}'), ExitRequired("} not found"));
        }

        public static TokenFactory BrokenBracketTokenFactory()
        {
            return new TokenFactory(x => new BacketCompositToken('<', '>', x), CharacterExit('>'), ExitRequired("> not found"));
        }

        public static TokenFactory SquareBracketTokenFactory()
        {
            return new TokenFactory(x => new BacketCompositToken('[', ']', x), CharacterExit(']'), ExitRequired("] not found"));
        }

        public static TokenFactory ParenthesisTokenFactory()
        {
            return new TokenFactory(x => new BacketCompositToken('(', ')', x), CharacterExit(')'), ExitRequired(") not found"));
        }

        public static TokenFactory CompositTokenFactory()
        {
            return new TokenFactory(x => new CompositToken(x), NoExit, DoesNotExit);
        }

    }

}
