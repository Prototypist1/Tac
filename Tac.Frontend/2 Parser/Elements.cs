using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.SyntaxModel.Elements;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Infastructure;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox.Object;
using Prototypist.Toolbox.Bool;
using Tac.Frontend.Parser;
using System.Diagnostics.CodeAnalysis;

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

        private static Condition<T> MustBeBefore<T>(System.Type type) => list => !list.Where(x => type.IsAssignableFrom(x.GetType())).Any();

        private static Condition<T> MustBeAfter<T>(System.Type type) => list => list.Where(x => type.IsAssignableFrom(x.GetType())).Any();

        private static readonly List<Func<IBox<IIsPossibly<IFrontendType>>, WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>>> implicitElementMakers = new List<Func<IBox<IIsPossibly<IFrontendType>>, WithConditions<ISetUp<IFrontendCodeElement,Tpn.ITypeProblemNode>>>>();
        private static readonly List<WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> operationMatchers = new List<WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> elementMakers = new List<WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>> typeOperationMatchers = new List<WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>> typeMakers = new List<WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>>();
        public IEnumerable<IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> OperationMatchers => Process(operationMatchers);
        public IEnumerable<IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> ElementMakers => Process(elementMakers);
        public IEnumerable<IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>> TypeOperationMatchers => Process(typeOperationMatchers);
        public IEnumerable<IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>> TypeMakers => Process(typeMakers);

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
            WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> res(IBox<IIsPossibly<IFrontendType>> x) => new WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(() => func(x), conditions.ToList());
            implicitElementMakers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> AddOperationMatcher(Func<IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] conditions) {
            var res = new WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(item, conditions.ToList());
            operationMatchers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> AddElementMakers(Func<IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] conditions)
        {
            var res = new WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(item, conditions.ToList());
            elementMakers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>> AddTypeOperationMatcher(Func<IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>[] conditions)
        {
            var res = new WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>(item, conditions.ToList());
            typeOperationMatchers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>> AddTypeMaker(Func<IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>[] conditions)
        {
            var res = new WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>(item, conditions.ToList());
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

        public ElementMatchingContext() : 
            this(
                MakerRegistry.Instance.OperationMatchers.ToArray(),
                MakerRegistry.Instance.ElementMakers.ToArray(),
                MakerRegistry.Instance.TypeOperationMatchers.ToArray(),
                MakerRegistry.Instance.TypeMakers.ToArray()
                )
        {}
        
        public ElementMatchingContext(
            IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] operationMatchers, 
            IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] elementMakers,
            IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>[] typeOperationMatchers,
            IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>[] typeMakers
            )
        {
            this.operationMatchers = operationMatchers ?? throw new ArgumentNullException(nameof(operationMatchers));
            this.elementMakers = elementMakers ?? throw new ArgumentNullException(nameof(elementMakers));
            this.typeOperationMatchers = typeOperationMatchers ?? throw new ArgumentNullException(nameof(typeOperationMatchers));
            this.typeMakers = typeMakers ?? throw new ArgumentNullException(nameof(typeMakers));
        }

        private readonly IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] elementMakers;
        private readonly IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] operationMatchers;
        private readonly IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>[] typeOperationMatchers;
        private readonly IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>[] typeMakers;

        #region Parse

        public ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode> ParseParenthesisOrElementType(IToken token) {
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
                        return res!;
                    }
                }
            }
            else if (token is ParenthesisToken parenthesisToken)
            {
                return ParseTypeLine(parenthesisToken.Tokens);
            }

            throw new Exception("");
        }

        public IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> ParseParenthesisOrElement(IToken token)
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
                    if (TokenMatching<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>.MakeStart(elementToken.Tokens,this)
                        .Has(tryMatch, out var res)
                        .Has(new DoneMaker())
                        is IMatchedTokenMatching)
                    {
                        return OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(res!);
                    }
                }
            }
            else if (token is ParenthesisToken parenthesisToken)
            {
                return ParseLine(parenthesisToken.Tokens);
            }

            return OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(Error.Other($"No element matches {token.ToString()}"));
        }

        public IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError> ParseObjectMember(IToken token)
        {
            if (token is ElementToken elementToken)
            {

                foreach (var tryMatch in  new[] {new ObjectOrTypeMemberDefinitionMaker() })
                {
                    if (TokenMatching<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>>.MakeStart(elementToken.Tokens, this)
                        .Has(tryMatch, out var res)
                        .Has(new DoneMaker())
                        is IMatchedTokenMatching)
                    {
                        return OrType.Make<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>(res!);
                    }
                }
            }


            return OrType.Make<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>(Error.Other($"No element matches {token.ToString()}"));
        }

        public IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> ParseLine(IEnumerable<IToken> tokens)
        {
            foreach (var operationMatcher in operationMatchers)
            {
                if (TokenMatching<ISetUp<ICodeElement, Tpn.ITypeProblemNode>>.MakeStart(tokens.ToArray(), this)
                        .Has(operationMatcher, out var res)
                         is IMatchedTokenMatching)
                {
                    return OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(res!);
                }
            }

            if (tokens.Count() == 1)
            {
                return ParseParenthesisOrElement(tokens.Single());
            }

            return OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(Error.Other($"No operation matches {tokens.Aggregate("",(x,y)=> x +" "+ y.ToString())}"));
        }

        public IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> ParseObjectLine(IEnumerable<IToken> tokens)
        {
            
            foreach (var operationMatcher in new[] { new AssertAssignInObjectOperationMaker() })
            {
                if (TokenMatching<ISetUp<ICodeElement, Tpn.ITypeProblemNode>>.MakeStart(tokens.ToArray(), this)
                        .Has(operationMatcher, out var res)
                         is IMatchedTokenMatching)
                {
                    return OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(res!);
                }
            }

            return OrType.Make<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>(Error.Other($"No operation matches {tokens.Aggregate("", (x, y) => x + " " + y.ToString())}"));
        }

        public IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError> ParseLineInDefinitionType(IEnumerable<IToken> tokens)
        {


            if (tokens.Count() == 1)
            {
                var token = tokens.First();
                if (token is ElementToken elementToken)
                {

                    foreach (var tryMatch in new[] { new ObjectOrTypeMemberDefinitionMaker() })
                    {
                        if (TokenMatching<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>>.MakeStart(elementToken.Tokens, this)
                            .Has(tryMatch, out var res)
                            .Has(new DoneMaker())
                            is IMatchedTokenMatching)
                        {
                            return OrType.Make<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>(res!);
                        }
                    }
                }
                return OrType.Make<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>(Error.Other($"No element matches {token.ToString()}"));
            }
            else {
                throw new Exception("type should not have more than one thing it it");
            }
        }

        public ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode> ParseTypeLine(IEnumerable<IToken> tokens)
        {
            foreach (var operationMatcher in typeOperationMatchers)
            {
                if (TokenMatching<ISetUp<ICodeElement, Tpn.ITypeProblemNode>>.MakeStart(tokens.ToArray(), this)
                        .Has(operationMatcher, out var res)
                         is IMatchedTokenMatching)
                {
                    return res!;
                }
            }

            if (tokens.Count() == 1)
            {
                return ParseParenthesisOrElementType(tokens.Single());
            }

            throw new Exception("");
        }
        
        public IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> ParseFile(FileToken file)
        {
            return file.Tokens.Select(x => ParseLine(x.CastTo<LineToken>().Tokens)).ToArray();
        }

        public IReadOnlyList< IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> ParseBlock(CurleyBracketToken block)
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


        public IReadOnlyList<IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>> ParseType(CurleyBracketToken block)
        {
            return block.Tokens.Select(x =>
            {
                if (x is LineToken lineToken)
                {
                    return ParseLineInDefinitionType(lineToken.Tokens);
                }
                throw new Exception("unexpected token type");
            }).ToArray();
        }


        public IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> ParseObject(CurleyBracketToken block)
        {
            return block.Tokens.Select(x =>
            {
                if (x is LineToken lineToken)
                {
                    return ParseObjectLine(lineToken.Tokens);
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

    internal interface ITokenMatching<out T1, out T2> : ITokenMatching
    {
    }

    internal interface ITokenMatching<out T1, out T2, out T3> : ITokenMatching
    {
    }

    internal interface ITokenMatching<out T1, out T2, out T3, out T4> : ITokenMatching
    {
    }

    internal interface ITokenMatching<out T1, out T2, out T3, out T4, out T5> : ITokenMatching
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

    internal interface IMatchedTokenMatching<out T1, out T2> : ITokenMatching<T1,T2>, IMatchedTokenMatching
    {
        T1 Value1 { get; }
        T2 Value2 { get; }
    }

    internal interface IMatchedTokenMatching<out T1, out T2 ,out T3> : ITokenMatching<T1,T2,T3>, IMatchedTokenMatching
    {
        T1 Value1 { get; }
        T2 Value2 { get; }
        T3 Value3 { get; }
    }

    internal interface IMatchedTokenMatching<out T1, out T2, out T3, out T4> : ITokenMatching<T1, T2, T3, T4>, IMatchedTokenMatching
    {
        T1 Value1 { get; }
        T2 Value2 { get; }
        T3 Value3 { get; }
        T4 Value4 { get; }
    }

    internal interface IMatchedTokenMatching<out T1, out T2, out T3, out T4, out T5> : ITokenMatching<T1, T2, T3, T4, T5>, IMatchedTokenMatching
    {
        T1 Value1 { get; }
        T2 Value2 { get; }
        T3 Value3 { get; }
        T4 Value4 { get; }
        T5 Value5 { get; }
    }


    internal static class TokenMatchingExtensions {
        public static ITokenMatching<T> ConvertIfMatched<T, T1, T2, T3, T4, T5>(this ITokenMatching<T1, T2, T3, T4, T5> tokenMatching, Func<T1,T2,T3,T4,T5, T> convert) {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1,T2,T3,T4,T5> matched)) {
                return TokenMatching < T >.MakeMatch(matched.Tokens,matched.Context, convert(matched.Value1, matched.Value2, matched.Value3, matched.Value4, matched.Value5));
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }

        public static ITokenMatching<T> ConvertIfMatched<T, T1, T2, T3, T4>(this ITokenMatching<T1, T2, T3, T4> tokenMatching, Func<T1, T2, T3, T4, T> convert)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1, T2, T3, T4> matched))
            {
                return TokenMatching<T>.MakeMatch(matched.Tokens, matched.Context, convert(matched.Value1, matched.Value2, matched.Value3, matched.Value4));
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }

        public static ITokenMatching<T> ConvertIfMatched<T, T1, T2, T3>(this ITokenMatching<T1, T2, T3> tokenMatching, Func<T1, T2, T3, T> convert)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1, T2, T3> matched))
            {
                return TokenMatching<T>.MakeMatch(matched.Tokens, matched.Context, convert(matched.Value1, matched.Value2, matched.Value3));
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }


        public static ITokenMatching<T> ConvertIfMatched<T, T1, T2>(this ITokenMatching<T1, T2> tokenMatching, Func<T1, T2, T> convert)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1, T2> matched))
            {
                return TokenMatching<T>.MakeMatch(matched.Tokens, matched.Context, convert(matched.Value1, matched.Value2));
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }


        public static ITokenMatching<T> ConvertIfMatched<T, T1>(this ITokenMatching<T1> tokenMatching, Func<T1,  T> convert)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1> matched))
            {
                return TokenMatching<T>.MakeMatch(matched.Tokens, matched.Context, convert(matched.Value));
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }

        public static ITokenMatching<T> ConvertIfMatched<T>(this ITokenMatching tokenMatching, Func<T> convert)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching matched))
            {
                return TokenMatching<T>.MakeMatch(matched.Tokens, matched.Context, convert());
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }
    }

    internal static class TokenMatching<T>
    {
        private class Start : IMatchedTokenMatching
        {
            public Start(IReadOnlyList<IToken> tokens, ElementMatchingContext context)
            {
                Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public IReadOnlyList<IToken> Tokens
            {
                get;
            }

            public ElementMatchingContext Context
            {
                get;
            }
        }

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
        
        public static IMatchedTokenMatching MakeStart(IReadOnlyList<IToken> tokens, ElementMatchingContext context)
        {
            return new Start(tokens, context);
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
    // but it falls down a bit when you start working with hasSquare, hasLine, hasElement the out vars don't play nice with the method
    // matchOne fails a bit too
    // composing is hard because you are limited to a single return
    
    internal static class TokenMatching<T1,T2> {
        private class Matched : IMatchedTokenMatching<T1,T2>
        {
            public Matched(IReadOnlyList<IToken> tokens, ElementMatchingContext context, T1 value1, T2 value2)
            {
                Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value1 = value1; 
                Value2 = value2;
            }

            public IReadOnlyList<IToken> Tokens
            {
                get;
            }

            public ElementMatchingContext Context
            {
                get;
            }

            public T1 Value1
            {
                get;
            }
            public T2 Value2
            {
                get;
            }
        }

        private class NotMatched : ITokenMatching<T1,T2>
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

        public static IMatchedTokenMatching<T1,T2> MakeMatch(IReadOnlyList<IToken> tokens, ElementMatchingContext context, T1 value1, T2 value2)
        {
            return new Matched(tokens, context, value1, value2);
        }

        public static ITokenMatching<T1,T2> MakeNotMatch(ElementMatchingContext context)
        {
            return new NotMatched(context);
        }
    }

    internal static class TokenMatching<T1, T2,T3>
    {
        private class Matched : IMatchedTokenMatching<T1, T2, T3>
        {
            public Matched(IReadOnlyList<IToken> tokens, ElementMatchingContext context, T1 value1, T2 value2, T3 value3)
            {
                Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
            }

            public IReadOnlyList<IToken> Tokens
            {
                get;
            }

            public ElementMatchingContext Context
            {
                get;
            }

            public T1 Value1
            {
                get;
            }
            public T2 Value2
            {
                get;
            }
            public T3 Value3
            {
                get;
            }
        }

        private class NotMatched : ITokenMatching<T1, T2,T3>
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

        public static IMatchedTokenMatching<T1, T2, T3> MakeMatch(IReadOnlyList<IToken> tokens, ElementMatchingContext context, T1 value1, T2 value2, T3 value3)
        {
            return new Matched(tokens, context, value1, value2, value3);
        }

        public static ITokenMatching<T1, T2,T3> MakeNotMatch(ElementMatchingContext context)
        {
            return new NotMatched(context);
        }
    }

    internal static class TokenMatching<T1, T2, T3, T4>
    {
        private class Matched : IMatchedTokenMatching<T1, T2, T3, T4>
        {
            public Matched(IReadOnlyList<IToken> tokens, ElementMatchingContext context, T1 value1, T2 value2, T3 value3, T4 value4)
            {
                Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
                Value4 = value4;
            }

            public IReadOnlyList<IToken> Tokens
            {
                get;
            }

            public ElementMatchingContext Context
            {
                get;
            }

            public T1 Value1
            {
                get;
            }
            public T2 Value2
            {
                get;
            }
            public T3 Value3
            {
                get;
            }
            public T4 Value4
            {
                get;
            }
        }

        private class NotMatched : ITokenMatching<T1, T2, T3, T4>
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

        public static IMatchedTokenMatching<T1, T2, T3, T4> MakeMatch(
            IReadOnlyList<IToken> tokens, 
            ElementMatchingContext context, 
            T1 value1, 
            T2 value2, 
            T3 value3,
            T4 value4)
        {
            return new Matched(tokens, context, value1, value2, value3, value4);
        }

        public static ITokenMatching<T1, T2, T3,T4> MakeNotMatch(ElementMatchingContext context)
        {
            return new NotMatched(context);
        }
    }

    internal static class TokenMatching<T1, T2, T3, T4, T5>
    {
        private class Matched : IMatchedTokenMatching<T1, T2, T3, T4, T5>
        {
            public Matched(IReadOnlyList<IToken> tokens, ElementMatchingContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
                Value4 = value4;
                Value5 = value5;
            }

            public IReadOnlyList<IToken> Tokens
            {
                get;
            }

            public ElementMatchingContext Context
            {
                get;
            }

            public T1 Value1
            {
                get;
            }
            public T2 Value2
            {
                get;
            }
            public T3 Value3
            {
                get;
            }
            public T4 Value4
            {
                get;
            }
            public T5 Value5
            {
                get;
            }
        }

        private class NotMatched : ITokenMatching<T1, T2, T3, T4, T5>
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

        public static IMatchedTokenMatching<T1, T2, T3, T4, T5> MakeMatch(
            IReadOnlyList<IToken> tokens,
            ElementMatchingContext context,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5)
        {
            return new Matched(tokens, context, value1, value2, value3, value4, value5);
        }

        public static ITokenMatching<T1, T2, T3,T4,T5> MakeNotMatch(ElementMatchingContext context)
        {
            return new NotMatched(context);
        }
    }

    internal static class ElementMatcher
    {
        public static ITokenMatching<T> GetValue<T>(this ITokenMatching<T> self, out T? value)
            where T:class
        {
            if (self is IMatchedTokenMatching<T> matched) {
                value = matched.Value;
                return matched;
            }
            value = default;
            return self;
        }



        public static ITokenMatching Has<T>(this ITokenMatching self, IMaker<T> pattern, out T? t)
            where T:class
        {

            if (! (self is IMatchedTokenMatching firstMatched))
            {
                t = default;
                return TokenMatching<T>.MakeNotMatch(self.Context);
            }

            var res = pattern.TryMake(firstMatched);
            if (res is IMatchedTokenMatching<T> matched)
            {
                t = matched.Value;
                return res;
            }

            t = default;
            return res;
        }

        public static ITokenMatching<T> Has<T>(this ITokenMatching self, IMaker<T> pattern)
            where T : class
        {

            if (!(self is IMatchedTokenMatching firstMatched))
            {
                return TokenMatching<T>.MakeNotMatch(self.Context);
            }

            var res = pattern.TryMake(firstMatched);
            if (res is IMatchedTokenMatching<T> matched)
            {
                return res;
            }

            return res;
        }

        public static ITokenMatching<T1,T2> Has<T1,T2>(this ITokenMatching<T1> self, IMaker<T2> pattern)
            where T1 : class
            where T2 : class
        {

            if (self is IMatchedTokenMatching<T1> firstMatched)
            {

                var res = pattern.TryMake(firstMatched);
                if (res is IMatchedTokenMatching<T2> matched)
                {
                    return TokenMatching<T1, T2>.MakeMatch(matched.Tokens, firstMatched.Context, firstMatched.Value, matched.Value);
                }
            }

            return TokenMatching<T1, T2>.MakeNotMatch(self.Context);
        }

        public static ITokenMatching<T1, T2, T3> Has<T1, T2,T3>(this ITokenMatching<T1,T2> self, IMaker<T3> pattern)
            where T1 : class
            where T2 : class
            where T3 : class
        {

            if (self is IMatchedTokenMatching<T1,T2> firstMatched)
            {

                var res = pattern.TryMake(firstMatched);
                if (res is IMatchedTokenMatching<T3> matched)
                {
                    return TokenMatching<T1, T2,T3>.MakeMatch(matched.Tokens, firstMatched.Context, firstMatched.Value1, firstMatched.Value2, matched.Value);
                }
            }

            return TokenMatching<T1, T2,T3>.MakeNotMatch(self.Context);
        }

        public static ITokenMatching<T1, T2, T3,T4> Has<T1, T2, T3,T4>(this ITokenMatching<T1, T2,T3> self, IMaker<T4> pattern)
            where T1 : class
            where T2 : class
            where T3 : class
        {

            if (self is IMatchedTokenMatching<T1, T2,T3> firstMatched)
            {

                var res = pattern.TryMake(firstMatched);
                if (res is IMatchedTokenMatching<T4> matched)
                {
                    return TokenMatching<T1, T2, T3,T4>.MakeMatch(matched.Tokens, firstMatched.Context, firstMatched.Value1, firstMatched.Value2, firstMatched.Value3, matched.Value);
                }
            }

            return TokenMatching<T1, T2, T3,T4>.MakeNotMatch(self.Context);
        }

        public static ITokenMatching<T1, T2, T3, T4,T5> Has<T1, T2, T3, T4,T5>(this ITokenMatching<T1, T2, T3,T4> self, IMaker<T5> pattern)
            where T1 : class
            where T2 : class
            where T3 : class
        {

            if (self is IMatchedTokenMatching<T1, T2, T3,T4> firstMatched)
            {

                var res = pattern.TryMake(firstMatched);
                if (res is IMatchedTokenMatching<T5> matched)
                {
                    return TokenMatching<T1, T2, T3, T4,T5>.MakeMatch(matched.Tokens, firstMatched.Context, firstMatched.Value1, firstMatched.Value2, firstMatched.Value3, firstMatched.Value4, matched.Value);
                }
            }

            return TokenMatching<T1, T2, T3, T4,T5>.MakeNotMatch(self.Context);
        }

        public static ITokenMatching<T> HasStruct<T>(this ITokenMatching self, IMaker<T> pattern, out T t)
            where T : struct
        {

            if (!(self is IMatchedTokenMatching firstMatched))
            {
                t = default;
                return TokenMatching<T>.MakeNotMatch(self.Context);
            }

            var res = pattern.TryMake(firstMatched);
            if (res is IMatchedTokenMatching<T> matched)
            {
                t = matched.Value;
                return res;
            }

            t = default;
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
            out T? res)
            where T:class
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
            out T? res)
            where T:class 
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


        // should return IPossibly<T>
        public static ITokenMatching OptionalHas<T>(this ITokenMatching self, IMaker<T> pattern, [MaybeNullWhen(false)] out T? t)
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
