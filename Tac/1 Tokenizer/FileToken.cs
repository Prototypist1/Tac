using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class FileToken : CompositToken
    {
        public FileToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"File({base.ToString()})";
        public override bool Equals(object obj)
        {
            return obj is FileToken token && base.Equals(token);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
    

}
