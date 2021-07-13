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
using System.Globalization;
using Tac.Frontend._3_Syntax_Model.Elements;

namespace Tac.Parser
{


    internal partial class MakerRegistry { // the real one

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

        private static readonly List<Func<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>, WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>>> implicitElementMakers = new List<Func<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>, WithConditions<ISetUp<IFrontendCodeElement,Tpn.ITypeProblemNode>>>>();
        private static readonly List<WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> operationMatchers = new List<WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> elementMakers = new List<WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>> typeOperationMatchers = new List<WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>> , Tpn.ITypeProblemNode>>>();
        private static readonly List<WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>> typeMakers = new List<WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>> , Tpn.ITypeProblemNode>>>();
        public IEnumerable<IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> OperationMatchers => Process(operationMatchers);
        public IEnumerable<IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>> ElementMakers => Process(elementMakers);
        public IEnumerable<IMaker<ISetUp<IBox<IFrontendType<IVerifiableType>>,  Tpn.ITypeProblemNode>>> TypeOperationMatchers => Process(typeOperationMatchers);
        public IEnumerable<IMaker<ISetUp<IBox<IFrontendType<IVerifiableType>>,  Tpn.ITypeProblemNode>>> TypeMakers => Process(typeMakers);

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

        private static Func<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>, WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> AddImplicitOperationMatcher(
            Func<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>, IMaker<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>> func, 
            params Condition<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>[] conditions)
        {
            WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> res(IBox<IIsPossibly<IFrontendType<IVerifiableType>>> x) => new WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(() => func(x), conditions.ToList());
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
        private static WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>> AddTypeOperationMatcher(Func<IMaker<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>[] conditions)
        {
            var res = new WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>(item, conditions.ToList());
            typeOperationMatchers.Add(res);
            return res;
        }
        private static WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>> AddTypeMaker(Func<IMaker<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>> item, params Condition<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>[] conditions)
        {
            var res = new WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>(item, conditions.ToList());
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

        public ElementMatchingContext() 
        {}


        private readonly IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] allMakers = new IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] { 
            
            // constants
            new ConstantBoolMaker(),
            new ConstantNumberMaker(),
            new ConstantStringMaker(),
            new EmptyInstanceMaker(),

            // types
            new TypeDefinitionMaker(),
            new TypeOrOperationMaker(),
            // new TypeReferanceMaker(), // this is so general.. going to need context to know if we are a type or a 
            // new TypeMaker(), // maybe I just need this?
            // new TypeOrOperationMaker(),

            // method likes
            new EntryPointDefinitionMaker(),
            new GenericTypeDefinitionMaker(),
            new MethodDefinitionMaker(),
            new ImplementationDefinitionMaker(),

            // object likes
            new ObjectDefinitionMaker(),

            new BlockDefinitionMaker(),

            new MemberDefinitionMakerAlreadyMatched(),
            new MemberDefinitionMaker(),
            //new ObjectOrTypeMemberDefinitionMaker(), // this only eixts inside types, also objects... I think objects need a specail matcher
            new MemberMaker(), // this matches very broadly so it pretty much goes last

            new ParenthesisOperationMaker(),

            new PathOperationMaker(),

            new MultiplyOperationMaker(),
            new AddOperationMaker(),
            new SubtractOperationMaker(),

            new LessThanOperationMaker(),

            new NextCallOperationMaker(),
            new LastCallOperationMaker(),

            new IfTrueOperationMaker(),
            new ElseOperationMaker(),

            new AssertAssignOperationMaker(),
            //new AssertAssignInObjectOperationMaker(),
            
            new TryAssignOperationMaker(),

            new ReturnOperationMaker(),



        };




