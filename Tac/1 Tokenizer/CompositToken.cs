using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{

    public abstract class CompositToken : IToken
    {
        public IEnumerable<IToken> Tokens { get; }

        public CompositToken(IEnumerable<IToken> tokens) => this.Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        
        public override string ToString() => Tokens.Aggregate("",(x,y)=>x + y.ToString() + ",");
        public override bool Equals(object obj)
        {
            return obj is CompositToken other && Tokens.SequenceEqual(other.Tokens);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return 1439444843 + Tokens.Sum(x => x.GetHashCode()); 
            }
        }
    }
    

}
