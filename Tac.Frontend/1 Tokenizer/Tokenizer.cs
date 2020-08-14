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

        private interface IResultAndExitString<out T>
        {
            T GetTokenOrThrow();
            bool HasToken();
            bool TryGetExitString(out string? exitString);
        }


        private class ResultAndExitString<T> : IResultAndExitString<T>//: ResultAndExitString
            where T : class, IToken
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

            private string? ExitString { get; }
            private T? Result { get; }
            public bool TryGetExitString(out string? exitString)
            {
                if (ExitString != null)
                {
                    exitString = ExitString;
                    return true;
                }
                exitString = default;
                return false;
            }
            public bool TryGetToken(out T? token)
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

        private delegate IResultAndExitString<IToken> Inner(ref StringWalker stringWalker);

        private ResultAndExitString<T> OuterTokenzie<T>(
            ref StringWalker enumerator,
            Inner inner,
            IReadOnlyList<string> exits,
            Func<IToken[], T> makeToken,
            bool alwaysMake,
            bool addExitString)
            where T : class, IToken
        {
            var elements = new List<IToken>();
            while (true)
            {
                var res = inner(ref enumerator);
                if (res.HasToken())
                {
                    elements.Add(res.GetTokenOrThrow());
                }
                if (res.TryGetExitString(out var exitString))
                {
                    if (exits.Contains(exitString))
                    {
                        if (alwaysMake || elements.Any())
                        {
                            return new ResultAndExitString<T>(exitString!, makeToken(elements.ToArray()));
                        }
                        else
                        {
                            return new ResultAndExitString<T>(exitString!);
                        }
                    }
                    else
                    {
                        if (addExitString)
                        {
                            elements.Add(new AtomicToken(exitString!));
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

        private IResultAndExitString<LineToken> TokenzieLine(ref StringWalker enumerator)
        {

            var elementsParts = new List<IToken>();
            while (true)
            {
                if (NextPart(ref enumerator, out var part))
                {
                    if (IsExit(part!))
                    {
                        if (elementsParts.Any())
                        {
                            return new ResultAndExitString<LineToken>(part!, new LineToken(elementsParts.ToArray()));
                        }
                        else
                        {
                            return new ResultAndExitString<LineToken>(part!);
                        }
                    }
                    if (TryEnter(part!, ref enumerator, out var token))
                    {
                        elementsParts.Add(token!);
                    }
                    else
                    {
                        elementsParts.Add(new AtomicToken(part!));
                    }
                }
                else
                {
                    if (elementsParts.Any())
                    {
                        return new ResultAndExitString<LineToken>(new LineToken(elementsParts.ToArray()));
                    }
                    else
                    {
                        return new ResultAndExitString<LineToken>();
                    }
                }
            }

            bool IsExit(string str)
            {
                return
                    str == ";" ||
                    str == "}" ||
                    str == ")" ||
                    str == "]";
            }
        }

        private IResultAndExitString<ParenthesisToken> TokenzieParenthesis(ref StringWalker enumerator)
        {
            return OuterTokenzie(ref enumerator, TokenzieLine, new[] { ")", }, x => new ParenthesisToken(x), false, true);
        }

        private IResultAndExitString<CurleyBracketToken> TokenzieCurleyBrackets(ref StringWalker enumerator)
        {
            return OuterTokenzie(ref enumerator, TokenzieLine, new[] { "}", }, x => new CurleyBracketToken(x), true, false);
        }

        private IResultAndExitString<SquareBacketToken> TokenzieSquareBrackets(ref StringWalker enumerator)
        {
            return OuterTokenzie(ref enumerator, TokenzieLine, new[] { "]", }, x => new SquareBacketToken(x), true, false);
        }

        private IResultAndExitString<FileToken> TokenzieFile(ref StringWalker enumerator)
        {
            return OuterTokenzie(ref enumerator, TokenzieLine, Array.Empty<string>(), x => new FileToken(x), true, false);

        }

        private bool TryEnter(string part, ref StringWalker enumerator, out IToken? token)
        {
            if (part == "(")
            {

                token = TokenzieParenthesis(ref enumerator).GetTokenOrThrow();
                return true;
            }
            else if (part == "{")
            {
                token = TokenzieCurleyBrackets(ref enumerator).GetTokenOrThrow();
                return true;
            }
            else if (part == "[")
            {
                token = TokenzieSquareBrackets(ref enumerator).GetTokenOrThrow();
                return true;
            }
            token = default;
            return false;
        }

        private bool NextPart(ref StringWalker enumerator, out string? part)
        {
            var buildingPart = "";
            while (enumerator.TryPeek(out var next))
            {
                if (buildingPart.Length == 0 && next == '"') {
                    return BuildStringConstant(ref enumerator, out part);
                }

                var numberWalker = new StringWalker(enumerator);
                if (buildingPart.Length == 0 && IsNumber(ref numberWalker, out var num)) {
                    enumerator.Update(numberWalker);
                    part = num;
                    return true;
                }

                if (IsOperationOrExit(ref enumerator, out var exit))
                {
                    if (buildingPart.Length != 0)
                    {
                        part = buildingPart;
                        return true;
                    }
                    else
                    {
                        while (buildingPart != exit)
                        {
                            if (enumerator.TryTake(out var innerNext))
                            {
                                buildingPart += innerNext;
                            }
                            else
                            {
                                throw new Exception("bug!");
                            }
                        }
                        part = buildingPart;
                        return true;
                    }
                }

                if (IsNothing(next)) {
                    if (buildingPart.Length != 0)
                    {
                        part = buildingPart;
                        return true;
                    }
                    else {
                        if (!enumerator.TryTake(out var _))
                        {
                            throw new Exception("bug!");
                        }

                        continue;
                    }
                }

                if (!enumerator.TryTake(out var _))
                {
                    throw new Exception("bug!");
                }

                buildingPart += next;

            }
            part = default;
            return false;

            
        }

        private static bool IsNothing(char next)
        {
            return
                next == ' ' ||
                next == '\t' ||
                next == '\n' ||
                next == '\r';
        }

        private static bool IsNumber(ref StringWalker stringWalker, out string? number)
        {
            var myWalker = new StringWalker(stringWalker);

            var res = "";
            var numbers = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            if (!myWalker.TryTake(out var first))
            {
                number = default;
                return false;
            }

            if (!numbers.Contains(first))
            {
                number = default;
                return false;
            }

            res += first;
            stringWalker.Update(myWalker);
            

            var hasDecimelPoint = false;

            
            while (myWalker.TryTake(out var peeked))
            {
                if (!hasDecimelPoint && peeked == '.' && myWalker.TryPeek(out var next) && numbers.Contains(next))
                {
                    hasDecimelPoint = true;
                    res += peeked;
                }
                else if (numbers.Contains(peeked))
                {
                    res += peeked;
                    stringWalker.Update(myWalker);
                }
                else 
                {
                    number = res;
                    return true;
                }
            }
            throw new Exception("bug!");

        }

        private bool IsOperationOrExit(ref StringWalker stringWalker, out string? exitString)
        {
            foreach (var op in operations.Union(new[] { "{", "}", "(", ")", "[", "]", ";", "," }).OrderByDescending(x=>x.Length))
            {
                if (stringWalker.Span().StartsWith(op))
                {
                    exitString = op;
                    return true;
                }
            }
            exitString = default;
            return false;
        }

        private static bool BuildStringConstant(ref StringWalker enumerator, out string? part)
        {
            part = "";

            if (!enumerator.TryTake(out var first) || first != '"') {
                part = default;
                return false;
            }

            part += first;

            while (enumerator.TryTake(out var next)) {
                part += next;
                if (next == '"') {
                    return true;
                }
            }
            part = default;
            return false;
        }

        public FileToken Tokenize(string s) {
            var stringWalker = new StringWalker(s);
            return TokenzieFile(ref stringWalker).GetTokenOrThrow();
        }

        private ref struct StringWalker{

            private ReadOnlySpan<char> backing;

            public StringWalker(ReadOnlySpan<char> backing) {
                this.backing = backing;
            }

            public StringWalker(StringWalker other):this(other.backing)
            {
            }

            public void Update(ReadOnlySpan<char> backing)
            {
                this.backing = backing;
            }
            public void Update(StringWalker other)
            {
                this.backing = other.backing;
            }

            public bool TryTake(out char character) {
                if (!TryPeek(out character)) {
                    return false;
                }
                backing = backing.Slice(1);
                return true;
            }

            public bool HasNext() {
                return backing.Length ==0;
            }

            public bool TryPeek(out char character)
            {
                if (HasNext())
                {
                    character = default;
                    return false;
                }
                character = backing[0];
                return true;
            }

            public ReadOnlySpan<char> Span() {
                return backing;
            }
        }
    }
}
