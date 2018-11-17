using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
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

        internal ElementMatchingContext ExpectPathPart(IBox<IVarifiableType> box) {
            return new ElementMatchingContext(operationMatchers, new IMaker<ICodeElement>[] {
                new MemberReferanceMaker(box)
            });
        }
        
        internal ElementMatchingContext AcceptImplicit(IBox<IVarifiableType> box)
        {
            return new ElementMatchingContext(operationMatchers, new IMaker<ICodeElement>[] {
                new BlockDefinitionMaker(),
                new ConstantNumberMaker(),
                new GenericTypeDefinitionMaker(),
                new ImplementationDefinitionMaker(),
                new MemberDefinitionMaker(),
                new MethodDefinitionMaker(),
                new ModuleDefinitionMaker(),
                new ObjectDefinitionMaker(),
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
                new IOperationMaker<ICodeElement>[] {
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
                new IMaker<ICodeElement>[] {
                    new BlockDefinitionMaker(),
                    new ConstantNumberMaker(),
                    new GenericTypeDefinitionMaker(),
                    new ImplementationDefinitionMaker(),
                    new MemberDefinitionMaker(),
                    new MethodDefinitionMaker(),
                    new ModuleDefinitionMaker(),
                    new ObjectDefinitionMaker(),
                    new TypeDefinitionMaker(),
                    new MemberMaker(),
                }){}
        
        public ElementMatchingContext(IOperationMaker<ICodeElement>[] operationMatchers, IMaker<ICodeElement>[] elementMakers)
        {
            this.operationMatchers = operationMatchers ?? throw new ArgumentNullException(nameof(operationMatchers));
            this.elementMakers = elementMakers ?? throw new ArgumentNullException(nameof(elementMakers));
        }

        private readonly IMaker<ICodeElement>[] elementMakers;
        private readonly IOperationMaker<ICodeElement>[] operationMatchers;
        
        #region Parse

        public IPopulateScope<ICodeElement> ParseParenthesisOrElement(IToken token)
        {
            if (token is ElementToken elementToken)
            {
                // smells 
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken)
                {
                    return ParseLine(parenthesisToken.Tokens);
                }

                foreach (var tryMatch in elementMakers)
                {
                    if (tryMatch.TryMake(elementToken, this).TryGetValue(out var obj))
                    {
                        return obj;
                    }
                }
            }
            else if (token is ParenthesisToken parenthesisToken)
            {
                return ParseLine(parenthesisToken.Tokens);
            }

            throw new Exception("");
        }

        public IPopulateScope<ICodeElement> ParseLine(IEnumerable<IToken> tokens)
        {
            foreach (var operationMatcher in operationMatchers)
            {
                if (operationMatcher.TryMake(tokens, this).TryGetValue(out var obj))
                {
                    return obj;
                }
            }

            if (tokens.Count() == 1)
            {
                return ParseParenthesisOrElement(tokens.Single());
            }

            throw new Exception("");
        }

        public IPopulateScope<ICodeElement>[] ParseFile(FileToken file)
        {
            return file.Tokens.Select(x => ParseLine(x.Cast<LineToken>().Tokens)).ToArray();
        }

        public IPopulateScope<ICodeElement>[] ParseBlock(CurleyBracketToken block)
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

    internal class TokenMatching
    {

        private TokenMatching(IEnumerable<IToken> tokens, bool isNotMatch, ElementMatchingContext Context)
        {
            IsNotMatch = isNotMatch;
            this.Context = Context ?? throw new ArgumentNullException(nameof(Context));
            Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        public bool IsMatch
        {
            get
            {
                return !IsNotMatch;
            }
        }
        public bool IsNotMatch { get; }
        public IEnumerable<IToken> Tokens { get; }
        public ElementMatchingContext Context { get;  }

        public static TokenMatching Start(IEnumerable<IToken> tokens, ElementMatchingContext context)
        {
            return Match(tokens, context);
        }

        public static TokenMatching Match(IEnumerable<IToken> tokens, ElementMatchingContext context)
        {
            return new TokenMatching(tokens, false, context);
        }

        public static TokenMatching NotMatch(IEnumerable<IToken> tokens, ElementMatchingContext context)
        {
            return new TokenMatching(tokens, true, context);
        }

    }

    internal static class ElementMatcher
    {
        public static TokenMatching Has<T1, T2, T3>(this TokenMatching self, IsMatch<T1, T2, T3> pattern, out T1 t1, out T2 t2, out T3 t3)
        {
            if (self.IsNotMatch)
            {
                t1 = default;
                t2 = default;
                t3 = default;
                return self;
            }

            return pattern(self, out t1, out t2, out t3);
        }

        public static TokenMatching Has<T1, T2>(this TokenMatching self, IsMatch<T1, T2> pattern, out T1 t1, out T2 t2)
        {
            if (self.IsNotMatch)
            {
                t1 = default;
                t2 = default;
                return self;
            }

            return pattern(self, out t1, out t2);
        }

        public static TokenMatching Has<T>(this TokenMatching self, IsMatch<T> pattern, out T t)
        {
            if (self.IsNotMatch)
            {
                t = default;
                return self;
            }

            return pattern(self, out t);
        }

        public static TokenMatching Has(this TokenMatching self, IsMatch pattern)
        {
            if (self.IsNotMatch)
            {
                return self;
            }

            return pattern(self);
        }

        public static TokenMatching OptionalHas<T>(this TokenMatching self, IsMatch<T> pattern, out T t)
        {
            if (self.IsNotMatch)
            {
                t = default;
                return self;
            }

            var next = pattern(self, out t);
            if (next.IsNotMatch)
            {
                t = default;
                return self;
            }

            return next;

        }

        public static TokenMatching OptionalHas(this TokenMatching self, IsMatch pattern)
        {
            if (self.IsNotMatch)
            {
                return self;
            }

            var next = pattern(self);
            if (next.IsNotMatch)
            {
                return self;
            }

            return next;
        }

        public delegate TokenMatching IsMatch(TokenMatching self);
        public delegate TokenMatching IsMatch<T>(TokenMatching self, out T matched);
        public delegate TokenMatching IsMatch<T1, T2>(TokenMatching self, out T1 matched1, out T2 matched2);
        public delegate TokenMatching IsMatch<T1, T2, T3>(TokenMatching self, out T1 matched1, out T2 matched2, out T3 matched3);

        public static TokenMatching IsName(TokenMatching self, out AtomicToken atomicToken)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                atomicToken = first;
                return TokenMatching.Match(self.Tokens.Skip(1).ToArray(), self.Context);
            }

            atomicToken = default;
            return TokenMatching.NotMatch(self.Tokens, self.Context);
        }

        public static TokenMatching IsType(TokenMatching self, out NameKey typeSource)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                var at = TokenMatching.Match(self.Tokens.Skip(1), self.Context);
                if (GenericN(at, out var keys).IsMatch)
                {
                    typeSource = new GenericNameKey(new NameKey(first.Item), keys);
                    
                    return TokenMatching.Match(self.Tokens.Skip(2).ToArray(), self.Context);
                }


                typeSource = new NameKey(first.Item);
                
                return TokenMatching.Match(self.Tokens.Skip(1).ToArray(), self.Context);
            }

            typeSource = default;
            return TokenMatching.NotMatch(self.Tokens, self.Context);

        }

        public static TokenMatching IsNumber(TokenMatching self, out double res)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                double.TryParse(first.Item, out res))
            {
                return TokenMatching.Match(self.Tokens.Skip(1).ToArray(), self.Context);
            }

            res = default;
            return TokenMatching.NotMatch(self.Tokens, self.Context);
        }

        public static TokenMatching IsDone(TokenMatching self)
        {
            if (!self.Tokens.Any())
            {
                return self;
            }

            return TokenMatching.NotMatch(self.Tokens, self.Context);
        }

        public static TokenMatching IsBody(TokenMatching self, out CurleyBracketToken body)
        {

            if (self.Tokens.Any() &&
                self.Tokens.First() is CurleyBracketToken first)
            {
                body = first;
                return TokenMatching.Match(self.Tokens.Skip(1).ToArray(), self.Context);
            }

            body = default;
            return TokenMatching.NotMatch(self.Tokens, self.Context);
        }

        public static TokenMatching Generic3(TokenMatching elementMatching, out AtomicToken type1, out AtomicToken type2, out AtomicToken type3)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                    typeParameters.Tokens.Count() == 3 &&
                    typeParameters.Tokens.ElementAt(0) is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is ElementToken firstElement &&
                        firstElement.Tokens.Count() == 1 &&
                        firstElement.Tokens.ElementAt(0) is AtomicToken firstType &&
                    typeParameters.Tokens.ElementAt(1) is LineToken secondLine &&
                        secondLine.Tokens.Count() == 1 &&
                        secondLine.Tokens.ElementAt(0) is ElementToken SecondElement &&
                        SecondElement.Tokens.Count() == 1 &&
                        SecondElement.Tokens.ElementAt(0) is AtomicToken SecondType &&
                    typeParameters.Tokens.ElementAt(2) is LineToken thridLine &&
                        thridLine.Tokens.Count() == 1 &&
                        thridLine.Tokens.ElementAt(0) is ElementToken thridElement &&
                        thridElement.Tokens.Count() == 1 &&
                        thridElement.Tokens.ElementAt(0) is AtomicToken thirdType)
            {
                type1 = firstType;
                type2 = SecondType;
                type3 = thirdType;
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context);
            }

            type1 = default;
            type2 = default;
            type3 = default;
            return TokenMatching.NotMatch(elementMatching.Tokens, elementMatching.Context);
        }

        public static TokenMatching DefineGenericN(TokenMatching elementMatching, out AtomicToken[] tokens)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                    typeParameters.Tokens.All(x => x is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is AtomicToken))
            {
                tokens = typeParameters.Tokens.Select(x => (x as LineToken).Tokens.First() as AtomicToken).ToArray();
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context);
            }

            tokens = default;
            return TokenMatching.NotMatch(elementMatching.Tokens, elementMatching.Context);
        }

        public static TokenMatching GenericN(TokenMatching elementMatching, out NameKey[] typeSources)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                typeParameters.Tokens.All(x => x is ElementToken) &&
                TryToToken(out var res))
            {
                typeSources = res;
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context);
            }

            typeSources = default;
            return TokenMatching.NotMatch(elementMatching.Tokens, elementMatching.Context);

            bool TryToToken(out NameKey[] typeSourcesInner)
            {
                var typeSourcesBuilding = new List<NameKey>();
                foreach (var elementToken in typeParameters.Tokens.OfType<ElementToken>())
                {
                    var matcher = TokenMatching.Start(elementToken.Tokens, elementMatching.Context);
                    if (matcher.Has(ElementMatcher.IsType, out NameKey typeSource).Has(IsDone).IsMatch)
                    {
                        typeSourcesBuilding.Add(typeSource);
                    }
                    else
                    {
                        typeSourcesInner = default;
                        return false;
                    }
                }
                typeSourcesInner = typeSourcesBuilding.ToArray();
                return true;
            }
        }

        public static TokenMatching Generic2(TokenMatching elementMatching, out AtomicToken type1, out AtomicToken type2)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                    typeParameters.Tokens.Count() == 2 &&
                    typeParameters.Tokens.ElementAt(0) is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is ElementToken firstElement &&
                        firstElement.Tokens.Count() == 1 &&
                        firstElement.Tokens.ElementAt(0) is AtomicToken firstType &&
                    typeParameters.Tokens.ElementAt(1) is LineToken secondLine &&
                        secondLine.Tokens.Count() == 1 &&
                        secondLine.Tokens.ElementAt(0) is ElementToken SecondElement &&
                        SecondElement.Tokens.Count() == 1 &&
                        SecondElement.Tokens.ElementAt(0) is AtomicToken SecondType)
            {
                type1 = firstType;
                type2 = SecondType;
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context);
            }

            type1 = default;
            type2 = default;
            return TokenMatching.NotMatch(elementMatching.Tokens, elementMatching.Context);
        }

        public static IsMatch<IEnumerable<IToken>, AtomicToken, IToken> IsBinaryOperation(string s)
        {
            return (TokenMatching elementMatching, out IEnumerable<IToken> preface, out AtomicToken operation, out IToken rhs) =>

            {
                if (elementMatching.Tokens.Any() &&
                (elementMatching.Tokens.Last() is ParenthesisToken ||
                elementMatching.Tokens.Last() is ElementToken)
                )
                {
                    var right = elementMatching.Tokens.Last();

                    var at = TokenMatching.Match(elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1).ToArray(), elementMatching.Context);

                    if (at.Tokens.Any() &&
                    at.Tokens.Last() is AtomicToken op &&
                    op.Item == s)
                    {

                        rhs = right;
                        operation = op;
                        preface = at.Tokens.Take(at.Tokens.Count() - 1);
                        return TokenMatching.Match(preface, elementMatching.Context);
                    }
                }

                rhs = default;
                preface = default;
                operation = default;
                return TokenMatching.NotMatch(elementMatching.Tokens, elementMatching.Context);

            };
        }

        public static IsMatch<IEnumerable<IToken>, AtomicToken> IsTrailingOperation(string s)
        {
            return (TokenMatching elementMatching, out IEnumerable<IToken> preface, out AtomicToken operation) =>
            {

                if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.Last() is AtomicToken op)
                {

                    preface = elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1);
                    operation = op;
                    return TokenMatching.Match(elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1).ToArray(), elementMatching.Context);
                }

                preface = default;
                operation = default;
                return TokenMatching.NotMatch(elementMatching.Tokens, elementMatching.Context);
            };
        }

        public static IsMatch<AtomicToken> KeyWord(string word)
        {
            return Inner;

            TokenMatching Inner(TokenMatching self, out AtomicToken token)
            {
                if (self.Tokens.First() is AtomicToken first &&
                    first.Item == word)
                {
                    token = first;
                    return TokenMatching.Match(self.Tokens.Skip(1).ToArray(), self.Context);
                }

                token = default;
                return TokenMatching.NotMatch(self.Tokens, self.Context);
            };
        }

        public static IsMatch Xor(this IsMatch self, IsMatch other)
        {
            return (TokenMatching element) =>
            {
                var first = self(element);
                var second = other(element);

                var table = new Dictionary<(bool, bool), Func<TokenMatching>>() {
                    { (true,true), ()=>TokenMatching.NotMatch(element.Tokens, element.Context)},
                    { (true,false), ()=> second},
                    { (false,true), ()=> first},
                    { (false,false), ()=> TokenMatching.NotMatch(element.Tokens, element.Context)},
                };

                return table[(first.IsNotMatch, second.IsNotMatch)]();
            };
        }

        public static IsMatch<T> Xor<T>(this IsMatch<T> self, IsMatch<T> other)
        {
            return Backing;

            TokenMatching Backing(TokenMatching element, out T t)
            {
                var first = self(element, out var t1);
                var second = other(element, out var t2);

                if (first.IsNotMatch)
                {
                    if (second.IsNotMatch)
                    {
                        t = default;
                        return TokenMatching.NotMatch(element.Tokens, element.Context);
                    }
                    else
                    {
                        t = t2;
                        return second;
                    }
                }
                else
                {
                    if (second.IsNotMatch)
                    {
                        t = t1;
                        return first;
                    }
                    else
                    {
                        t = default;
                        return TokenMatching.NotMatch(element.Tokens, element.Context);
                    }
                }
            };
        }


    }

}
