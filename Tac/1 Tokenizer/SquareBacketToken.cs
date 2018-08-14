using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class SquareBacketToken : CompositToken
    {
        public SquareBacketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Square({base.ToString()})";
        public override bool Equals(object obj)
        {
            return obj is SquareBacketToken token && base.Equals(token);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
}
