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
            return file.Tokens.Select(x => ParseLine((LineToken)x,new StaticScope(new RootScope()))).ToArray();
        }
        

        public static ICodeElement ParseLine(LineToken tokens, IScope enclosingScope)
        {
            return ParseLine(tokens.Tokens, enclosingScope);
        }

        public static ICodeElement ParseLine(IEnumerable<IToken> tokens, IScope enclosingScope) {
            
            var state = new ParseState(tokens);
            if (state.TryGetStart(out var view)) {
                return ParseLine(view, enclosingScope);
            }
            throw new Exception("there was nothing in that line!");
        }

        public static ICodeElement ParseLine(IParseStateView view, IScope enclosingScope) {
            IParseStateView lastView;
            do
            {
                lastView = view;
                ParseLineElementOrThrow(view, enclosingScope);
            } while (view.TryGetNext(out view));
            return WalkBackwords(lastView);
        }

        private static ICodeElement ParseLineElementOrThrow(IParseStateView view, IScope enclosingScope)
        {
            if (TryParseAtomic(view,enclosingScope)) { }
            else if (TryParseElement(view, enclosingScope)) { }
            return view.GetCodeElementOrThrow();
        }

        private static ICodeElement WalkBackwords(IParseStateView view) {
            var viewElement = view.GetCodeElementOrThrow();

            while (view.TryGetLast(out view)) {
                var lastViewElement = view.GetCodeElementOrThrow();

                if (lastViewElement.ContainsInTree(viewElement))
                {
                    viewElement = lastViewElement;
                }
                
            }
            return viewElement;

        }

        public static bool TryParseAtomic(IParseStateView view, IScope scope) {
            if (view.Token is AtomicToken atomicToken) {

                if (Operations.StandardOperations.Value.BinaryOperations.TryGetValue(atomicToken.Item, out var binaryFunc))
                {
                    if (view.TryGetLast(out var last) && view.TryGetNext(out var next))
                    {
                        view.GetCodeElement(() => binaryFunc(
                            WalkBackwords(last),
                            next.GetCodeElement(() => ParseLineElementOrThrow(next, scope))));
                    }
                }
                else if (Operations.StandardOperations.Value.LastOperations.TryGetValue(atomicToken.Item, out var lastFunc))
                {
                    if (view.TryGetLast(out var last))
                    {
                        view.GetCodeElement(() => lastFunc(
                            WalkBackwords(last)));
                    }
                }
                else if (Operations.StandardOperations.Value.NextOperations.TryGetValue(atomicToken.Item, out var nextFunc))
                {
                    if (view.TryGetNext(out var next))
                    {
                        view.GetCodeElement(() => nextFunc(
                            next.GetCodeElement(() => ParseLineElementOrThrow(next, scope))));
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

        public static ICodeElement[] ParseBlock(CurleyBacketToken token, IScope scope)
        {
            return token.Tokens.Select(x =>
            {
                if (x is LineToken lineToken)
                {
                    return ParseLine(lineToken.Tokens, scope);
                }
                throw new Exception("unexpected token type");
            }).ToArray();
        }

        public static bool TryParseElement(IParseStateView view, IScope enclosingScope)
        {
            if (view.Token is ElementToken elementToken) {
                // smells 
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken) {
                    view.GetCodeElement(() => ParseLine(parenthesisToken.Tokens, enclosingScope));
                }

                view.GetCodeElement(() => {
                    foreach (var tryMatch in Elements.StandardElements.Value.ElementBuilders)
                    {
                        if (tryMatch(elementToken, enclosingScope, out var codeElement)){
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
