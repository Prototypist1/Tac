using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tac.Parser
{
    public class AtomicToken : IToken
    {
        public string Item { get; }

        public AtomicToken(string item) => this.Item = item ?? throw new ArgumentNullException(nameof(item));

        public override string ToString()=> $"Atom({Item})";
        public override bool Equals(object obj)=> ToString() == obj.ToString();
        public override int GetHashCode() => ToString().GetHashCode();
    }

}
