using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    public class Tokenizer
    {
        private readonly IReadOnlyList<string> operations;

        public Tokenizer(IReadOnlyList<string> operations)
        {
            this.operations = operations ?? throw new ArgumentNullException(nameof(operations));
        }

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

        private ResultAndExitString OuterTokenzie(
            CharEnumerator enumerator, 
            Func<CharEnumerator, ResultAndExitString> inner, 
            Func<string, bool> isExit, 
            Func<IToken[], IToken> makeToken, 
            bool alwaysMake,
            bool addExitString)
        {
            var elements = new List<IToken>();
            while (true)
            {
                var res = inner(enumerator);
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
                            return new ResultAndExitString(exitString, makeToken(elements.ToArray()));
                        }
                        else
                        {
                            return new ResultAndExitString(exitString);
                        }
                    }
                    else
                    {
                        if (addExitString)
                        {
                            elements.Add(new AtomicToken(exitString));
                        }
                    }
                }
                else
                {
                    if (alwaysMake || elements.Any())
                    {
                        return new ResultAndExitString(makeToken(elements.ToArray()));
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
                            return new ResultAndExitString(part, new ElementToken(elementsParts.ToArray()));
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
                        return new ResultAndExitString(new ElementToken(elementsParts.ToArray()));
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
                    operations.Contains(str);
            }
        }

        private ResultAndExitString TokenzieLine(CharEnumerator enumerator)
        {

            return OuterTokenzie(enumerator, TokenizeElement, IsExit, x => new LineToken(x), false, true);

            bool IsExit(string str)
            {
                return
                    str == ";" ||
                    str == "}" ||
                    str == ")" ||
                    str == "]";
            }
        }
        
        private ResultAndExitString TokenzieParenthesis(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenizeElement, IsExit, x => new ParenthesisToken(x), false, true);

            bool IsExit(string str)
            {
                return
                    str == ")";
            }
        }
        
        private ResultAndExitString TokenzieCurleyBrackets(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new CurleyBacketToken(x), true, false);

            bool IsExit(string str)
            {
                return
                    str == "}";
            }
        }

        private ResultAndExitString TokenzieSquareBrackets(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new SquareBacketToken(x), true, false);

            bool IsExit(string str)
            {
                return
                    str == "]";
            }
        }

        //private ResultAndExitString TokenzieBrokenBrackets(CharEnumerator enumerator)
        //{
        //    return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new BrokenBracketToken(x), true, false);

        //    bool IsExit(string str)
        //    {
        //        return
        //            str == ">";
        //    }
        //}

        private ResultAndExitString TokenzieFile(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new FileToken(x), true, false);

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
    }

}
