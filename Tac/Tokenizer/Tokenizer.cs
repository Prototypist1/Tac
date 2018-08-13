using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class Tokenizer
    {
        private class ResultAndExitString
        {
            public ResultAndExitString(string exitString, IToken result)
            {
                ExitString = exitString ?? throw new ArgumentNullException(nameof(exitString));
                Result = result ?? throw new ArgumentNullException(nameof(result));
            }

            public ResultAndExitString(string exitString)
            {
                ExitString = exitString ?? throw new ArgumentNullException(nameof(exitString));
            }

            public ResultAndExitString(IToken result)
            {
                Result = result ?? throw new ArgumentNullException(nameof(result));
            }

            public ResultAndExitString()
            {
            }

            private string ExitString { get; }
            private IToken Result { get; }
            public bool TryGetExitString(out string exitString)
            {
                if (ExitString != null)
                {
                    exitString = ExitString;
                    return true;
                }
                exitString = default;
                return false;
            }
            public bool TryGetToken(out IToken token)
            {
                if (Result != null)
                {
                    token = Result;
                    return true;
                }
                token = default;
                return false;
            }
            public IToken GetTokenOrThrow()
            {
                if (Result == null)
                {
                    throw new Exception("Token not defined");
                }
                return Result;
            }
        }

        private ResultAndExitString OuterTokenzie(CharEnumerator enumerator, Func<CharEnumerator, ResultAndExitString> inner, Func<string, bool> isExit, Func<IEnumerable<IToken>, IToken> makeToken, bool alwaysMake)
        {
            var elements = new List<IToken>();
            while (true)
            {
                var res = TokenzieLine(enumerator);
                if (res.TryGetToken(out var token))
                {
                    elements.Add(token);
                }
                if (res.TryGetExitString(out var exitString))
                {
                    if (isExit(exitString))
                    {
                        if (alwaysMake || elements.Any())
                        {
                            return new ResultAndExitString(exitString, makeToken(elements));
                        }
                        else
                        {
                            return new ResultAndExitString(exitString);
                        }
                    }
                    else
                    {
                        elements.Add(new AtomicToken(exitString));
                    }
                }
                else
                {
                    if (alwaysMake || elements.Any())
                    {
                        return new ResultAndExitString(makeToken(elements));
                    }
                    else
                    {
                        return new ResultAndExitString();
                    }
                }
            }
        }

        private ResultAndExitString TokenizeElement(CharEnumerator enumerator)
        {
            var elementsParts = new List<IToken>();
            while (true)
            {
                if (NextPart(enumerator, out var part))
                {
                    if (IsExit(part))
                    {
                        if (elementsParts.Any())
                        {
                            return new ResultAndExitString(part, new ElementToken(elementsParts));
                        }
                        else
                        {
                            return new ResultAndExitString(part);
                        }
                    }
                    if (TryEnter(part, enumerator, out var token))
                    {
                        elementsParts.Add(token);
                    }
                    else
                    {
                        elementsParts.Add(new AtomicToken(part));
                    }
                }
                else
                {
                    if (elementsParts.Any())
                    {
                        return new ResultAndExitString(new ElementToken(elementsParts));
                    }
                    else
                    {
                        return new ResultAndExitString();
                    }
                }
            }

            bool IsExit(string str)
            {
                return
                    str == ";" ||
                    str == "}" ||
                    str == ")" ||
                    str == "]" ||
                    str == ">" ||
                    Operations.StandardOperations.Value.AllOperationKeys.Contains(str);
            }
        }

        private ResultAndExitString TokenzieLine(CharEnumerator enumerator)
        {

            return OuterTokenzie(enumerator, TokenizeElement, IsExit, x => new LineToken(x), false);

            bool IsExit(string str)
            {
                return
                    str == ";" ||
                    str == "}" ||
                    str == ")" ||
                    str == "]" ||
                    str == ">";
            }
        }

        private ResultAndExitString TokenzieCurleyBrackets(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new CurleyBacketToken(x), true);

            bool IsExit(string str)
            {
                return
                    str == "}";
            }
        }

        private ResultAndExitString TokenzieSquareBrackets(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new SquareBacketToken(x), true);

            bool IsExit(string str)
            {
                return
                    str == "]";
            }
        }

        private ResultAndExitString TokenzieBrokenBrackets(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new BrokenBracketToken(x), true);

            bool IsExit(string str)
            {
                return
                    str == ">";
            }
        }

        private ResultAndExitString TokenzieParenthesis(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new ParentThesisToken(x), true);

            bool IsExit(string str)
            {
                return
                    str == ")";
            }
        }

        private ResultAndExitString TokenzieFile(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new FileToken(x), true);

            bool IsExit(string str)
            {
                return false;
            }
        }

        private bool TryEnter(string part, CharEnumerator enumerator, out IToken token)
        {
            if (part == "(")
            {

                token = TokenzieParenthesis(enumerator).GetTokenOrThrow();
                return true;
            }
            else if (part == "<")
            {
                token = TokenzieBrokenBrackets(enumerator).GetTokenOrThrow();
                return true;
            }
            else if (part == "{")
            {
                token = TokenzieCurleyBrackets(enumerator).GetTokenOrThrow();
                return true;
            }
            else if (part == "[")
            {
                token = TokenzieSquareBrackets(enumerator).GetTokenOrThrow();
                return true;
            }
            token = default;
            return false;
        }

        private bool NextPart(CharEnumerator enumerator, out string part)
        {
            var buildingPart = "";
            while (enumerator.MoveNext())
            {
                if (IsNothing(enumerator.Current)) {
                    if (buildingPart != "")
                    {
                        part = buildingPart;
                        return true;
                    }
                    else {
                        continue;
                    }
                }
                buildingPart += enumerator.Current;
            }
            part = default;
            return false;
            
            bool IsNothing(char current) {
                return 
                    current == ' ' ||
                    current == '\t' ||
                    current == '\n' ||
                    current == '\r';
            }
        }

        public IToken Tokenize(string s) {
            return TokenzieFile(s.GetEnumerator()).GetTokenOrThrow();
        }

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
                            if (currentPartial.Count > 1)
                            {
                                tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(currentPartial));
                            }
                            else
                            {
                                tokens.AddRange(currentPartial);
                            }
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
                                        if (currentPartial.Count > 1)
                                        {
                                            tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(currentPartial));
                                        }
                                        else
                                        {
                                            tokens.AddRange(currentPartial);
                                        }
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
                        if (currentPartial.Count > 1)
                        {
                            tokens.Add(TokenFactory.CompositTokenFactory().MakeToken(currentPartial));
                        }
                        else
                        {
                            tokens.AddRange(currentPartial);
                        }
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
