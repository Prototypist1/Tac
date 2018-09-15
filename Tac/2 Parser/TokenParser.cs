//using Prototypist.LeftToRight;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tac.Semantic_Model;
//using Tac.Semantic_Model.CodeStuff;

//namespace Tac.Parser
//{
//    public static class TokenParser
//    {

//        public static ICodeElement[] ParseFile(FileToken file, ElementMatchingContext matchingContext)
//        {
//            return file.Tokens.Select(x => ParseLine((LineToken)x, matchingContext)).ToArray();
//        }


//        public static ICodeElement ParseLine(LineToken tokens, ElementMatchingContext matchingContext)
//        {
//            return ParseLine(tokens.Tokens, matchingContext);
//        }

//        public static ICodeElement ParseLine(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
//        {
//            var state = new ParseState(tokens);
//            if (state.TryGetStart(out var view))
//            {
//                return ParseLine(view, matchingContext);
//            }
//            throw new Exception("there was nothing in that line!");
//        }

//        public static ICodeElement ParseLine(IParseStateView view, ElementMatchingContext matchingContext)
//        {
//            object lastElement = default;
//            do
//            {
//                lastElement = ParseLineElementOrThrow(view,  lastElement, matchingContext);
//            } while (view.TryGetNext(out view));
//            return lastElement.Cast<ICodeElement>();
//        }

//        private static object ParseLineElementOrThrow(IParseStateView view, object last, ElementMatchingContext matchingContext)
//        {
//            if (TryParseAtomic(view,  last, matchingContext, out var codeElement))
//            {
//                return codeElement;
//            }
//            else if (TryParseElement(view,matchingContext, out var obj))
//            {
//                return obj;
//            }
//            else {
//                throw new Exception($"could not parse {view.Token}");
//            }
//        }


//        public static bool TryParseAtomic(IParseStateView view, object last, ElementMatchingContext matchingContext, out ICodeElement codeElement)
//        {
//            if (view.Token is AtomicToken atomicToken)
//            {

//                if (Operations.StandardOperations(matchingContext.ElementBuilder).BinaryOperations.TryGetValue(atomicToken.Item, out var binaryFunc))
//                {
//                    if (last == null)
//                    {
//                        throw new Exception("last required but not provied");
//                    }
//                    if (view.TryGetNext(out var next))
//                    {
//                        codeElement = binaryFunc(
//                            last,
//                            ParseLineElementOrThrow(next, last, matchingContext));
//                    }
//                }
//                else if (Operations.StandardOperations(matchingContext.ElementBuilder).LastOperations.TryGetValue(atomicToken.Item, out var lastFunc))
//                {

//                    if (last == null)
//                    {
//                        throw new Exception("last required but not provied");
//                    }
//                    codeElement = lastFunc(
//                        last);
//                    return true;

//                }
//                else if (Operations.StandardOperations(matchingContext.ElementBuilder).NextOperations.TryGetValue(atomicToken.Item, out var nextFunc))
//                {
//                    if (view.TryGetNext(out var next))
//                    {
//                        codeElement = nextFunc(
//                            ParseLineElementOrThrow(next, last, matchingContext));

//                        return true;
//                    }
//                    else
//                    {
//                        throw new Exception($"Operation: {atomicToken.Item}, requires next. next is not defined");
//                    }
//                }
//                else if (Operations.StandardOperations(matchingContext.ElementBuilder).ConstantOperations.TryGetValue(atomicToken.Item, out var action))
//                {
//                    codeElement = action();
//                    return true;
//                }
//                else
//                {
//                    throw new Exception($"Operation: {atomicToken.Item}, not known");
//                }
//            }
//            codeElement = default;
//            return false;
//        }
        
        
//    }
//}
