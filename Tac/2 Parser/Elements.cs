using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac._2_Parser;
using Tac.New;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public interface IOperationBuilder
    {
        IReadOnlyList<Func<ICodeElement, ICodeElement, ICodeElement>> Operations { get; }
        Func<ICodeElement, ICodeElement, AddOperation> AddOperation { get; }
        Func<ICodeElement, ICodeElement, SubtractOperation> SubtractOperation { get; }
        Func<ICodeElement, ICodeElement, MultiplyOperation> MultiplyOperation { get; }
        Func<ICodeElement, ICodeElement, IfTrueOperation> IfTrueOperation { get; }
        Func<ICodeElement, ICodeElement, ElseOperation> ElseOperation { get; }
        Func<ICodeElement, ICodeElement, LessThanOperation> LessThanOperation { get; }
        Func<ICodeElement, ICodeElement, NextCallOperation> NextCallOperation { get; }
        Func<ICodeElement, ICodeElement, AssignOperation> AssignOperation { get; }
        Func<ICodeElement, ICodeElement, PathOperation> PathOperation { get; }
        Func<ICodeElement, ReturnOperation> ReturnOperation { get; }
    }

    public interface IElementBuilders
    {
        Func<int, IBox<MemberDefinition>, Member> Member { get; }
        Func<string, ExplicitMemberName> ExplicitMemberName { get; }
        Func<string, ExplicitTypeName> ExplicitTypeName { get; }
        Func<string, ITypeDefinition[], GenericNameKey> GenericExplicitTypeName { get; }
        Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> ObjectDefinition { get; }
        Func<IScope, IEnumerable<ICodeElement>, ModuleDefinition> ModuleDefinition { get; }
        Func<MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, MethodDefinition> MethodDefinition { get; }
        Func<IScope, TypeDefinition> TypeDefinition { get; }
        Func<IScope, string, NamedTypeDefinition> NamedTypeDefinition { get; }
        Func<NameKey, ObjectScope, GenericTypeParameterDefinition[], GenericTypeDefinition> GenericTypeDefinition { get; }
        Func<MemberDefinition, MemberDefinition, IBox<ITypeDefinition>, IEnumerable<ICodeElement>, IScope, IEnumerable<ICodeElement>, ImplementationDefinition> ImplementationDefinition { get; }
        Func<ICodeElement[], IScope, IEnumerable<ICodeElement>, BlockDefinition> BlockDefinition { get; }
        Func<double, ConstantNumber> ConstantNumber { get; }
        Func<int, MemberDefinition, Member> MemberPath { get; }
        Func<IBox<MemberDefinition>, PathPart> PathPart { get; }
    }

    public class ElementMatchingContext
    {


        internal ElementMatchingContext Child(IScope scope)
        {
        }


        public ElementMatchingContext(IElementBuilders builders, IOperationBuilder operationBuilder, ScopeStack scope) {
            OperationMatchers = new IOperationMaker<ICodeElement>[] {
                new AddOperationMaker(operationBuilder.AddOperation),
                new SubtractOperationMaker(operationBuilder.SubtractOperation),
                new MultiplyOperationMaker(operationBuilder.MultiplyOperation),
                new IfTrueOperationMaker(operationBuilder.IfTrueOperation),
                new ElseOperationMaker(operationBuilder.ElseOperation),
                new LessThanOperationMaker(operationBuilder.LessThanOperation),
                new NextCallOperationMaker(operationBuilder.NextCallOperation),
                new AssignOperationMaker(operationBuilder.AssignOperation),
                new PathOperationMaker(operationBuilder.PathOperation),
                new ReturnOperationMaker(operationBuilder.ReturnOperation)
            };
            ElementMakers = new IMaker<ICodeElement>[] {
                new BlockDefinitionMaker(builders.BlockDefinition),
                new ConstantNumberMaker(builders.ConstantNumber),
                new GenericTypeDefinitionMaker(builders.GenericTypeDefinition),
                new ImplicitMemberMaker(builders.Member),
                new ImplementationDefinitionMaker(builders.ImplementationDefinition,builders),
                new MemberMaker(builders.Member,builders),
                new MemberDefinitionMaker(builders.Member,builders),
                new MethodDefinitionMaker(builders.MethodDefinition,builders),
                new ModuleDefinitionMaker(builders.ModuleDefinition),
                new ObjectDefinitionMaker(builders.ObjectDefinition),
                new PathPartMaker(builders.PathPart,builders),
                new TypeDefinitionMaker(builders.TypeDefinition,builders.NamedTypeDefinition),
            };
        }

        private readonly IMaker<ICodeElement>[] ElementMakers;
        private readonly IOperationMaker<ICodeElement>[] OperationMatchers;

        public ScopeStack ScopeStack { get;  }

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

                foreach (var tryMatch in ElementMakers)
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
            foreach (var operationMatcher in OperationMatchers)
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

        public IPopulateScope<ICodeElement>[] ParseBlock(CurleyBacketToken block)
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

    public class TokenMatching
    {

        private TokenMatching(IEnumerable<IToken> tokens, bool isNotMatch)
        {
            IsNotMatch = isNotMatch;
            Tokens = tokens;
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

        public static TokenMatching Start(IEnumerable<IToken> tokens)
        {
            return Match(tokens);
        }

        public static TokenMatching Match(IEnumerable<IToken> tokens)
        {
            return new TokenMatching(tokens, false);
        }

        public static TokenMatching NotMatch(IEnumerable<IToken> tokens)
        {
            return new TokenMatching(tokens, true);
        }

    }

    public static class ElementMatcher
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
                return TokenMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            atomicToken = default;
            return TokenMatching.NotMatch(self.Tokens);
        }

        public static TokenMatching IsType(TokenMatching self, out NameKey typeSource)
        {

            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                var at = TokenMatching.Match(self.Tokens.Skip(1));
                if (GenericN(at, out var keys).IsMatch)
                {
                    typeSource = new GenericNameKey(new NameKey(first.Item), keys);
                    
                    return TokenMatching.Match(self.Tokens.Skip(2).ToArray());
                }


                typeSource = new NameKey(first.Item);
                
                return TokenMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            typeSource = default;
            return TokenMatching.NotMatch(self.Tokens);

        }

        public static TokenMatching IsNumber(TokenMatching self, out double res)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                double.TryParse(first.Item, out res))
            {
                return TokenMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            res = default;
            return TokenMatching.NotMatch(self.Tokens);
        }

        public static TokenMatching IsDone(TokenMatching self)
        {
            if (!self.Tokens.Any())
            {
                return self;
            }

            return TokenMatching.NotMatch(self.Tokens);
        }

        public static TokenMatching IsBody(TokenMatching self, out CurleyBacketToken body)
        {

            if (self.Tokens.Any() &&
                self.Tokens.First() is CurleyBacketToken first)
            {
                body = first;
                return TokenMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            body = default;
            return TokenMatching.NotMatch(self.Tokens);
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
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            type1 = default;
            type2 = default;
            type3 = default;
            return TokenMatching.NotMatch(elementMatching.Tokens);
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
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            tokens = default;
            return TokenMatching.NotMatch(elementMatching.Tokens);
        }

        public static TokenMatching GenericN(TokenMatching elementMatching, out NameKey[] typeSources)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                typeParameters.Tokens.All(x => x is ElementToken) &&
                TryToToken(out var res))
            {
                typeSources = res;
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            typeSources = default;
            return TokenMatching.NotMatch(elementMatching.Tokens);

            bool TryToToken(out NameKey[] typeSourcesInner)
            {
                var typeSourcesBuilding = new List<NameKey>();
                foreach (var elementToken in typeParameters.Tokens.OfType<ElementToken>())
                {
                    var matcher = TokenMatching.Start(elementToken.Tokens);
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
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            type1 = default;
            type2 = default;
            return TokenMatching.NotMatch(elementMatching.Tokens);
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

                    var at = TokenMatching.Match(elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1).ToArray());

                    if (at.Tokens.Any() &&
                    at.Tokens.Last() is AtomicToken op &&
                    op.Item == s)
                    {

                        rhs = right;
                        operation = op;
                        preface = at.Tokens.Take(at.Tokens.Count() - 1);
                        return TokenMatching.Match(preface);
                    }
                }

                rhs = default;
                preface = default;
                operation = default;
                return TokenMatching.NotMatch(elementMatching.Tokens);

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
                    return TokenMatching.Match(elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1).ToArray());
                }

                preface = default;
                operation = default;
                return TokenMatching.NotMatch(elementMatching.Tokens);
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
                    return TokenMatching.Match(self.Tokens.Skip(1).ToArray());
                }

                token = default;
                return TokenMatching.NotMatch(self.Tokens);
            };
        }

        public static IsMatch Xor(this IsMatch self, IsMatch other)
        {
            return (TokenMatching element) =>
            {
                var first = self(element);
                var second = other(element);

                var table = new Dictionary<(bool, bool), Func<TokenMatching>>() {
                    { (true,true), ()=>TokenMatching.NotMatch(element.Tokens)},
                    { (true,false), ()=> second},
                    { (false,true), ()=> first},
                    { (false,false), ()=> TokenMatching.NotMatch(element.Tokens)},
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
                        return TokenMatching.NotMatch(element.Tokens);
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
                        return TokenMatching.NotMatch(element.Tokens);
                    }
                }
            };
        }


    }

}
