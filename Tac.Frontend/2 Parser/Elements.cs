using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class Operation<T>
        where T: Delegate
    {
        public readonly T make;
        public readonly string idenifier;

        public Operation(T make, string idenifier)
        {
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.idenifier = idenifier ?? throw new ArgumentNullException(nameof(idenifier));
        }
    }

    internal class ElementMatchingContext
    {

        internal ElementMatchingContext ExpectPathPart(IBox<IIsPossibly<IFrontendType<IVerifiableType>>> box) {
            return new ElementMatchingContext(operationMatchers, new IMaker<IPopulateScope<IFrontendCodeElement<ICodeElement>>>[] {
                new MemberReferanceMaker(box)
            });
        }
        
        internal ElementMatchingContext AcceptImplicit(IBox<IIsPossibly<IFrontendType<IVerifiableType>>> box)
        {
            return new ElementMatchingContext(operationMatchers, new IMaker<IPopulateScope<IFrontendCodeElement<ICodeElement>>>[] {
                new BlockDefinitionMaker(),
                new ConstantNumberMaker(),
                new GenericTypeDefinitionMaker(),
                new ImplementationDefinitionMaker(),
                new MemberDefinitionMaker(),
                new MethodDefinitionMaker(),
                new ModuleDefinitionMaker(),
                new ObjectDefinitionMaker(),
                new EmptyInstanceMaker(),
                new ConstantBoolMaker(),
                new ConstantStringMaker(),
                new TypeDefinitionMaker(),
                new ImplicitMemberMaker(box),
                new MemberMaker(),
            });
        }
        
        //internal ElementMatchingContext Child(ScopeStack scope)
        //{
        //    return new ElementMatchingContext(Builders,operationMatchers, elementMakers, scope);
        //}
        
        public ElementMatchingContext() : 
            this(
                new IMaker<IPopulateScope<IFrontendCodeElement<ICodeElement>>>[] {
                    new AddOperationMaker(),
                    new SubtractOperationMaker(),
                    new MultiplyOperationMaker(),
                    new IfTrueOperationMaker(),
                    new ElseOperationMaker(),
                    new LessThanOperationMaker(),
                    new NextCallOperationMaker(),
                    new AssignOperationMaker(),
                    new PathOperationMaker(),
                    new ReturnOperationMaker()
                },
                new IMaker<IPopulateScope<IFrontendCodeElement<ICodeElement>>>[] {
                    new BlockDefinitionMaker(),
                    new ConstantNumberMaker(),
                    new GenericTypeDefinitionMaker(),
                    new ImplementationDefinitionMaker(),
                    new MemberDefinitionMaker(),
                    new MethodDefinitionMaker(),
                    new ModuleDefinitionMaker(),
                    new ObjectDefinitionMaker(),
                    new TypeDefinitionMaker(),
                    new GenericTypeDefinitionMaker(),
                    new EmptyInstanceMaker(),
                    new ConstantBoolMaker(),
                    new ConstantStringMaker(),
                    new MemberMaker(),
                }){}
        
        public ElementMatchingContext(
            IMaker<IPopulateScope<IFrontendCodeElement<ICodeElement>>>[] operationMatchers, 
            IMaker<IPopulateScope<IFrontendCodeElement<ICodeElement>>>[] elementMakers)
        {
            this.operationMatchers = operationMatchers ?? throw new ArgumentNullException(nameof(operationMatchers));
            this.elementMakers = elementMakers ?? throw new ArgumentNullException(nameof(elementMakers));
        }

        private readonly IMaker<IPopulateScope<IFrontendCodeElement<ICodeElement>>>[] elementMakers;
        private readonly IMaker<IPopulateScope<IFrontendCodeElement<ICodeElement>>>[] operationMatchers;
        
        #region Parse

        public IPopulateScope<IFrontendCodeElement<ICodeElement>> ParseParenthesisOrElement(IToken token)
        {
            if (token is ElementToken elementToken)
            {
                // smells
                // why did i write this agian?
                // why would an element be wrapped in parenthesis ?
                // maybe I can just remove??
                // maybe we have a parentthesis matcher?
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken)
                {
                    return ParseLine(parenthesisToken.Tokens);
                }

                foreach (var tryMatch in elementMakers)
                {
                    if (TokenMatching<IPopulateScope<IFrontendCodeElement<ICodeElement>>>.MakeStart(elementToken.Tokens,this)
                        .Has(tryMatch, out var res)
                        .Has(new DoneMaker())
                        is IMatchedTokenMatching)
                    {
                        return res;
                    }
                }
            }
            else if (token is ParenthesisToken parenthesisToken)
            {
                return ParseLine(parenthesisToken.Tokens);
            }

            throw new Exception("");
        }

        public IPopulateScope<IFrontendCodeElement<ICodeElement>> ParseLine(IEnumerable<IToken> tokens)
        {
            foreach (var operationMatcher in operationMatchers)
            {
                if (TokenMatching<IPopulateScope<ICodeElement>>.MakeStart(tokens.ToArray(), this)
                        .Has(operationMatcher, out var res)
                         is IMatchedTokenMatching)
                {
                    return res;
                }
            }

            if (tokens.Count() == 1)
            {
                return ParseParenthesisOrElement(tokens.Single());
            }

            throw new Exception("");
        }

        public IPopulateScope<IFrontendCodeElement<ICodeElement>>[] ParseFile(FileToken file)
        {
            return file.Tokens.Select(x => ParseLine(x.Cast<LineToken>().Tokens)).ToArray();
        }

        public IPopulateScope<IFrontendCodeElement<ICodeElement>>[] ParseBlock(CurleyBracketToken block)
        {
            return block.Tokens.Select(x =>
            {
                if (x is LineToken lineToken)
                {
                    return ParseLine(lineToken.Tokens);
                }
                throw new Exception("unexpected token type");
            }).ToArray();
        }
        
        #endregion

    }

    // TODO well this is a mess

    internal interface ITokenMatching
    {
        ElementMatchingContext Context { get; }
    }

    internal interface ITokenMatching<out T>: ITokenMatching
    {
    }

    internal interface IMatchedTokenMatching: ITokenMatching
    {
        IReadOnlyList<IToken> Tokens { get; }
    }

    internal interface IMatchedTokenMatching<out T> : ITokenMatching<T>, IMatchedTokenMatching
    {
        T Value { get; }
    }
    
    internal static class TokenMatching<T> 
    {

        private class Matched : IMatchedTokenMatching<T>
        {
            public Matched(IReadOnlyList<IToken> tokens, ElementMatchingContext context, T value)
            {
                Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value = value;
            }

            public IReadOnlyList<IToken> Tokens
            {
                get;
            }

            public ElementMatchingContext Context
            {
                get;
            }

            public T Value
            {
                get;
            }
        }

        private class NotMatched : ITokenMatching<T>
        {
            public NotMatched(ElementMatchingContext context)
            {
                Context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public ElementMatchingContext Context
            {
                get;
            }
        }
        
        public static IMatchedTokenMatching<T> MakeStart(IReadOnlyList<IToken> tokens, ElementMatchingContext context)
        {
            return new Matched(tokens, context, default);
        }

        public static IMatchedTokenMatching<T> MakeMatch(IReadOnlyList<IToken> tokens, ElementMatchingContext context, T value)
        {
            return new Matched(tokens, context,value);
        }

        // TODO this should not take tokens 
        // and we should protect the tokens from being accessed on non-matched entries
        // I want to encode tokens and matchedness in the type
        // this is going to be a few types and interfaces with this static class that creates the real private inner classes
        public static ITokenMatching<T> MakeNotMatch(ElementMatchingContext context)
        {
            return new NotMatched(context);
        }

    }

    // this is a good api
    // but it falls down a bit when you start working wiht hasSquare, hasLine, hasElement the out vars don't play nice with the method
    // matchOne fails a bit too
    // composing is hard because you are limited to a single return
    
    internal static class ElementMatcher
    {
        public static ITokenMatching<T> GetValue<T>(this ITokenMatching<T> self, out T value) {
            if (self is IMatchedTokenMatching<T> matched) {
                value = matched.Value;
                return matched;
            }
            value = default;
            return self;
        }

        public static ITokenMatching<T> Has<T>(this ITokenMatching self, IMaker<T> pattern, out T t)
        {
            t = default;

            if (! (self is IMatchedTokenMatching firstMatched))
            {
                return TokenMatching<T>.MakeNotMatch(self.Context);
            }

            var res = pattern.TryMake(firstMatched);
            if (res is IMatchedTokenMatching<T> matched)
            {
                t = matched.Value;
            }
            return res;
        }



        public static ITokenMatching HasSquare(this ITokenMatching self, Func<IMatchedTokenMatching, ITokenMatching> inner)
        {
            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                return self;
            }

            if (matchedTokenMatching.Tokens.Any().Not())
            {
                return TokenMatching<object>.MakeNotMatch(self.Context);
            }

            if (matchedTokenMatching.Tokens.First() is SquareBacketToken squareBacketToken)
            {
                if (inner(TokenMatching<object>.MakeStart(squareBacketToken.Tokens, self.Context)) is IMatchedTokenMatching) {
                    return TokenMatching<object>.MakeStart(matchedTokenMatching.Tokens.Skip(1).ToArray(), self.Context);
                };
                return TokenMatching<object>.MakeNotMatch(self.Context);
            }

            return TokenMatching<object>.MakeNotMatch(self.Context);
        }
        
        public static ITokenMatching<T> HasOne<T>(
            this ITokenMatching self, 
            Func<ITokenMatching, ITokenMatching<T>> first, 
            Func<ITokenMatching, ITokenMatching<T>> second,
            out T res)
        {
            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                res = default;
                return TokenMatching<T>.MakeNotMatch(self.Context);
            }

            var firstResult = first(self);
            var secondResult = second(self);

            if (firstResult is IMatchedTokenMatching<T> && secondResult is IMatchedTokenMatching<T>) {
                throw new Exception("should not match both!");
            }

            if (firstResult is IMatchedTokenMatching<T> firstMatched) {
                res = firstMatched.Value;
                return firstResult;
            }

            if (secondResult is IMatchedTokenMatching<T> secondMatched)
            {
                res = secondMatched.Value;
                return secondResult;
            }

            res = default;
            return TokenMatching<T>.MakeNotMatch(self.Context);
        }


        public static ITokenMatching HasLine(this ITokenMatching self, Func<IMatchedTokenMatching, ITokenMatching> inner)
        {
            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                return self;
            }

            if (matchedTokenMatching.Tokens.Any().Not()) {
                return TokenMatching<object>.MakeNotMatch(self.Context);
            }

            if (matchedTokenMatching.Tokens.First() is LineToken line)
            {
                if (inner(TokenMatching<object>.MakeStart(line.Tokens, self.Context)) is IMatchedTokenMatching matched)
                {
                    return TokenMatching<object>.MakeStart(matchedTokenMatching.Tokens.Skip(1).ToArray(), self.Context);
                };
                return TokenMatching<object>.MakeNotMatch(self.Context);
            }

            return TokenMatching<object>.MakeNotMatch(self.Context);
        }

        public static ITokenMatching HasElement(this ITokenMatching self, Func<IMatchedTokenMatching, ITokenMatching> inner)
        {
            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                return self;
            }

            if (matchedTokenMatching.Tokens.Any().Not())
            {
                return TokenMatching<object>.MakeNotMatch(self.Context);
            }

            if (matchedTokenMatching.Tokens.First() is ElementToken elementToken)
            {
                if (inner(TokenMatching<object>.MakeStart(elementToken.Tokens, self.Context)) is IMatchedTokenMatching matched)
                {
                    return TokenMatching<object>.MakeStart(matched.Tokens.Skip(1).ToArray(), self.Context);
                };
                return TokenMatching<object>.MakeNotMatch(self.Context);
            }

            return TokenMatching<object>.MakeNotMatch(self.Context);
        }


        public static ITokenMatching<T> Has<T>(this ITokenMatching<T> self, IMaker pattern)
        {
            if (!(self is IMatchedTokenMatching<T> matchedTokenMatching))
            {
                return self;
            }

            var patternMatch = pattern.TryMake(self);

            if (!(patternMatch is IMatchedTokenMatching matchedPattern)) {
                return TokenMatching<T>.MakeNotMatch(patternMatch.Context);
            }

            return TokenMatching<T>.MakeMatch(matchedPattern.Tokens, matchedPattern.Context, matchedTokenMatching.Value);
        }

        public static ITokenMatching Has(this ITokenMatching self, IMaker pattern)
        {
            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                return self;
            }

            return pattern.TryMake(self);
        }


        public static ITokenMatching OptionalHas<T>(this ITokenMatching self, IMaker<T> pattern, out T t)
            where T : class
        {

            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                t = default;
                return self;
            }

            var res = pattern.TryMake(matchedTokenMatching);
            if (res is IMatchedTokenMatching<T> matched)
            {
                t = matched.Value;
                return res;
            }

            t = default;
            return self;
        }

        public static ITokenMatching OptionalHas(this ITokenMatching self, IMaker pattern)
        {
            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                return self;
            }

            var next = pattern.TryMake(self);
            if (next is IMatchedTokenMatching)
            {
                return next;
            }

            return self;
        } 
    }
}
