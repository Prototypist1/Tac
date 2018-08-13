using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    // TODO update equals 

    public class SquareBacketToken : CompositToken
    {
        public SquareBacketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
    }

    public class FileToken : CompositToken
    {
        public FileToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
    }

    public class ParentThesisToken : CompositToken
    {
        public ParentThesisToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
    }
    
    public class CurleyBacketToken : CompositToken
    {
        public CurleyBacketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
    }

    public class BrokenBracketToken : CompositToken
    {
        public BrokenBracketToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
    }


    public class LineToken : CompositToken
    {
        public LineToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
    }

    public class ElementToken : CompositToken
    {
        public ElementToken(IEnumerable<IToken> tokens) : base(tokens)
        {
        }
    }

    public abstract class CompositToken : IToken
    {
        public IEnumerable<IToken> Tokens { get; }

        public CompositToken(IEnumerable<IToken> tokens) => this.Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));

        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }

        public override bool Equals(object obj)
        {
            var token = obj as CompositToken;
            return token != null &&
                   Enumerable.SequenceEqual(Tokens, token.Tokens);
        }

        public override int GetHashCode() {
            unchecked {
                return Tokens?.Sum(x => x.GetHashCode()) ?? 0;
            }
        }
    }

}