        private class ParenthesisOperationMaker : IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>
        {
            public ITokenMatching<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> TryMake(IMatchedTokenMatching elementToken)
            {
                if (elementToken.AllTokens[elementToken.EndIndex].Is1(out var token) && token.SafeIs(out ParenthesisToken parenthesisToken))
                {
                    var res = elementToken.Context.ParseParenthesis(parenthesisToken);
                    if (res.Is1(out var setUp))
                    {
                        return TokenMatching<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>.MakeMatch(elementToken,setUp, elementToken.EndIndex+1);
                    }
                }
                return TokenMatching<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>.MakeNotMatch(elementToken.Context);
            }
        }

        // {48146F3A-6D75-4F24-B857-BED24CE846EA}
        // TODO
        // I think I need special parsing for inside ojects
        // here is why
        // 1 =: x;
        // object {x =: x; 2 =: y;}
        // in object the LHS x is resolves up 
        // the RHS x resolves to create a new member
        // looks like x := ...
        // maybe just special init symbol :
        // 

        private readonly IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] typeLineMakers = new IMaker<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>[] {

            new MemberDefinitionMaker(),
            new MemberMaker(),
        };

        #region Parse

        public IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> ParseLine(IReadOnlyList<IToken> tokens)
        {
            return InnerParseLine(tokens,allMakers);
        }


        private IOrType<ISetUp<IBox<T>, Tpn.ITypeProblemNode>, IError> InnerParseLine<T>(IReadOnlyList<IToken> tokens, IMaker<ISetUp<IBox<T>, Tpn.ITypeProblemNode>>[] makers)
        {

            var myList = tokens.Select(x => OrType.Make<IToken, ISetUp<IBox<T>, Tpn.ITypeProblemNode>>(x)).ToList();

            foreach (var maker in makers)
            {
                // TODO
                // {796AF658-9160-41E2-AD6D-A6D5B905482F}
                // some makers actually want to go right to left 
                // x :=  aggregate < filter < transform < filter < somelist
                // x := y := z := 1
                // ++ ++ ++ ++ a
                // a of b of c (as opposed to c.b.a)

                top:
                for (int i = 0; i < myList.Count; i++)
                {
                    var matching = TokenMatching<ISetUp<ICodeElement, Tpn.ITypeProblemNode>>.MakeStart(myList, this, i);

                    if (matching.Has(maker, out var element).SafeIs(out IMatchedTokenMatching<ISetUp<IBox<T>, Tpn.ITypeProblemNode>> matched))
                    {
                        myList.RemoveRange(matched.StartIndex, matched.EndIndex - matched.StartIndex);
                        myList.Insert(matched.StartIndex, OrType.Make<IToken, ISetUp<IBox<T>, Tpn.ITypeProblemNode>>(matched.Value));
                        goto top;
                    }
                }
            }

            if (myList.Count > 1) {
                return OrType.Make<ISetUp<IBox<T>, Tpn.ITypeProblemNode>, IError>(Error.Other("expected to parse down to one element"));
            }

            var single = myList.Single();

            if (single.Is2(out var setup))
            {
                return OrType.Make<ISetUp<IBox<T>, Tpn.ITypeProblemNode>, IError>(setup);
            }

            return OrType.Make<ISetUp<IBox<T>, Tpn.ITypeProblemNode>, IError>(Error.Other("token could not be matched"));
        }

        public IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError> ParseTypeLine(IReadOnlyList<IToken> tokens)
        {
            return InnerParseLine(tokens, typeLineMakers).TransformAndFlatten(x=> {
                if (x.SafeIs(out ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode> res))
                {
                    return OrType.Make<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>,IError>( res);
                }
                else { 
                    return OrType.Make<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>(Error.Other("member reference expected"));
                }
            });
        }

        private IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> ParseParenthesis(ParenthesisToken parenthesisToken)
        {
            // TODO.. why have a signle line in a parenthesis?
            // {FA9BEE9F-446E-488D-B591-C7634A989D03}
            var item = parenthesisToken.Tokens.Single();
            
            return ParseLine(item.SafeCastTo(out LineToken lt).Tokens);
        }

