using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class ParseContextContext : IEnumerable<ParseContext>
    {
        private readonly (int index, ParseContext parseContext)[] backing;


        public ParseContextContext(IEnumerable<string> strings, Parser parser, Operations operations)
        {
            backing = new(int index, ParseContext parseContext)[strings.Count()];
            for (int i = 0; i < strings.Count(); i++)
            {
                backing[i] = (i, new ParseContext(i, strings.ElementAt(i), this, parser, operations));
            }
        }

        public IEnumerator<ParseContext> GetEnumerator()
        {
            foreach (var item in backing)
            {
                yield return item.parseContext;
            }
        }

        internal ParseContext GetContext(int i)
        {
            return backing[i].parseContext;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
