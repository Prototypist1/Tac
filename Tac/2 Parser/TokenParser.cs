using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static ICodeElement ParseLine(IEnumerable<IToken> tokens) {
            
            var state = new ParseState(tokens);
            if (state.TryGetStart(out var view)) {
                return ParseLine(view);
            }
            throw new Exception("there was nothing in that line!");
        }

        public static ICodeElement ParseLine(IParseStateView view) {
            IParseStateView lastView;
            do
            {
                lastView = view;
                ParseLineElementOrThrow(view);
            } while (view.TryGetNext(out view));
            return WalkBackwords(lastView);
        }

        private static ICodeElement ParseLineElementOrThrow(IParseStateView view)
        {
            if (TryParseAtomic(view)) { }
            else if (TryParseElement(view)) { }
            return view.GetCodeElementOrThrow();
        }

        private static ICodeElement WalkBackwords(IParseStateView view) {
            var viewElement = view.GetCodeElement(() => ParseLineElementOrThrow(view));

            while (view.TryGetLast(out view)) {
                var lastViewElement = view.GetCodeElementOrThrow();

                if (lastViewElement.ContainsInTree(viewElement))
                {
                    viewElement = lastViewElement;
                }
                
            }
            return viewElement;

        }

        public static bool TryParseAtomic(IParseStateView view) {
            if (view.Token is AtomicToken atomicToken) {

                if (Operations.StandardOperations.Value.BinaryOperations.TryGetValue(atomicToken.Item, out var binaryFunc))
                {
                    if (view.TryGetLast(out var last) && view.TryGetNext(out var next))
                    {
                        view.GetCodeElement(() => binaryFunc(
                            last.GetCodeElement(() => WalkBackwords(last)),
                            next.GetCodeElement(() => ParseLineElementOrThrow(next))));
                    }
                }
                else if (Operations.StandardOperations.Value.LastOperations.TryGetValue(atomicToken.Item, out var lastFunc))
                {
                    if (view.TryGetLast(out var last))
                    {
                        view.GetCodeElement(() => lastFunc(
                            last.GetCodeElement(() => WalkBackwords(last))));
                    }
                }
                else if (Operations.StandardOperations.Value.NextOperations.TryGetValue(atomicToken.Item, out var nextFunc))
                {
                    if (view.TryGetNext(out var next))
                    {
                        view.GetCodeElement(() => nextFunc(
                            next.GetCodeElement(() => ParseLineElementOrThrow(next))));
                    }
                }
                else if (Operations.StandardOperations.Value.ConstantOperations.TryGetValue(atomicToken.Item, out var action))
                {
                    view.GetCodeElement(action);
                }
                else {
                    throw new Exception($"Operation: {atomicToken.Item}, not known");
                }

                return true;
            }
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

        public static bool TryParseElement(IParseStateView view)
        {
            if (view.Token is ElementToken elementToken) {
                // smells 
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken) {
                    view.GetCodeElement(() => ParseLine(parenthesisToken.Tokens));
                }

                view.GetCodeElement(() => {
                    foreach (var tryMatch in Elements.StandardElements.Value.ElementBuilders)
                    {
                        if (tryMatch(elementToken, out var codeElement)){
                            return codeElement;
                        }
                    }
                    throw new Exception("no match found");
                });

                return true;
            }
            return false;
        }
    }
}
