﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Parser
{
    public static class TokenParser
    {

        public static ICodeElement[] ParseFile(FileToken file, ElementMatchingContext matchingContext)
        {
            return file.Tokens.Select(x => ParseLine((LineToken)x, matchingContext)).ToArray();
        }


        public static ICodeElement ParseLine(LineToken tokens, ElementMatchingContext matchingContext)
        {
            return ParseLine(tokens.Tokens, matchingContext);
        }

        public static ICodeElement ParseLine(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {

            var state = new ParseState(tokens);
            if (state.TryGetStart(out var view))
            {
                return ParseLine(view, matchingContext);
            }
            throw new Exception("there was nothing in that line!");
        }

        public static ICodeElement ParseLine(IParseStateView view, ElementMatchingContext matchingContext)
        {
            ICodeElement lastElement = default;
            do
            {
                lastElement = ParseLineElementOrThrow(view,  lastElement, matchingContext);
            } while (view.TryGetNext(out view));
            return lastElement;
        }

        private static ICodeElement ParseLineElementOrThrow(IParseStateView view, ICodeElement last, ElementMatchingContext matchingContext)
        {
            if (TryParseAtomic(view,  last, matchingContext, out var codeElement))
            {
                return codeElement;
            }
            else if (TryParseElement(view,matchingContext,  out codeElement))
            {
                return codeElement;
            }
            else {
                throw new Exception($"could not parse {view.Token}");
            }
        }


        public static bool TryParseAtomic(IParseStateView view, ICodeElement last, ElementMatchingContext matchingContext, out ICodeElement codeElement)
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
                            ParseLineElementOrThrow(next, last, matchingContext));
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
                            ParseLineElementOrThrow(next, last, matchingContext));

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

        public static ICodeElement[] ParseBlock(CurleyBacketToken token, ElementMatchingContext matchingContext)
        {
            return token.Tokens.Select(x =>
            {
                if (x is LineToken lineToken)
                {
                    return ParseLine(lineToken.Tokens, matchingContext);
                }
                throw new Exception("unexpected token type");
            }).ToArray();
        }
        
        public static bool TryParseElement(IParseStateView view, ElementMatchingContext matchingContext, out ICodeElement codeElement)
        {
            if (view.Token is ElementToken elementToken)
            {
                // smells 
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken)
                {
                    codeElement = ParseLine(parenthesisToken.Tokens, matchingContext);
                    return true;
                }
                
                foreach (var tryMatch in matchingContext.ElementMatchers)
                {
                    if (tryMatch(elementToken, matchingContext, out codeElement))
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