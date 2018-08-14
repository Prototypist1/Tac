using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class LineToken : CompositToken
    {
        public LineToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Line({base.ToString()})";
        public override bool Equals(object obj)
        {
            return obj is LineToken token && base.Equals(token);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
    

}
