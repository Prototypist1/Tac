using System;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using static Tac.Parser.Parser;

namespace Tac.Parser
{

    public class ParseContext
    {
        private readonly int index;
        private readonly string thing;
        private readonly ParseContextContext master;
        private Parser Parser { get; }
        private Operations Operations { get; }
        private CodeElement current = new NoELement();

        public CodeElement ToCodeElement()
        {
            if (current is NoELement)
            {
                current = Parser.Whatever(thing, Operations, this);
            }
            return current;

        }

        public ParseContext(int index, string thing, ParseContextContext master, Parser parser, Operations operations)
        {
            this.index = index;
            this.thing = thing ?? throw new ArgumentNullException(nameof(thing));
            this.master = master ?? throw new ArgumentNullException(nameof(master));
            this.Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            this.Operations = operations ?? throw new ArgumentNullException(nameof(operations));
        }

        internal ParseContext Last() => master.GetContext(index - 1);
        internal ParseContext Next() => master.GetContext(index + 1);

    }

}
