using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    internal class ElementToken : CompositToken
    {
        public ElementToken(IToken[] tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Ele({base.ToString()})";
        public override bool Equals(object obj)
        {
            return obj is ElementToken token && base.Equals(token);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
    

}
