using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class CurleyBacketToken : CompositToken
    {
        public CurleyBacketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Curl({base.ToString()})";
        public override bool Equals(object obj)
        {
            return obj is CurleyBacketToken token && base.Equals(token);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
    

}
