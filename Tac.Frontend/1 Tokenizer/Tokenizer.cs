using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Parser
{
    internal class Tokenizer
    {
        private readonly IReadOnlyList<string> operations;

        public Tokenizer(IReadOnlyList<string> operations)
        {
            this.operations = operations ?? throw new ArgumentNullException(nameof(operations));
        }

        //private class ResultAndExitString { }

        private interface IResultAndExitString<out T>
        {
            T GetTokenOrThrow();
            bool HasToken();
            bool TryGetExitString(out string exitString);
        }
        

        private class ResultAndExitString<T>: IResultAndExitString<T>//: ResultAndExitString
            where T :class, IToken
        {
            public ResultAndExitString(string exitString, T result)
            {
                ExitString = exitString ?? throw new ArgumentNullException(nameof(exitString));
                Result = result ?? throw new ArgumentNullException(nameof(result));
            }

            public ResultAndExitString(string exitString)
            {
                ExitString = exitString ?? throw new ArgumentNullException(nameof(exitString));
            }

            public ResultAndExitString(T result)
            {
                Result = result ?? throw new ArgumentNullException(nameof(result));
            }

            public ResultAndExitString()
            {
            }

            private string ExitString { get; }
            private T Result { get; }
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
            public bool TryGetToken(out T token)
            {
                if (Result != null)
                {
                    token = Result;
                    return true;
                }
                token = default;
                return false;
            }
            public T GetTokenOrThrow()
            {
                if (Result == null)
                {
                    throw new Exception("Token not defined");
                }
                return Result;
            }

            public bool HasToken()
            {
                return Result != null;
            }
        }

        private ResultAndExitString<T> OuterTokenzie<T>(
            CharEnumerator enumerator, 
            Func<CharEnumerator, IResultAndExitString<IToken>> inner, 
            Func<string, bool> isExit, 
            Func<IToken[], T> makeToken, 
            bool alwaysMake,
            bool addExitString)
            where T: class,IToken
        {
            var elements = new List<IToken>();
            while (true)
            {
                var res = inner(enumerator);
                if (res.HasToken())
                {
                    elements.Add(res.GetTokenOrThrow());
                }
                if (res.TryGetExitString(out var exitString))
                {
                    if (isExit(exitString))
                    {
                        if (alwaysMake || elements.Any())
                        {
                            return new ResultAndExitString<T>(exitString, makeToken(elements.ToArray()));
                        }
                        else
                        {
                            return new ResultAndExitString<T>(exitString);
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
                        return new ResultAndExitString<T>(makeToken(elements.ToArray()));
                    }
                    else
                    {
                        return new ResultAndExitString<T>();
                    }
                }
            }
        }

        private IResultAndExitString<ElementToken> TokenizeElement(CharEnumerator enumerator)
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
                            return new ResultAndExitString<ElementToken>(part, new ElementToken(elementsParts.ToArray()));
                        }
                        else
                        {
                            return new ResultAndExitString<ElementToken>(part);
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
                        return new ResultAndExitString<ElementToken>(new ElementToken(elementsParts.ToArray()));
                    }
                    else
                    {
                        return new ResultAndExitString<ElementToken>();
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

        private IResultAndExitString<LineToken> TokenzieLine(CharEnumerator enumerator)
        {

            return OuterTokenzie<LineToken>(enumerator, TokenizeElement, IsExit, x => new LineToken(x), false, true);

            bool IsExit(string str)
            {
                return
                    str == ";" ||
                    str == "}" ||
                    str == ")" ||
                    str == "]";
            }
        }
        
        private IResultAndExitString<ParenthesisToken> TokenzieParenthesis(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenizeElement, IsExit, x => new ParenthesisToken(x), false, true);

            bool IsExit(string str)
            {
                return
                    str == ")";
            }
        }
        
        private IResultAndExitString<CurleyBracketToken> TokenzieCurleyBrackets(CharEnumerator enumerator)
        {
            return OuterTokenzie(enumerator, TokenzieLine, IsExit, x => new CurleyBracketToken(x), true, false);

            bool IsExit(string str)
            {
                return
                    str == "}";
            }
        }

        private IResultAndExitString<SquareBacketToken> TokenzieSquareBrackets(CharEnumerator enumerator)
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

        private IResultAndExitString<FileToken> TokenzieFile(CharEnumerator enumerator)
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
                if (buildingPart == "" && enumerator.Current=='"') {
                    return BuildStringConstant(enumerator,out part);
                }

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

        private bool BuildStringConstant(CharEnumerator enumerator,out string part)
        {
            part = enumerator.Current + "";
            while (enumerator.MoveNext() ) {
                part += enumerator.Current;
                if (enumerator.Current == '"') {
                    return true; 
                }
            }
            part = default;
            return false;
        }

        public FileToken Tokenize(string s) {
            return TokenzieFile(s.GetEnumerator()).GetTokenOrThrow();
        }
    }

}