        public RootScopePopulateScope ParseFile(FileToken file)
        {
            IOrType<EntryPointDefinitionPopulateScope, IError> defaultEntryPoint = OrType.Make<EntryPointDefinitionPopulateScope, IError>(
                new EntryPointDefinitionPopulateScope(
                    new TypeReferancePopulateScope(new NameKey("empty")), 
                    Array.Empty<IOrType<EntryPointDefinitionPopulateScope, IError >>(), 
                    new TypeReferancePopulateScope(new NameKey("empty")), 
                    "unused"));
            var entryToReturn = defaultEntryPoint;

            var assignments = new List<IOrType<WeakAssignOperationPopulateScope, IError>>();
            var types = new List<IOrType<TypeDefinitionPopulateScope, IError>>();
            var genericTypes = new List<IOrType< GenericTypeDefinitionPopulateScope, IError>>();

            foreach (var element in file.Tokens.Select(line => ParseLine(line.CastTo<LineToken>().Tokens)))
            {
                element.Switch(setup =>
                {
                    if (setup.SafeIs(out WeakAssignOperationPopulateScope assign))
                    {
                        assignments.Add(OrType.Make<WeakAssignOperationPopulateScope, IError>(assign));
                    }
                    else if (setup.SafeIs(out GenericTypeDefinitionPopulateScope genericType))
                    {
                        genericTypes.Add(OrType.Make<GenericTypeDefinitionPopulateScope, IError>(genericType));
                    }
                    else if(setup.SafeIs(out TypeDefinitionPopulateScope type))
                    {
                        types.Add(OrType.Make<TypeDefinitionPopulateScope, IError>(type));
                    }
                    else if (setup.SafeIs(out EntryPointDefinitionPopulateScope entry))
                    {
                        if (entryToReturn == defaultEntryPoint)
                        {
                            entryToReturn = OrType.Make<EntryPointDefinitionPopulateScope, IError>(entry);
                        }
                        else
                        {
                            entryToReturn = OrType.Make<EntryPointDefinitionPopulateScope, IError>(Error.Other("you can't have more than one entry point"));
                        }
                    }
                    else
                    {
                        assignments.Add(OrType.Make<WeakAssignOperationPopulateScope, IError>(Error.Other($"unexpected type {setup.GetType().Name}")));
                    }
                }
                , error =>
                {
                    assignments.Add(OrType.Make<WeakAssignOperationPopulateScope, IError>(error));
                });
            }


            return new RootScopePopulateScope(assignments.ToArray() ,entryToReturn, types,genericTypes);
        }

        public IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>[] ParseBlock(CurleyBracketToken block)
        {
            var res = new List<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>>();

            foreach (var line in block.Tokens)
            {
                res.Add(ParseLine(line.CastTo<LineToken>().Tokens));
            }

            return res.ToArray();

        }


        public IReadOnlyList<IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>> ParseType(CurleyBracketToken block)
        {
            var list = new List<IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>>();

            foreach (var line in block.Tokens)
            {
                list.Add(ParseTypeLine(line.CastTo<LineToken>().Tokens));
            }



            return list;
        }

        // it might be worth paring down what can happen in an object

        //public IReadOnlyList<IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>> ParseObject(CurleyBracketToken block)
        //{
        //    foreach (var line in block.Tokens)
        //    {
        //        ParseLine(line.CastTo<LineToken>().Tokens);
        //    }

        //    return block.Tokens.Select(x => Map.GetGreatestParent(x.CastTo<LineToken>().Tokens.First())).ToArray();
        //}

        //public IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,  IError>> ParseObject(CurleyBracketToken block)
        //{
        //    return block.Tokens.Select(x =>
        //    {
        //        if (x is LineToken lineToken)
        //        {
        //            return ParseObjectLine(lineToken.Tokens);
        //        }
        //        throw new Exception("unexpected token type");
        //    }).ToArray();
        //}

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
        IReadOnlyList<IOrType< IToken, ISetUp >> AllTokens { get; }

