using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class BrokenBracketToken : CompositToken
    {
        public BrokenBracketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Broke({base.ToString()})";
        public override bool Equals(object obj)
        {
            return obj is BrokenBracketToken token && base.Equals(token);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
    

}
