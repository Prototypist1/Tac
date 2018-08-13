using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    // these are totally not really sub classes
    // just an enum or something 
    
    public class SquareBacketToken : CompositToken
    {
        public SquareBacketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Square({base.ToString()})";
    }

    public class FileToken : CompositToken
    {
        public FileToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"File({base.ToString()})";
    }

    public class ParentThesisToken : CompositToken
    {
        public ParentThesisToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Par({base.ToString()})";
    }
    
    public class CurleyBacketToken : CompositToken
    {
        public CurleyBacketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Curl({base.ToString()})";
    }

    public class BrokenBracketToken : CompositToken
    {
        public BrokenBracketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Broke({base.ToString()})";
    }
    
    public class LineToken : CompositToken
    {
        public LineToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Line({base.ToString()})";
    }

    public class ElementToken : CompositToken
    {
        public ElementToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
        public override string ToString() => $"Ele({base.ToString()})";
    }

    public abstract class CompositToken : IToken
    {
        public IEnumerable<IToken> Tokens { get; }

        public CompositToken(IEnumerable<IToken> tokens) => this.Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        
        public override string ToString() => Tokens.Aggregate("",(x,y)=>x + y.ToString() + ",");
        // this is stupid and dangerous ! TODO!
        public override bool Equals(object obj) => ToString() == obj.ToString();
        public override int GetHashCode() => ToString().GetHashCode();
    }
    

}
