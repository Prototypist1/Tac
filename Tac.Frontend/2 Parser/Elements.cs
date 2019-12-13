using Prototypist.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{

    internal partial class MakerRegistry {

        public static MakerRegistry Instance = new MakerRegistry();

        private class WithConditions<T> {
            public readonly Func<IMaker<T>> makerMaker;
            private readonly IReadOnlyList<Condition<T>> conditions;

            public WithConditions(Func<IMaker<T>> maker, IReadOnlyList<Condition<T>> conditions)
            {
                this.makerMaker = maker ?? throw new ArgumentNullException(nameof(maker));
                this.conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
            }

            public bool CanGo(IEnumerable<IMaker<T>> list) => conditions.All(x=>x(list));
        }

        private delegate bool Condition<T>(IEnumerable<IMaker<T>> makers);

        private static Condition<T> MustBeBefore<T>(Type type) => list => !list.Where(x => type.IsAssignableFrom(x.GetType())).Any();

        private static Condition<T> MustBeAfter<T>(Type type) => list => list.Where(x => type.IsAssignableFrom(x.GetType())).Any();

        private static readonly List<Func<IBox<IIsPossibly<IFrontendType>>, WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>>> implicitElementMakers = new List<Func<IBox<IIsPossibly<IFrontendType>>, WithConditions<ISetUp<IFrontendCodeElement,Tpn.ITypeProblemNode>>>>();
        private static readonly List<WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> operationMatchers = new List<WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> elementMakers = new List<WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>> typeOperationMatchers = new List<WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>> typeMakers = new List<WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>>();
        public IEnumerable<IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> OperationMatchers => Process(operationMatchers);
        //public IEnumerable<IMaker<IPopulateScope<IFrontendCodeElement>>> ImplicitOperationMatchers(IBox<IIsPossibly<IFrontendType>> box) {
        //    var list = new List<WithConditions<IPopulateScope<IFrontendCodeElement>>>();
        //    list.AddRange(elementMakers);
        //    list.AddRange(implicitElementMakers.Select(x => x(box)));

        //    return Process(list);
        //}

        public IEnumerable<IMaker<ISetUp<IFrontendCodeElement,Tpn.ITypeProblemNode>>> ElementMakers => Process(elementMakers);
        public IEnumerable<IMaker<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>> TypeOperationMatchers => Process(typeOperationMatchers);
        public IEnumerable<IMaker<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>> TypeMakers => Process(typeMakers);

        private IEnumerable<IMaker<T>> Process<T>(List<WithConditions<T>> withConditionss) {
            var lastCount = -1;
            var res = new List<IMaker<T>>();
            while (withConditionss.Count != lastCount) {
                lastCount = withConditionss.Count;

                var nextWithConditionss = new List<WithConditions<T>>();

                foreach (var withConditions in withConditionss)
                {
                    if (withConditions.CanGo(res))
                    {
                        var testList = new List<IMaker<T>>();
                        var item = withConditions.makerMaker();
                        testList.AddRange(res);
                        testList.Add(item);
                        if (withConditionss.Except(new[] { withConditions }).All(x => x.CanGo(testList)))
                        {
                            res = testList;
                        }
                        else {
                            nextWithConditionss.Add(withConditions);
                        }
                    }
                    else {
                        nextWithConditionss.Add(withConditions);
                    }
                }

                if (!nextWithConditionss.Any()) {
                    return res;
                }

                withConditionss = nextWithConditionss;
            }
            throw new Exception("could not order");
        }

        private static Func<IBox<IIsPossibly<IFrontendType>>, WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> AddImplicitOperationMatcher(
            Func<IBox<IIsPossibly<IFrontendType>>, IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> func, 
            params Condition<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>[] conditions)
        {
            Func<IBox<IIsPossibly<IFrontendType>>, WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> res = (x) => new WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(() => func(x), conditions.ToList());
            implicitElementMakers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> AddOperationMatcher(Func<IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>[] conditions) {
            var res = new WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(item, conditions.ToList());
            operationMatchers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> AddElementMakers(Func<IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>[] conditions)
        {
            var res = new WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(item, conditions.ToList());
            elementMakers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>> AddTypeOperationMatcher(Func<IMaker<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>[] conditions)
        {
            var res = new WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>(item, conditions.ToList());
            typeOperationMatchers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>> AddTypeMaker(Func<IMaker<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>[] conditions)
        {
            var res = new WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>(item, conditions.ToList());
            typeMakers.Add(res);
            return res;
        }
    }

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

        //internal ElementMatchingContext ExpectPathPart(IBox<IIsPossibly<IFrontendType>> box) {
        //    return new ElementMatchingContext(
        //        operationMatchers, 
        //        new IMaker<IPopulateScope<IFrontendCodeElement>>[] {
        //            new MemberReferanceMaker(box)
        //        },
        //        typeOperationMatchers,
        //        typeMakers);
        //}
        
        //internal ElementMatchingContext AcceptImplicit(IBox<IIsPossibly<IFrontendType>> box)
        //{
        //    return new ElementMatchingContext(
        //        operationMatchers,
        //        MakerRegistry.Instance.ImplicitOperationMatchers(box).ToArray(),
        //        typeOperationMatchers, 
        //        typeMakers);
        //}
        
        //internal ElementMatchingContext Child(ScopeStack scope)
        //{
        //    return new ElementMatchingContext(Builders,operationMatchers, elementMakers, scope);
        //}
        
        public ElementMatchingContext() : 
            this(
                MakerRegistry.Instance.OperationMatchers.ToArray(),
                MakerRegistry.Instance.ElementMakers.ToArray(),
                MakerRegistry.Instance.TypeOperationMatchers.ToArray(),
                MakerRegistry.Instance.TypeMakers.ToArray()
                )
        {}
        
        public ElementMatchingContext(
            IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>[] operationMatchers, 
            IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>[] elementMakers,
            IMaker<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>[] typeOperationMatchers,
            IMaker<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>[] typeMakers
            )
        {
            this.operationMatchers = operationMatchers ?? throw new ArgumentNullException(nameof(operationMatchers));
            this.elementMakers = elementMakers ?? throw new ArgumentNullException(nameof(elementMakers));
            this.typeOperationMatchers = typeOperationMatchers ?? throw new ArgumentNullException(nameof(typeOperationMatchers));
            this.typeMakers = typeMakers ?? throw new ArgumentNullException(nameof(typeMakers));
        }

        private readonly IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>[] elementMakers;
        private readonly IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>[] operationMatchers;
        private readonly IMaker<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>[] typeOperationMatchers;
        private readonly IMaker<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>[] typeMakers;

        #region Parse

        public ISetUp<IFrontendType, Tpn.ITypeProblemNode> ParseParenthesisOrElementType(IToken token) {
            if (token is ElementToken elementToken)
            {
                // smells
                // why did i write this agian?
                // why would an element be wrapped in parenthesis ?
                // maybe I can just remove??
                // maybe we have a parentthesis matcher?
                if (elementToken.Tokens.Count == 1 && elementToken.Tokens[0] is ParenthesisToken parenthesisToken)
                {
                    return ParseTypeLine(parenthesisToken.Tokens);
                }

                foreach (var tryMatch in typeMakers)
                {
                    if (TokenMatching<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>.MakeStart(elementToken.Tokens, this)
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
                return ParseTypeLine(parenthesisToken.Tokens);
            }

            throw new Exception("");
        }

        public ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> ParseParenthesisOrElement(IToken token)
        {
            if (token is ElementToken elementToken)
            {
                // smells
                // why did i write this agian?
                // why would an element be wrapped in parenthesis ?
                // maybe I can just remove??
                // maybe we have a parentthesis matcher?
                if (elementToken.Tokens.Count == 1 && elementToken.Tokens[0] is ParenthesisToken parenthesisToken)
                {
                    return ParseLine(parenthesisToken.Tokens);
                }

                foreach (var tryMatch in elementMakers)
                {
                    if (TokenMatching<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>.MakeStart(elementToken.Tokens,this)
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

        public ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> ParseLine(IEnumerable<IToken> tokens)
        {
            foreach (var operationMatcher in operationMatchers)
            {
                if (TokenMatching<ISetUp<ICodeElement, Tpn.ITypeProblemNode>>.MakeStart(tokens.ToArray(), this)
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

        public ISetUp<IFrontendType, Tpn.ITypeProblemNode> ParseTypeLine(IEnumerable<IToken> tokens)
        {
            foreach (var operationMatcher in typeOperationMatchers)
            {
                if (TokenMatching<ISetUp<ICodeElement, Tpn.ITypeProblemNode>>.MakeStart(tokens.ToArray(), this)
                        .Has(operationMatcher, out var res)
                         is IMatchedTokenMatching)
                {
                    return res;
                }
            }

            if (tokens.Count() == 1)
            {
                return ParseParenthesisOrElementType(tokens.Single());
            }

            throw new Exception("");
        }

        public ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] ParseFile(FileToken file)
        {
            return file.Tokens.Select(x => ParseLine(x.Cast<LineToken>().Tokens)).ToArray();
        }

        public ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] ParseBlock(CurleyBracketToken block)
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

            if (matchedTokenMatching.Tokens[0] is SquareBacketToken squareBacketToken)
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
            if (!(self is IMatchedTokenMatching))
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

        public static ITokenMatching<T> HasOne<T>(
            this ITokenMatching self,
            Func<ITokenMatching, ITokenMatching<T>>[] items,
            out T res)
        {
            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                res = default;
                return TokenMatching<T>.MakeNotMatch(self.Context);
            }

            var results = items.Select(x => x(self)).ToArray();

            var goodResults = results.OfType<IMatchedTokenMatching<T>>().ToArray();

            if (goodResults.Length > 1)
            {
                throw new Exception("more than one should not match!");
            }

            if (goodResults.Length == 1)
            {
                res = goodResults.First().Value;
                return goodResults.First();
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

            if (matchedTokenMatching.Tokens[0] is LineToken line)
            {
                if (inner(TokenMatching<object>.MakeStart(line.Tokens, self.Context)) is IMatchedTokenMatching)
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

            if (matchedTokenMatching.Tokens[0] is ElementToken elementToken)
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
            if (!(self is IMatchedTokenMatching))
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
            if (!(self is IMatchedTokenMatching))
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
