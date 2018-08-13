using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tac.Parser
{
    public class AtomicToken : IToken
    {
        public string Item { get; }

        public AtomicToken(string item) => this.Item = item ?? throw new ArgumentNullException(nameof(item));

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override bool Equals(object obj)
        {
            var token = obj as AtomicToken;
            return token != null &&
                   Item == token.Item;
        }

        public override int GetHashCode() => -979861770 + EqualityComparer<string>.Default.GetHashCode(Item);
    }

}
