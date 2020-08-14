using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    internal class FileToken : CompositToken
    {
        public FileToken(IToken[] tokens) : base(tokens)
        {
        }
        public override string ToString()
        {
            return $"File({base.ToString()})";
        }

        //public override bool Equals(object? obj)
        //{
        //    return obj is FileToken token && base.Equals(token);
        //}
        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
    }


}
