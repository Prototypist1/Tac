using Newtonsoft.Json;
using System;

namespace Tac.Parser
{
    public class AtomicToken : IToken
    {
        public string Item { get; }

        public AtomicToken(string item) => this.Item = item ?? throw new ArgumentNullException(nameof(item));

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

}
