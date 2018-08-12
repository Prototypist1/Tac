using System;

namespace Tac.Parser
{
    public class AtomicToken : IToken
    {
        public string Item { get; }

        public AtomicToken(string item) => this.Item = item ?? throw new ArgumentNullException(nameof(item));
    }

}
