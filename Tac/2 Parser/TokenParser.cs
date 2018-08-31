using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Parser
{
    public static class TokenParser
    {

        public static ICodeElement[] ParseFile(FileToken file)
        {
            return file.Tokens.Select(x => ParseLine((LineToken)x)).ToArray();
        }


        public static ICodeElement ParseLine(LineToken tokens)
        {
            return ParseLine(tokens.Tokens);
        }

        public static ICodeElement ParseLine(IEnumerable<IToken> tokens)
        {

            var state = new ParseState(tokens);
            if (state.TryGetStart(out var view))
            {
                return ParseLine(view);
            }
            throw new Exception("there was nothing in that line!");
        }

        public static ICodeElement ParseLine(IParseStateView view)
        {
            ICodeElement lastElement = default;
            do
            {
                lastElement = ParseLineElementOrThrow(view,  lastElement);
            } while (view.TryGetNext(out view));
            return lastElement;
        }

        private static ICodeElement ParseLineElementOrThrow(IParseStateView view, ICodeElement last)
        {
            if (TryParseAtomic(view,  last, out var codeElement))
            {
                return codeElement;
            }
            else if (TryParseElement(view,  out codeElement))
            {
                return codeElement;
            }
            else {
                throw new Exception($"could not parse {view.Token}");
            }
        }


        public static bool TryParseAtomic(IParseStateView view, ICodeElement last, out ICodeElement codeElement)
        {
            if (view.Token is AtomicToken atomicToken)
            {

                if (Operations.StandardOperations.Value.BinaryOperations.TryGetValue(atomicToken.Item, out var binaryFunc))
                {
                    if (last == null)
                    {
                        throw new Exception("last required but not provied");
                    }
                    if (view.TryGetNext(out var next))
                    {
                        codeElement = binaryFunc(
                            last,
                            ParseLineElementOrThrow(next,  last));
                    }
                }
                else if (Operations.StandardOperations.Value.LastOperations.TryGetValue(atomicToken.Item, out var lastFunc))
                {

                    if (last == null)
                    {
                        throw new Exception("last required but not provied");
                    }
                    codeElement = lastFunc(
                        last);
                    return true;

                }
                else if (Operations.StandardOperations.Value.NextOperations.TryGetValue(atomicToken.Item, out var nextFunc))
                {
                    if (view.TryGetNext(out var next))
                    {
                        codeElement = nextFunc(
                            ParseLineElementOrThrow(next, last));

                        return true;
                    }
                    else
                    {
                        throw new Exception($"Operation: {atomicToken.Item}, requires next. next is not defined");
                    }
                }
                else if (Operations.StandardOperations.Value.ConstantOperations.TryGetValue(atomicToken.Item, out var action))
                {
                    codeElement = action();
                    return true;
                }
                else
                {
                    throw new Exception($"Operation: {atomicToken.Item}, not known");
                }
            }
            codeElement = default;
            return false;
        }

        public static ICodeElement[] ParseBlock(CurleyBacketToken token)
        {
            return token.Tokens.Select(x =>
            {
                if (x is LineToken lineToken)
                {
                    return ParseLine(lineToken.Tokens);
                }
                throw new Exception("unexpected token type");
            }).ToArray();
        }
        
        public static bool TryParseElement(IParseStateView view, out ICodeElement codeElement)
        {
            if (view.Token is ElementToken elementToken)
            {
                // smells 
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken)
                {
                    codeElement = ParseLine(parenthesisToken.Tokens);
                    return true;
                }
                
                foreach (var tryMatch in Elements.StandardElements.Value.ElementBuilders)
                {
                    if (tryMatch(elementToken, out codeElement))
                    {
                        return true;
                    }
                }
            }

            codeElement = default;
            return false;
        }
    }
}
