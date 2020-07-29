using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tac.Parser
{
    internal class AtomicToken : IToken
    {
        public string Item { get; }

        public AtomicToken(string item) => this.Item = item ?? throw new ArgumentNullException(nameof(item));

        public override string ToString()=> $"Atom({Item})";

        public override bool Equals(object? obj)
        {
            return obj is AtomicToken token &&
                   Item == token.Item;
        }

        public override int GetHashCode() => -979861770 + EqualityComparer<string>.Default.GetHashCode(Item);
    }
}