        /// if you match a single element
        /// EndIndex - StartIndex  will be 1
        int StartIndex { get; }
        /// if you match a single element
        /// EndIndex - StartIndex  will be 1
        int EndIndex { get; }
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
        public static ITokenMatching<T> ConvertIfMatched<T, T1, T2, T3, T4, T5>(this ITokenMatching<T1, T2, T3, T4, T5> tokenMatching, Func<T1,T2,T3,T4,T5, T> convert, IMatchedTokenMatching source) {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1,T2,T3,T4,T5> matched)) {
                return TokenMatching<T>.MakeMatch(source, convert(matched.Value1, matched.Value2, matched.Value3, matched.Value4, matched.Value5), matched.EndIndex);
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }

        public static ITokenMatching<T> ConvertIfMatched<T, T1, T2, T3, T4>(this ITokenMatching<T1, T2, T3, T4> tokenMatching, Func<T1, T2, T3, T4, T> convert, IMatchedTokenMatching source)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1, T2, T3, T4> matched))
            {
                return TokenMatching<T>.MakeMatch(source, convert(matched.Value1, matched.Value2, matched.Value3, matched.Value4),  matched.EndIndex);
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }

        public static ITokenMatching<T> ConvertIfMatched<T, T1, T2, T3>(this ITokenMatching<T1, T2, T3> tokenMatching, Func<T1, T2, T3, T> convert, IMatchedTokenMatching source)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1, T2, T3> matched))
            {
                return TokenMatching<T>.MakeMatch(source, convert(matched.Value1, matched.Value2, matched.Value3),  matched.EndIndex);
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }


        public static ITokenMatching<T> ConvertIfMatched<T, T1, T2>(this ITokenMatching<T1, T2> tokenMatching, Func<T1, T2, T> convert, IMatchedTokenMatching source)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1, T2> matched))
            {
                return TokenMatching<T>.MakeMatch(source, convert(matched.Value1, matched.Value2),  matched.EndIndex);
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }


        public static ITokenMatching<T> ConvertIfMatched<T, T1>(this ITokenMatching<T1> tokenMatching, Func<T1,  T> convert, IMatchedTokenMatching source)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching<T1> matched))
            {
                return TokenMatching<T>.MakeMatch(source, convert(matched.Value), matched.EndIndex);
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }

        public static ITokenMatching<T> ConvertIfMatched<T>(this ITokenMatching tokenMatching, Func<T> convert, IMatchedTokenMatching source)
        {
            if (tokenMatching.SafeIs(out IMatchedTokenMatching matched))
            {
                return TokenMatching<T>.MakeMatch(source, convert(),  matched.EndIndex);
            }
            return TokenMatching<T>.MakeNotMatch(tokenMatching.Context);
        }
    }

    internal static class TokenMatching<T>
    {
        private class Start : IMatchedTokenMatching
        {
            public Start(IReadOnlyList<IOrType<IToken, ISetUp>> allTokens, ElementMatchingContext context, int currentIndex)
            {
                AllTokens = allTokens ?? throw new ArgumentNullException(nameof(allTokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                StartIndex = currentIndex;
                EndIndex = currentIndex;
            }

            public IReadOnlyList<IOrType<IToken, ISetUp>> AllTokens
            {
                get;
            }

            public ElementMatchingContext Context
            {
                get;
            }

            public int EndIndex { get; }
            public int StartIndex { get; }
        }

        private class Matched : IMatchedTokenMatching<T>
        {
            public Matched(IReadOnlyList<IOrType<IToken, ISetUp>> allTokens, ElementMatchingContext context, T value, int startIndex, int endIndex)
            {
                AllTokens = allTokens ?? throw new ArgumentNullException(nameof(allTokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value = value;
                EndIndex = endIndex;
                StartIndex = startIndex;
            }

            public IReadOnlyList<IOrType<IToken, ISetUp>> AllTokens
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

            public int EndIndex { get; }
            public int StartIndex { get; }
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
        
        public static IMatchedTokenMatching MakeStart(IReadOnlyList<IOrType<IToken, ISetUp>> tokens, ElementMatchingContext context, int currentIndex)
        {
            return new Start(tokens, context, currentIndex);
        }


        public static IMatchedTokenMatching<T> MakeMatch(IMatchedTokenMatching parent, T value, int endIndex)
        {
            return new Matched(parent.AllTokens, parent.Context, value, parent.EndIndex, endIndex);
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
            public Matched(IReadOnlyList<IOrType<IToken, ISetUp>> allTokens, ElementMatchingContext context, T1 value1, T2 value2, int startIndex, int currentIndex)
            {
                AllTokens = allTokens ?? throw new ArgumentNullException(nameof(allTokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value1 = value1; 
                Value2 = value2;
                EndIndex = currentIndex;
                StartIndex = startIndex;
            }

            public IReadOnlyList<IOrType<IToken, ISetUp>> AllTokens
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
            public int EndIndex { get; }
            public int StartIndex { get; }
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

        public static IMatchedTokenMatching<T1,T2> MakeMatch(IMatchedTokenMatching<T1> parent, T1 value1, T2 value2, int endIndex)
        {
            return new Matched(parent.AllTokens, parent.Context, value1, value2, parent.EndIndex, endIndex);
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
            public Matched(IReadOnlyList<IOrType<IToken, ISetUp>> allTokens, ElementMatchingContext context, T1 value1, T2 value2, T3 value3, int startIndex, int endIndex)
            {
                AllTokens = allTokens ?? throw new ArgumentNullException(nameof(allTokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
                StartIndex = startIndex;
                EndIndex = endIndex;
            }

            public IReadOnlyList<IOrType<IToken, ISetUp>> AllTokens
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
            public int StartIndex { get; }
            public int EndIndex { get; }
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

        public static IMatchedTokenMatching<T1, T2, T3> MakeMatch(IMatchedTokenMatching<T1, T2> parent, T1 value1, T2 value2, T3 value3, int endIndex)
        {
            return new Matched(parent.AllTokens, parent.Context, value1, value2, value3, parent.EndIndex, endIndex);
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
            public Matched(IReadOnlyList<IOrType<IToken, ISetUp>> allTokens, ElementMatchingContext context, T1 value1, T2 value2, T3 value3, T4 value4, int startIndex, int endIndex)
            {
                AllTokens = allTokens ?? throw new ArgumentNullException(nameof(allTokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
                Value4 = value4;
                StartIndex = startIndex;
                EndIndex = endIndex;
            }

            public IReadOnlyList<IOrType<IToken, ISetUp>> AllTokens
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
            public int EndIndex { get; }
            public int StartIndex { get; }
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
            IMatchedTokenMatching<T1,T2,T3> parent,
            T1 value1, 
            T2 value2, 
            T3 value3,
            T4 value4, 
            int currentIndex)
        {
            return new Matched(parent.AllTokens, parent.Context, value1, value2, value3, value4, parent.EndIndex, currentIndex);
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
            public Matched(IReadOnlyList<IOrType<IToken, ISetUp>> allTokens, ElementMatchingContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, int startIndex, int endIndex)
            {
                AllTokens = allTokens ?? throw new ArgumentNullException(nameof(allTokens));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
                Value4 = value4;
                Value5 = value5;
                StartIndex = startIndex;
                EndIndex = endIndex;
            }

            public IReadOnlyList<IOrType<IToken, ISetUp>> AllTokens
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
            public int StartIndex { get; }
            public int EndIndex { get; }
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
    IMatchedTokenMatching<T1,T2,T3,T4> parent,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5, 
            int endIndex)
        {
            return new Matched(parent.AllTokens, parent.Context, value1, value2, value3, value4, value5, parent.EndIndex, endIndex);
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

        public static ITokenMatching<T> Has<T>(this ITokenMatching self, IMaker<T> pattern, out T t)
        {
            if (! (self is IMatchedTokenMatching firstMatched))
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                t = default;
#pragma warning restore CS8601 // Possible null reference assignment.
                return TokenMatching<T>.MakeNotMatch(self.Context);
            }

            var res = pattern.TryMake(firstMatched);
            if (res is IMatchedTokenMatching<T> matched)
            {
                t = matched.Value;
                return res;
            }
#pragma warning disable CS8601 // Possible null reference assignment.
            t = default;
#pragma warning restore CS8601 // Possible null reference assignment.
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
                return matched;
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
                    return TokenMatching<T1, T2>.MakeMatch(firstMatched, firstMatched.Value, matched.Value, matched.EndIndex );
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
                    return TokenMatching<T1, T2,T3>.MakeMatch(firstMatched, firstMatched.Value1, firstMatched.Value2, matched.Value, matched.EndIndex );
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
                    return TokenMatching<T1, T2, T3,T4>.MakeMatch(firstMatched, firstMatched.Value1, firstMatched.Value2, firstMatched.Value3, matched.Value, matched.EndIndex);
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
                    return TokenMatching<T1, T2, T3, T4,T5>.MakeMatch(firstMatched, firstMatched.Value1, firstMatched.Value2, firstMatched.Value3, firstMatched.Value4, matched.Value, matched.EndIndex);
                }
            }

            return TokenMatching<T1, T2, T3, T4,T5>.MakeNotMatch(self.Context);
        }

        //public static ITokenMatching<T> HasStruct<T>(this ITokenMatching self, IMaker<T> pattern, out T t)
        //    where T : struct
        //{

        //    if (!(self is IMatchedTokenMatching firstMatched))
        //    {
        //        t = default;
        //        return TokenMatching<T>.MakeNotMatch(self.Context);
        //    }

        //    var res = pattern.TryMake(firstMatched);
        //    if (res is IMatchedTokenMatching<T> matched)
        //    {
        //        t = matched.Value;
        //        return res;
        //    }

        //    t = default;
        //    return res;
        //}


        public static ITokenMatching HasSquare(this ITokenMatching self, Func<IMatchedTokenMatching, ITokenMatching> inner)
        {
            if (!(self is IMatchedTokenMatching matchedTokenMatching))
            {
                return self;
            }

            var index = matchedTokenMatching.EndIndex;

            if (matchedTokenMatching.AllTokens.Count < index)
            {
                return TokenMatching<object>.MakeNotMatch(self.Context);
            }

            if (matchedTokenMatching.AllTokens[index].Is1(out var token)  && token.SafeIs(out SquareBacketToken squareBacketToken))
            {
                if (inner(TokenMatching<object>.MakeStart(squareBacketToken.Tokens.Select(x=> OrType.Make<IToken, ISetUp>(x)).ToList(), self.Context, 0)) is IMatchedTokenMatching<object> mtm) {
                    return TokenMatching<object>.MakeMatch(matchedTokenMatching, mtm.Value, index+1);
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

            if (matchedTokenMatching.AllTokens.Count <= matchedTokenMatching.EndIndex) {
                return TokenMatching<object>.MakeNotMatch(self.Context);
            }

            if (matchedTokenMatching.AllTokens[matchedTokenMatching.EndIndex].Is1(out var token) && token.SafeIs(out LineToken line))
            {
                if (inner(TokenMatching<object>.MakeStart(line.Tokens.Select(x => OrType.Make<IToken, ISetUp>(x)).ToList(), self.Context,0)) is IMatchedTokenMatching)
                {
                    // uhhh new object?
                    // todo make a IMatchedTokenMatching with out a generic just for this
                    return TokenMatching<object>.MakeMatch(matchedTokenMatching, new object(), matchedTokenMatching.EndIndex+1);
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

            return TokenMatching<T>.MakeMatch(matchedTokenMatching, matchedTokenMatching.Value, matchedPattern.EndIndex);
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
