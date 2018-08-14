using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Parser
{
    public static class TokenParser
    {

        private static ICodeElement ParseLine(IEnumerable<IToken> tokens) => throw new NotImplementedException();

        public static ICodeElement ParseLine(IParseStateView view) {
            do
            {
                ParseLineElementOrThrow(view);
            } while (view.TryGetNext(out view));
        }

        private static ICodeElement ParseLineElementOrThrow(IParseStateView view)
        {
            if (TryParseAtomic(view)) { }
            else if (TryParseElement(view)) { }
            return view.GetCodeElementOrThrow();
        }

        private static ICodeElement WalkBackwords(IParseStateView view) {
            var viewElement = view.GetCodeElement(() => ParseLineElementOrThrow(view));

            while (view.TryGetNext(out var lastView)) {
                var lastViewElement = lastView.GetCodeElement(() => ParseLineElementOrThrow(lastView));

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
