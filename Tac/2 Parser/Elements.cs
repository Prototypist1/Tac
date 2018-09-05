using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class ParsingContext {

    }

    public class Elements
    {
        public Elements(List<TryMatch> elementBuilders) => ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));

        public delegate bool TryMatch(ElementToken elementToken, out ICodeElement element);

        public List<TryMatch> ElementBuilders { get; }

        public static Lazy<Elements> StandardElements = new Lazy<Elements>(() =>
        {
            return new Elements(
                new List<TryMatch> {
                    MatchStaticMemberDefinition_Var,
                    MatchObjectDefinition,
                    MatchLocalDefinition_Var,
                    MatchMethodDefinition,
                    MatchBlockDefinition,
                    MatchConstantNumber,
                    MatchReferance
                });
        });

        public static bool MatchLocalDefinition_Var(ElementToken elementToken, out ICodeElement element) {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch) {

                var readOnly = readonlyToken != default;

                element = new MemberDefinition(readOnly, false, new ExplicitName(nameToken.Item), new ImplicitTypeReferance());

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchStaticMemberDefinition_Var(ElementToken elementToken, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("static"), out var _)
                .OptionalHas(ElementMatcher.KeyWord("var"), out var _)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var readOnly = readonlyToken != default;

                element = new MemberDefinition(readOnly, true, new ExplicitName(nameToken.Item), new ImplicitTypeReferance());

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchObjectDefinition(ElementToken elementToken, out ICodeElement element) {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("object"), out var keyword)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken block)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var scope = new ObjectScope();

                var elements = TokenParser.ParseBlock(block);

                var localDefininitions = elements.OfType<AssignOperation>().ToArray();

                if (!elements.All(x => x is AssignOperation assignOperation && (assignOperation.right is MemberReferance || !(assignOperation.right as MemberDefinition).IsStatic))) {
                    throw new Exception("all lines in an object should be none static");
                }

                foreach (var loaclDefinition in localDefininitions)
                {
                    if (loaclDefinition.right is MemberDefinition memberDefinition)
                    {
                        scope.TryAddLocalMember(memberDefinition);
                    }
                    else if (loaclDefinition.right is MemberReferance referance) {
                        scope.TryAddLocalMember(new MemberDefinition(false, false, referance.key.names.Single()));
                    } else {
                        throw new Exception(loaclDefinition.right + "is of unexpected type");
                    }
                }

                element = new ObjectDefinition(scope, localDefininitions);
                return true;
            }
            element = default;
            return false;
        }

        public static bool MatchModuleDefinition(ElementToken elementToken, out ICodeElement element) {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("module"), out AtomicToken frist)
                .Has(ElementMatcher.IsName, out AtomicToken second)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken third)
                .Has(ElementMatcher.IsDone)
                .IsMatch) {

                var scope = new StaticScope();

                var elements = TokenParser.ParseBlock(third);


                var staticDefininitions = elements.OfType<AssignOperation>().ToArray();

                var types = elements.OfType<TypeDefinition>().ToArray();

                if (!elements.All(x => x is AssignOperation assignOperation && (assignOperation.right is MemberReferance || (assignOperation.right as MemberDefinition).IsStatic)))
                {
                    throw new Exception("all lines in an object should be none static");
                }

                foreach (var staticDefinition in staticDefininitions)
                {
                    if (staticDefinition.right is MemberDefinition memberDefinition)
                    {
                        scope.TryAddStaticMember(memberDefinition);
                    }
                    else if (staticDefinition.right is MemberReferance referance)
                    {
                        scope.TryAddStaticMember(new MemberDefinition(false, true, referance.key.names.Single()));
                    }
                    else
                    {
                        throw new Exception(staticDefinition.right + "is of unexpected type");
                    }
                }

                foreach (var type in types)
                {
                    scope.TryAddStaticType(type);
                }

                element = new ModuleDefinition(new ExplicitName(second.Item), scope, staticDefininitions);

            }
            element = default;
            return false;
        }

        public static bool MatchMethodDefinition(ElementToken elementToken, out ICodeElement element)
        {
            if (
                elementToken.Tokens.Count() == 3 &&
                elementToken.Tokens.First() is AtomicToken first &&
                    first.Item == "method" &&
                elementToken.Tokens.ElementAt(1) is ParenthesisToken typeParameters &&
                    typeParameters.Tokens.Count() == 2 &&
                    typeParameters.Tokens.ElementAt(0) is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is AtomicToken inputType &&
                    typeParameters.Tokens.ElementAt(1) is LineToken secondLine &&
                        secondLine.Tokens.Count() == 1 &&
                        secondLine.Tokens.ElementAt(0) is AtomicToken outputType &&
                elementToken.Tokens.ElementAt(2) is AtomicToken second &&
                elementToken.Tokens.ElementAt(3) is CurleyBacketToken third) {

                var methodScope = new MethodScope();

                var elements = TokenParser.ParseBlock(third);


                var staticDefininitions = elements.OfType<AssignOperation>().ToArray();

                var types = elements.OfType<TypeDefinition>().ToArray();

                if (!elements.All(x => x is AssignOperation assignOperation && (assignOperation.right is MemberReferance || (assignOperation.right as MemberDefinition).IsStatic)))
                {
                    throw new Exception("all lines in an object should be none static");
                }

                foreach (var staticDefinition in staticDefininitions)
                {
                    if (staticDefinition.right is MemberDefinition memberDefinition)
                    {
                        methodScope.TryAddStaticMember(memberDefinition);
                    }
                    else if (staticDefinition.right is MemberReferance referance)
                    {
                        methodScope.TryAddStaticMember(new MemberDefinition(false, true, referance.key.names.Single()));
                    }
                    else
                    {
                        throw new Exception(staticDefinition.right + "is of unexpected type");
                    }
                }

                foreach (var type in types)
                {
                    methodScope.TryAddStaticType(type);
                }

                element = new MethodDefinition(
                    new MemberReferance(inputType.Item),
                    new ParameterDefinition(
                        false, // TODO, the way this is hard coded is something to think about, readonly should be encoded somewhere!
                        new MemberReferance(inputType.Item),
                        new ExplicitName(second.Item)),
                    elements.Except(staticDefininitions).Except(types).ToArray(),
                    methodScope,
                    staticDefininitions);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchBlockDefinition(ElementToken elementToken, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var scope = new LocalStaticScope();

                var elements = TokenParser.ParseBlock(first);


                var staticDefininitions = elements.OfType<AssignOperation>().ToArray();

                var types = elements.OfType<TypeDefinition>().ToArray();

                if (!elements.All(x => x is AssignOperation assignOperation && (assignOperation.right is MemberReferance || (assignOperation.right as MemberDefinition).IsStatic)))
                {
                    throw new Exception("all lines in an object should be none static");
                }

                foreach (var staticDefinition in staticDefininitions)
                {
                    if (staticDefinition.right is MemberDefinition memberDefinition)
                    {
                        scope.TryAddStaticMember(memberDefinition);
                    }
                    else if (staticDefinition.right is MemberReferance referance)
                    {
                        scope.TryAddStaticMember(new MemberDefinition(false, true, referance.key.names.Single()));
                    }
                    else
                    {
                        throw new Exception(staticDefinition.right + "is of unexpected type");
                    }
                }

                foreach (var type in types)
                {
                    scope.TryAddStaticType(type);
                }

                element = new BlockDefinition(
                    elements.Except(staticDefininitions).Except(types).ToArray(), scope, staticDefininitions);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchConstantNumber(ElementToken elementToken, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.IsNumber, out double dub)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                element = new ConstantNumber(dub);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchReferance(ElementToken elementToken, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                element = new MemberReferance(first.Item);

                return true;
            }

            element = default;
            return false;
        }

    }

    public class ElementMatching {
        
        private ElementMatching(IEnumerable<IToken> tokens, bool isNotMatch)
        {
            this.IsNotMatch = isNotMatch;
            this.Tokens = tokens;
        }

        public bool IsMatch { get => !IsNotMatch; }
        public bool IsNotMatch { get; }
        public IEnumerable<IToken> Tokens { get; }

        public static ElementMatching Start(ElementToken elementToken) {
            return Match(elementToken.Tokens);
        }

        public static ElementMatching Match(IEnumerable<IToken> tokens) {
            return new ElementMatching(tokens, false);
        }

        public static ElementMatching NotMatch(IEnumerable<IToken> tokens)
        {
            return new ElementMatching(tokens, true);
        }

    }

    public static class ElementMatcher {
        public static ElementMatching Has<T>(this ElementMatching self, IsMatch<T> pattern, out T t) {
            if (self.IsNotMatch) {
                t = default;
                return self;
            }

            return pattern(self, out t);
        }

        public static ElementMatching Has(this ElementMatching self, IsMatch pattern)
        {
            if (self.IsNotMatch)
            {
                return self;
            }

            return pattern(self);
        }
        
        public static ElementMatching OptionalHas<T>(this ElementMatching self, IsMatch<T> pattern, out T t) {
            if (self.IsNotMatch)
            {
                t = default;
                return self;
            }

            var next = pattern(self, out t);
            if (next.IsNotMatch) {
                t = default;
                return self;
            }

            return next;

        }

        public static ElementMatching OptionalHas(this ElementMatching self, IsMatch pattern)
        {
            if (self.IsNotMatch)
            {
                return self;
            }

            var next = pattern(self);
            if (next.IsNotMatch) {
                return self;
            }

            return next;
        }

        public delegate ElementMatching IsMatch(ElementMatching self);
        public delegate ElementMatching IsMatch<T>(ElementMatching self, out T matched);
        
        public static ElementMatching IsName(ElementMatching self, out AtomicToken atomicToken)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _)) {
                atomicToken = first;
                return ElementMatching.Match(self.Tokens.Skip(1).ToArray());
            }
            
            atomicToken = default;
            return ElementMatching.NotMatch(self.Tokens);
        }

        public static ElementMatching IsNumber(ElementMatching self, out double res)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                double.TryParse(first.Item, out res))
            {
                return ElementMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            res = default;
            return ElementMatching.NotMatch(self.Tokens);
        }

        public static ElementMatching IsDone(ElementMatching self) {
            if (!self.Tokens.Any())
            {
                return self;
            }

            return ElementMatching.NotMatch(self.Tokens);
        }

        public static ElementMatching IsBody(ElementMatching self, out CurleyBacketToken body) {

            if (self.Tokens.Any() &&
                self.Tokens.First() is CurleyBacketToken first)
            {
                body = first;
                return ElementMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            body = default;
            return ElementMatching.NotMatch(self.Tokens);
        }

        public static IsMatch<AtomicToken> KeyWord(string word) {
            return Inner;

            ElementMatching Inner(ElementMatching self, out AtomicToken token)
            {
                if (self.Tokens.First() is AtomicToken first &&
                    first.Item == word)
                {
                    token = first;
                    return ElementMatching.Match(self.Tokens.Skip(1).ToArray());
                }

                token = default;
                return ElementMatching.NotMatch(self.Tokens);
            };
        }

        public static IsMatch Xor(this IsMatch self, IsMatch other)
        {
            return (ElementMatching element) =>
            {
                var first = self(element);
                var second = other(element);

                var table = new Dictionary<(bool, bool), Func<ElementMatching>>() {
                    { (true,true), ()=>ElementMatching.NotMatch(element.Tokens)},
                    { (true,false), ()=> second},
                    { (false,true), ()=> first},
                    { (false,false), ()=> ElementMatching.NotMatch(element.Tokens)},
                };

                return table[(first.IsNotMatch,second.IsNotMatch)]();
            };
        }

        public static IsMatch<T> Xor<T>(this IsMatch<T> self, IsMatch<T> other)
        {
            return Backing;

            ElementMatching Backing(ElementMatching element, out T t)
            {
                var first = self(element,out var t1);
                var second = other(element, out var t2);

                if (first.IsNotMatch)
                {
                    if (second.IsNotMatch)
                    {
                        t = default;
                        return ElementMatching.NotMatch(element.Tokens);
                    }
                    else
                    {
                        t = t2;
                        return second;
                    }
                }
                else {
                    if (second.IsNotMatch)
                    {
                        t = t1;
                        return first;
                    }
                    else
                    {
                        t = default;
                        return ElementMatching.NotMatch(element.Tokens);
                    }
                }
            };
        }

    }
    
    
}
