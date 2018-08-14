using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Parser
{
    public class ParseContextContext : IEnumerable<ParseContext>
    {
        private readonly (int index, ParseContext parseContext)[] backing;


        public ParseContextContext(IEnumerable<IToken> tokens)
        {
            backing = new(int index, ParseContext parseContext)[strings.Count()];
            for (int i = 0; i < strings.Count(); i++)
            {
                backing[i] = (i, new ParseContext(i, strings.ElementAt(i), this));
            }
        }

        public IEnumerator<ParseContext> GetEnumerator()
        {
            foreach (var item in backing)
            {
                yield return item.parseContext;
            }
        }

        public CodeElement Whatever(string value, Operations operations, ParseContext context)
        {
            if (Operations.StandardOperations.Value.BinaryOperations.ContainsKey(value))
            {
                return Operations.StandardOperations.Value.BinaryOperations[value](context.Last(), context.Next());
            }
            if (Operations.StandardOperations.Value.NextOperations.ContainsKey(value))
            {
                return Operations.StandardOperations.Value.NextOperations[value](context.Next());
            }
            if (Operations.StandardOperations.Value.NextOperations.ContainsKey(value))
            {
                return Operations.StandardOperations.Value.LastOperations[value](context.Last());
            }
            if (Operations.StandardOperations.Value.ConstantOperations.ContainsKey(value))
            {
                return Operations.StandardOperations.Value.ConstantOperations[value]();
            }
            return ParseElement(value);
        }

        public CodeElement ParseElement(string s) => throw new NotImplementedException();

        internal ParseContext GetContext(int i)
        {
            return backing[i].parseContext;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
