using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    internal class LineToken : CompositToken
    {
        public LineToken(IToken[] tokens) : base(tokens)
        {
        }
        public override string ToString()
        {
            return $"Line({base.ToString()})";
        }

        //public override bool Equals(object? obj)
        //{
        //    return obj is LineToken token && base.Equals(token);
        //}
        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
    }


}
