using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    internal class SquareBacketToken : CompositToken
    {
        public SquareBacketToken(IToken[] tokens) : base(tokens)
        {
        }
        public override string ToString()
        {
            return $"Square({base.ToString()})";
        }

        public override bool Equals(object? obj)
        {
            return obj is SquareBacketToken token && base.Equals(token);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
