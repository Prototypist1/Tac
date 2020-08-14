using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{

    internal abstract class CompositToken : IToken
    {
        public IReadOnlyList<IToken> Tokens { get; }

        public CompositToken(IToken[] tokens)
        {
            Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        public override string ToString()
        {
            return Tokens.Aggregate("", (x, y) => x + y.ToString() + ",");
        }

        //public override bool Equals(object? obj)
        //{
        //    return obj is CompositToken other && Tokens.SequenceEqual(other.Tokens);
        //}
        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        var x = 1439444843;
        //        foreach (var item in Tokens)
        //        {
        //            x += item.GetHashCode();
        //        }
        //        return x;
        //    }
        //}
    }
}
