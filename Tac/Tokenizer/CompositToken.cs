using System;
using System.Collections.Generic;

namespace Tac.Parser
{
    public class CompositToken : IToken
    {

        public IEnumerable<IToken> Tokens { get; }

        public CompositToken(IEnumerable<IToken> tokens) => this.Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    }

}
