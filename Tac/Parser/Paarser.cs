using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using static Tac.Parser.Parser;

namespace Tac.Parser
{
    public class TokenFactory
    {
        public Func<IEnumerable<IToken>, IToken> MakeToken { get; }
        public TryExitDelegate TryExit { get; }
        public MustExitDelegate MustExit { get; }


        public delegate bool MustExitDelegate(out string error);
        public delegate bool TryExitDelegate(string current);

        public static TokenFactory CurlyBracketTokenFactory()
        {

        }

        public static TokenFactory BrokenBracketTokenFactory()
        {

        }

        public static TokenFactory SquareBracketTokenFactory()
        {

        }

        public static TokenFactory ParenthesisTokenFactory()
        {

        }

        public static TokenFactory CompositTokenFactory()
        {

        }

    }

    public interface IToken
    {

    }

    public class AtomicToken : IToken
    {
        public string Item { get; }

        public AtomicToken(string item) => this.Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public class CompositToken : IToken
    {

        public IEnumerable<IToken> Tokens { get; }

        public CompositToken(IEnumerable<IToken> tokens) => this.Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    }

    public class BacketCompositToken : CompositToken
    {
        public BacketCompositToken(string openBracket, string closeBracket, IEnumerable<IToken> tokens) : base(tokens)
        {
            OpenBracket = openBracket ?? throw new ArgumentNullException(nameof(openBracket));
            CloseBracket = closeBracket ?? throw new ArgumentNullException(nameof(closeBracket));
        }

        public string OpenBracket { get; }
        public string CloseBracket { get; }

    }

    public static class Parser
    {

        public static Dictionary<string, Func<ParseContext, ParseContext, CodeElement>> binaryOperations = new Dictionary<string, Func<ParseContext, ParseContext, CodeElement>> { };
        public static Dictionary<string, Func<ParseContext, CodeElement>> nextOperations = new Dictionary<string, Func<ParseContext, CodeElement>> { };
        public static Dictionary<string, Func<ParseContext, CodeElement>> lastOperations = new Dictionary<string, Func<ParseContext, CodeElement>> { };
        public static Dictionary<string, Func<CodeElement>> constantOperations = new Dictionary<string, Func<CodeElement>> { };

        public static IEnumerable<string> AllOperationKeys
        {
            get
            {

                foreach (var item in binaryOperations.Keys)
                {
                    yield return item;
                }
                foreach (var item in nextOperations.Keys)
                {
                    yield return item;
                }
                foreach (var item in lastOperations.Keys)
                {
                    yield return item;
                }
                foreach (var item in constantOperations.Keys)
                {
                    yield return item;
                }
            }
        }

        public static CodeElement ParseLine(this string s)
        {
            var elements = s.SmartSplitInclusive(AllOperationKeys).ToArray();
            var contextContext = new ParseContextContext(elements);
            foreach (var item in contextContext)
            {
                item.ToCodeElement();
            }
        }
        public static CodeElement Whatever(this string value, ParseContext context)
        {
            if (binaryOperations.ContainsKey(value))
            {
                return binaryOperations[value](context.last(), context.next());
            }
            if (nextOperations.ContainsKey(value))
            {
                return nextOperations[value](context.next());
            }
            if (nextOperations.ContainsKey(value))
            {
                return lastOperations[value](context.last());
            }
            if (constantOperations.ContainsKey(value))
            {
                return constantOperations[value]();
            }
            return value.ParseElement();
        }


        private static IToken SmartSplitBase(this string s, IEnumerable<string> specials)
        {
            var enumerator = s.GetEnumerator();
            return Inner(TokenFactory.CompositTokenFactory());

            IToken Inner(TokenFactory tokenFactory)
            {
                var tokens = new List<IToken>();
                var current = "";
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == '{')
                    {
                        tokens.AddRange(WhiteSpaceSplit(current));
                        tokens.Add(Inner(TokenFactory.CurlyBracketTokenFactory()));
                    }
                    else if (enumerator.Current == '(')
                    {
                        tokens.AddRange(WhiteSpaceSplit(current));
                        tokens.Add(Inner(TokenFactory.ParenthesisTokenFactory()));
                    }
                    else if (enumerator.Current == '[')
                    {
                        tokens.AddRange(WhiteSpaceSplit(current));
                        tokens.Add(Inner(TokenFactory.SquareBracketTokenFactory()));
                    }
                    else if (enumerator.Current == '<')
                    {
                        tokens.AddRange(WhiteSpaceSplit(current));
                        tokens.Add(Inner(TokenFactory.BrokenBracketTokenFactory()));
                    }

                    else if (enumerator.Current == ';')
                    {
                        tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(WhiteSpaceSplit(current)));
                        tokens.Add(Inner(TokenFactory.BrokenBracketTokenFactory()));
                    }


                    if (tokenFactory.TryExit(current))
                    {
                        var whiteSpaceTokens = WhiteSpaceSplit(current);
                        if (whiteSpaceTokens.Any())
                        {
                            tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(whiteSpaceTokens));
                        }
                        return tokenFactory.MakeToken(tokens);
                    }

                    current += enumerator.Current;
                    foreach (var special in specials)
                    {
                        if (current.EndsWith(special))
                        {
                            var startsAt = current.IndexOf(special);
                            if (startsAt != 0)
                            {
                                var whiteSpaceTokens = WhiteSpaceSplit(current.Substring(0, startsAt));
                                if (whiteSpaceTokens.Any())
                                {
                                    tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(whiteSpaceTokens));
                                }
                                tokens.Add(new AtomicToken(special));
                            }
                        }
                    }
                }
                if (tokenFactory.MustExit(out var error))
                {
                    throw new Exception(error);
                }
                return tokenFactory.MakeToken(WhiteSpaceSplit(current));
            }
        }

        private static IToken[] WhiteSpaceSplit(string current)
        {
            return current.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new AtomicToken(x)).ToArray();
        }

        public static CodeElement ParseElement(this string s) => throw new NotImplementedException();

    }

    public class ParseContextContext : IEnumerable<ParseContext>
    {
        private readonly (int index, ParseContext parseContext)[] backing;

        public ParseContextContext(IEnumerable<string> strings)
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

        internal ParseContext GetContext(int i)
        {
            return backing[i].parseContext;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ParseContext
    {
        private readonly int index;
        private readonly string thing;
        private readonly ParseContextContext master;
        private CodeElement current = new NoELement();

        public void ToCodeElement()
        {
            current = thing.Whatever(this);
        }

        public ParseContext(int index, string thing, ParseContextContext master)
        {
            this.index = index;
            this.thing = thing ?? throw new ArgumentNullException(nameof(thing));
            this.master = master ?? throw new ArgumentNullException(nameof(master));
        }

        internal ParseContext last() => master.GetContext(index - 1);
        internal ParseContext next() => master.GetContext(index + 1);

    }

}
