using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class Tokenizer
    {
        public IToken SmartSplitBase(string s, IEnumerable<string> specials)
        {
            var enumerator = s.GetEnumerator();
            return Inner(TokenFactory.CompositTokenFactory());

            IToken Inner(TokenFactory tokenFactory)
            {
                var composites = new List<IToken>();
                var tokens = new List<IToken>();
                var currentPartial = new List<IToken>();
                var current = "";
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == '{')
                    {
                        currentPartial.AddRange(WhiteSpaceSplit(current));
                        currentPartial.Add(Inner(TokenFactory.CurlyBracketTokenFactory()));
                        current = "";
                    }
                    else if (enumerator.Current == '(')
                    {
                        currentPartial.AddRange(WhiteSpaceSplit(current));
                        currentPartial.Add(Inner(TokenFactory.ParenthesisTokenFactory()));
                        current = "";
                    }
                    else if (enumerator.Current == '[')
                    {
                        currentPartial.AddRange(WhiteSpaceSplit(current));
                        currentPartial.Add(Inner(TokenFactory.SquareBracketTokenFactory()));
                        current = "";
                    }
                    else if (enumerator.Current == '<')
                    {
                        currentPartial.AddRange(WhiteSpaceSplit(current));
                        currentPartial.Add(Inner(TokenFactory.BrokenBracketTokenFactory()));
                        current = "";
                    }
                    else if (enumerator.Current == ';')
                    {
                        var whiteSpaceTokens = WhiteSpaceSplit(current);
                        if (whiteSpaceTokens.Any())
                        {
                            currentPartial.Add(TokenFactory.CompositTokenFactory().MakeToken(whiteSpaceTokens));
                        }
                        if (currentPartial.Any())
                        {
                            if (tokens.Any())
                            {
                                tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(currentPartial));
                            }
                            else
                            {
                                tokens = currentPartial;
                            }
                        }
                        tokens.Add(new AtomicToken(";"));
                        if (tokens.Any())
                        {
                            composites.Add(TokenFactory.CompositTokenFactory().MakeToken(tokens));
                        }

                        currentPartial = new List<IToken> { };
                        tokens = new List<IToken> { };
                        current = "";
                    }
                    else if (tokenFactory.TryExit(enumerator.Current))
                    {
                        var whiteSpaceTokens = WhiteSpaceSplit(current);
                        if (whiteSpaceTokens.Any())
                        {
                            if (currentPartial.Any())
                            {
                                currentPartial.AddRange(whiteSpaceTokens);
                            }
                            else
                            {
                                currentPartial = whiteSpaceTokens.ToList();
                            }
                        }
                        if (currentPartial.Any())
                        {
                            tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(currentPartial));
                        }
                        if (tokens.Any())
                        {
                            composites.Add(TokenFactory.CompositTokenFactory().MakeToken(tokens));
                        }
                        return tokenFactory.MakeToken(composites);
                    }
                    else
                    {
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
                                        if (currentPartial.Any())
                                        {
                                            currentPartial.AddRange(whiteSpaceTokens);
                                        }
                                        else
                                        {
                                            currentPartial = whiteSpaceTokens.ToList();
                                        }
                                    }
                                    if (currentPartial.Any())
                                    {
                                        tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(currentPartial));
                                    }
                                    tokens.Add(new AtomicToken(special));
                                }

                                currentPartial = new List<IToken> { };
                                current = "";
                            }
                        }
                    }
                }
                if (tokenFactory.MustExit(out var error))
                {
                    throw new Exception(error);
                }
                {
                    var whiteSpaceTokens = WhiteSpaceSplit(current);
                    if (whiteSpaceTokens.Any())
                    {
                        if (currentPartial.Any())
                        {
                            currentPartial.AddRange(whiteSpaceTokens);
                        }
                        else
                        {
                            currentPartial = whiteSpaceTokens.ToList();
                        }
                    }
                    if (currentPartial.Any())
                    {
                        tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(currentPartial));
                    }
                    if (tokens.Any())
                    {
                        composites.Add(TokenFactory.CompositTokenFactory().MakeToken(tokens));
                    }
                }
                return tokenFactory.MakeToken(composites);
            }
        }

        private IToken[] WhiteSpaceSplit(string current)
        {
            return current.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new AtomicToken(x)).ToArray();
        }
    }

}
