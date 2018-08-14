using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class ParenthesisToken : CompositToken
    {
        public ParenthesisToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Par({base.ToString()})";
        public override bool Equals(object obj)
        {
            return obj is ParenthesisToken token && base.Equals(token);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
    

}
