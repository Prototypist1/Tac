using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac._2_Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class ElementMatchingContext {
        public ElementMatchingContext(ScopeStack enclosingScope, IEnumerable<TryMatch> elementMatchers)
        {
            EnclosingScope = enclosingScope ?? throw new ArgumentNullException(nameof(enclosingScope));
            ElementMatchers = elementMatchers ?? throw new ArgumentNullException(nameof(elementMatchers));
        }

        public ScopeStack EnclosingScope { get; }
        
        public IEnumerable<TryMatch> ElementMatchers { get; }
        
        public static ElementMatchingContext StandMatchingContext(ScopeStack enclosingScope) =>
             new ElementMatchingContext(enclosingScope, 
                new List<TryMatch> {
                    MatchObjectDefinition,
                    MatchLocalDefinition_Var,
                    MatchGenericTypeDefinition,
                    MatchTypeDefinition,
                    MatchMethodDefinition,
                    MatchImplementationDefinition,
                    MatchBlockDefinition,
                    MatchConstantNumber,
                    MatchReferance
                });


        public static ElementMatchingContext StandMatchingContext() => StandMatchingContext(new ScopeStack(new IScope[] { }));

        public delegate bool TryMatch(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element);
        
        public static bool MatchLocalDefinition_Var(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            // if I do a good job on ElementMatching it can pick up var
            // all the definitions matchers can probably rolled in togther
            
            // when they day comes I should make the type entry optional

            // "readonly x" is a totally legitiment method definition

            // while I am thinking about including updates

            // there are a lot of things here that use () that should use []

            if (ElementMatching.Start(elementToken)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var readOnly = readonlyToken != default;

                element = new MemberDefinition(readOnly, new ExplicitMemberName(nameToken.Item), new ImplicitTypeReferance());

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchMemberDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            // TODO use ElementMatcher.IsType

            if (ElementMatching.Start(elementToken)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsName, out AtomicToken typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var readOnly = readonlyToken != default;

                element = new MemberDefinition(readOnly, new ExplicitMemberName(nameToken.Item), new ExplicitTypeName(typeToken.Item));

                return true;
            }

            element = default;
            return false;
        }


        public static bool MatchGenericMemberDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            // TODO use ElementMatcher.IsType

            if (ElementMatching.Start(elementToken)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsName, out AtomicToken typeToken)
                .Has(ElementMatcher.GenericN, out ITypeSource[] tokenSources)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var readOnly = readonlyToken != default;

                element = new MemberDefinition(readOnly, new ExplicitMemberName(nameToken.Item), new GenericExplicitTypeName(typeToken.Item, tokenSources));

                return true;
            }

            element = default;
            return false;
        }

        //public static bool MatchStaticMemberDefinition_Var(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        //{
        //    if (ElementMatching.Start(elementToken)
        //        .Has(ElementMatcher.KeyWord("static"), out var _)
        //        .OptionalHas(ElementMatcher.KeyWord("var"), out var _)
        //        .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
        //        .Has(ElementMatcher.IsName, out AtomicToken nameToken)
        //        .Has(ElementMatcher.IsDone)
        //        .IsMatch)
        //    {

        //        var readOnly = readonlyToken != default;

        //        element = new MemberDefinition(readOnly, true, new ExplicitName(nameToken.Item), new ImplicitTypeReferance());

        //        return true;
        //    }

        //    element = default;
        //    return false;
        //}

        public static bool MatchObjectDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("object"), out var keyword)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken block)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var scope = new ObjectScope();
                
                var elements = TokenParser.ParseBlock(block, StandMatchingContext(new ScopeStack(matchingContext.EnclosingScope,scope)));
                
                if (elements.ExtractTopLevelAssignOperations(out var assignOperations).Any())
                {
                    throw new Exception("objects should only contain assign operations");
                }

                if (assignOperations
                    .ExtractMemberDefinitions(out var memberDefinitions)
                    .ExtractMemberReferances(out var memberReferances)
                    .Any()) {
                    throw new Exception("objects should only assign to member definitions or member referances");
                }

                foreach (var memberDefinition in memberDefinitions)
                {
                    scope.TryAddLocalMember(memberDefinition);
                }

                foreach (var (left, memberReferance) in memberReferances)
                {
                    scope.TryAddLocalMember(new MemberDefinition(false, memberReferance.Key, new ImplicitTypeReferance(left)));
                }
                
                element = new ObjectDefinition(scope, assignOperations);

                return true;
            }
            element = default;
            return false;
        }
        
        public static bool MatchModuleDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("module"), out var frist)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken third)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var scope = new StaticScope();

                var elements = TokenParser.ParseBlock(third, StandMatchingContext(new ScopeStack(matchingContext.EnclosingScope, scope)));
                
                if (elements
                    .ExtractTopLevelAssignOperations(out var assignOperations)
                    .ExtractTopLevelTypeDefinitions(out var types)
                    .Any())
                {
                    throw new Exception("objects should only contain assign operations");
                }

                if (assignOperations
                    .ExtractMemberDefinitions(out var memberDefinitions)
                    .ExtractMemberReferances(out var memberReferances)
                    .Any())
                {
                    throw new Exception("objects should only assign to member definitions or member referances");
                }

                foreach (var memberDefinition in memberDefinitions)
                {
                    scope.TryAddStaticMember(memberDefinition);
                }

                foreach (var (left, memberReferance) in memberReferances)
                {
                    scope.TryAddStaticMember(new MemberDefinition(false, memberReferance.Key, new ImplicitTypeReferance(left)));
                }


                foreach (var type in types)
                {
                    scope.TryAddStaticType(type);
                }

                element = new ModuleDefinition(scope, assignOperations);

            }
            element = default;
            return false;
        }

        public static bool MatchMethodDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("method"), out var _)
                .Has(ElementMatcher.Generic2, out AtomicToken inputType,out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var methodScope = new MethodScope();

                var enclosingScope = new ScopeStack(matchingContext.EnclosingScope, methodScope);
                var elements = TokenParser.ParseBlock(body, StandMatchingContext(enclosingScope));

                var definitions = elements.DeepMemberDefinitions();

                foreach (var definition in definitions)
                {
                    methodScope.TryAddLocal(definition);
                }
                
                var parameterDefinition = new MemberDefinition(
                        false,
                        new ExplicitMemberName(parameterName?.Item ?? "input"),
                        new ExplicitTypeName(inputType.Item)
                        );


                methodScope.TryAddParameter(parameterDefinition);

                var referances = elements.DeepMemberReferances();

                foreach (var referance in referances)
                {
                    if (enclosingScope.GetMemberOrDefault(referance.Key) == null)
                    {
                        methodScope.TryAddLocal(new MemberDefinition(false,referance.Key,RootScope.AnyType));
                    }
                }

                element = new MethodDefinition(
                    new ExplicitTypeName(outputType.Item),
                    parameterDefinition,
                    elements,
                    methodScope,
                    new ICodeElement[0]);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchTypeDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element) {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("type"), out var _)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken typeName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .IsMatch) {

                ITypeSource name;
                if (typeName == default)
                {
                    name = new AnonymousName();
                }
                else {
                    name = new ExplicitTypeName(typeName.Item);
                }

                var scope = new ObjectScope();
                
                var elements = TokenParser.ParseBlock(body, StandMatchingContext(new ScopeStack(matchingContext.EnclosingScope, scope)));

                if (elements
                    .ExtractMemberDefinitions(out var memberDefinitions)
                    .ExtractMemberReferances(out var memberReferances)
                    .Any().Not()) {
                    throw new Exception("Types should only contain member definitions and member referances");
                }
                
                foreach (var memberDef in memberDefinitions)
                {
                    scope.TryAddLocalMember(memberDef);
                }

                foreach (var memberRef in memberReferances)
                {
                    scope.TryAddLocalMember(new MemberDefinition(false,memberRef.Item2.Key,RootScope.AnyType));
                }

                element = new TypeDefinition(name, scope);
                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchGenericTypeDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("type"), out var _)
                .Has(ElementMatcher.GenericN, out AtomicToken[] genericTypes)
                .Has(ElementMatcher.IsName, out AtomicToken typeName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .IsMatch)
            {

                var name = new ExplicitTypeName(typeName.Item);
                
                var scope = new ObjectScope();

                var elements = TokenParser.ParseBlock(body, StandMatchingContext(new ScopeStack(matchingContext.EnclosingScope, scope)));
                
                if (elements
                    .ExtractMemberDefinitions(out var memberDefinitions)
                    .ExtractMemberReferances(out var memberReferances)
                    .Any()
                    .Not())
                {
                    throw new Exception("Types should only contain member definitions and member referances");
                }
                
                foreach (var memberDef in memberDefinitions)
                {
                    scope.TryAddLocalMember(memberDef);
                }
                
                foreach (var memberRef in memberReferances)
                {
                    scope.TryAddLocalMember(new MemberDefinition(false, memberRef.Item2.Key, RootScope.AnyType));
                }

                var genericParameters = genericTypes.Select(x => new GenericTypeParameterDefinition(x.Item)).ToArray();
                
                element = new GenericTypeDefinition(name, scope, genericParameters);
                return true;
            }
            
            element = default;
            return false;
        }

        public static bool MatchImplementationDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.KeyWord("method"), out var _)
                .Has(ElementMatcher.Generic3, out AtomicToken contextType, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken contextName)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var methodScope = new MethodScope();

                var enclosingScope = new ScopeStack(matchingContext.EnclosingScope, methodScope);
                var elements = TokenParser.ParseBlock(body, StandMatchingContext(enclosingScope));

                var definitions = elements.DeepMemberDefinitions();

                foreach (var definition in definitions)
                {
                    methodScope.TryAddLocal(definition);
                }
                
                var contextDefinition = new MemberDefinition(
                        false,
                        new ExplicitMemberName(parameterName?.Item ?? "context"),
                        new ExplicitTypeName(contextType.Item)
                        );

                methodScope.TryAddParameter(contextDefinition);

                var parameterDefinition = new MemberDefinition(
                        false,
                        new ExplicitMemberName(parameterName?.Item ?? "input"),
                        new ExplicitTypeName(inputType.Item)
                        );


                methodScope.TryAddParameter(parameterDefinition);

                var referances = elements.DeepMemberReferances();

                foreach (var referance in referances)
                {
                    if (enclosingScope.GetMemberOrDefault(referance.Key) == null)
                    {
                        methodScope.TryAddLocal(new MemberDefinition(false, referance.Key, RootScope.AnyType));
                    }
                }

                element = new ImplementationDefinition(
                    contextDefinition,
                    new ExplicitTypeName(outputType.Item),
                    parameterDefinition,
                    elements,
                    methodScope,
                    new ICodeElement[0]);

                return true;
            }

            element = default;
            return false;
        }
        
        public static bool MatchBlockDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var scope = new LocalStaticScope();

                var enclosingScope = new ScopeStack(matchingContext.EnclosingScope, scope);
                var elements = TokenParser.ParseBlock(body, StandMatchingContext(enclosingScope));

                var definitions = elements.DeepMemberDefinitions();

                foreach (var definition in definitions)
                {
                    scope.TryAddLocal(definition);
                }

                var referances = elements.DeepMemberReferances();

                foreach (var referance in referances)
                {
                    if (enclosingScope.GetMemberOrDefault(referance.Key) == null)
                    {
                        scope.TryAddLocal(new MemberDefinition(false, referance.Key, RootScope.AnyType));
                    }
                }
                
                element = new BlockDefinition(
                    elements, scope, new ICodeElement[0]);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchConstantNumber(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
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

        public static bool MatchReferance(ElementToken elementToken, ElementMatchingContext matchingContext, out ICodeElement element)
        {
            if (ElementMatching.Start(elementToken)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                element = new ExplicitMemberName(first.Item);

                return true;
            }

            element = default;
            return false;
        }
    }

    public class ElementMatching
    {

        private ElementMatching(IEnumerable<IToken> tokens, bool isNotMatch)
        {
            this.IsNotMatch = isNotMatch;
            this.Tokens = tokens;
        }

        public bool IsMatch { get => !IsNotMatch; }
        public bool IsNotMatch { get; }
        public IEnumerable<IToken> Tokens { get; }

        public static ElementMatching Start(ElementToken elementToken)
        {
            return Match(elementToken.Tokens);
        }

        public static ElementMatching Match(IEnumerable<IToken> tokens)
        {
            return new ElementMatching(tokens, false);
        }

        public static ElementMatching NotMatch(IEnumerable<IToken> tokens)
        {
            return new ElementMatching(tokens, true);
        }

    }

    public static class ElementMatcher
    {
        public static ElementMatching Has<T1, T2, T3>(this ElementMatching self, IsMatch<T1, T2, T3> pattern, out T1 t1, out T2 t2, out T3 t3)
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

        public static ElementMatching Has<T1,T2>(this ElementMatching self, IsMatch<T1,T2> pattern, out T1 t1, out T2 t2)
        {
            if (self.IsNotMatch)
            {   
                t1 = default;
                t2 = default;
                return self;
            }

            return pattern(self, out t1, out t2);
        }

        public static ElementMatching Has<T>(this ElementMatching self, IsMatch<T> pattern, out T t)
        {
            if (self.IsNotMatch)
            {
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

        public static ElementMatching OptionalHas<T>(this ElementMatching self, IsMatch<T> pattern, out T t)
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

        public static ElementMatching OptionalHas(this ElementMatching self, IsMatch pattern)
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

        public delegate ElementMatching IsMatch(ElementMatching self);
        public delegate ElementMatching IsMatch<T>(ElementMatching self, out T matched);
        public delegate ElementMatching IsMatch<T1,T2>(ElementMatching self, out T1 matched1, out T2 matched2);
        public delegate ElementMatching IsMatch<T1, T2, T3>(ElementMatching self, out T1 matched1, out T2 matched2, out T3 matched3);

        public static ElementMatching IsName(ElementMatching self, out AtomicToken atomicToken)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                atomicToken = first;
                return ElementMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            atomicToken = default;
            return ElementMatching.NotMatch(self.Tokens);
        }
        
        public static ElementMatching IsType(ElementMatching self, out ITypeSource typeSource)
        {
            
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                var at = ElementMatching.Match(self.Tokens.Skip(1));
                if (GenericN(at,out var generics).IsMatch){
                    typeSource = new GenericExplicitTypeName(first.Item, generics);
                    return ElementMatching.Match(self.Tokens.Skip(2).ToArray());
                }

                typeSource = new ExplicitTypeName(first.Item);
                return ElementMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            typeSource = default;
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

        public static ElementMatching IsDone(ElementMatching self)
        {
            if (!self.Tokens.Any())
            {
                return self;
            }

            return ElementMatching.NotMatch(self.Tokens);
        }

        public static ElementMatching IsBody(ElementMatching self, out CurleyBacketToken body)
        {

            if (self.Tokens.Any() &&
                self.Tokens.First() is CurleyBacketToken first)
            {
                body = first;
                return ElementMatching.Match(self.Tokens.Skip(1).ToArray());
            }

            body = default;
            return ElementMatching.NotMatch(self.Tokens);
        }
        
        public static ElementMatching Generic3(ElementMatching elementMatching, out AtomicToken type1, out AtomicToken type2, out AtomicToken type3)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is ParenthesisToken typeParameters &&
                    typeParameters.Tokens.Count() == 3 &&
                    typeParameters.Tokens.ElementAt(0) is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is AtomicToken firstType &&
                    typeParameters.Tokens.ElementAt(1) is LineToken secondLine &&
                        secondLine.Tokens.Count() == 1 &&
                        secondLine.Tokens.ElementAt(0) is AtomicToken SecondType &&
                    typeParameters.Tokens.ElementAt(2) is LineToken thridLine &&
                        thridLine.Tokens.Count() == 1 &&
                        thridLine.Tokens.ElementAt(0) is AtomicToken thridType)
            {
                type1 = firstType;
                type2 = SecondType;
                type3 = thridType;
                return ElementMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            type1 = default;
            type2 = default;
            type3 = default;
            return ElementMatching.NotMatch(elementMatching.Tokens);
        }
        
        public static ElementMatching GenericN(ElementMatching elementMatching, out AtomicToken[] tokens)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is ParenthesisToken typeParameters &&
                    typeParameters.Tokens.All(x => x is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is AtomicToken))
            {
                tokens = typeParameters.Tokens.Select(x => (x as LineToken).Tokens.First() as AtomicToken).ToArray();
                return ElementMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            tokens = default;
            return ElementMatching.NotMatch(elementMatching.Tokens);
        }

        public static ElementMatching GenericN(ElementMatching elementMatching, out ITypeSource[] typeSources)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is ParenthesisToken typeParameters &&
                    typeParameters.Tokens.All(x => x is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is AtomicToken))
            {
                types = typeParameters.Tokens.Select(x => (x as LineToken).Tokens.First() as AtomicToken).ToArray();
                return ElementMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            types = default;
            return ElementMatching.NotMatch(elementMatching.Tokens);
        }

        public static ElementMatching Generic2(ElementMatching elementMatching, out AtomicToken type1, out AtomicToken type2)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is ParenthesisToken typeParameters &&
                    typeParameters.Tokens.Count() == 2 &&
                    typeParameters.Tokens.ElementAt(0) is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is AtomicToken firstType &&
                    typeParameters.Tokens.ElementAt(1) is LineToken secondLine &&
                        secondLine.Tokens.Count() == 1 &&
                        secondLine.Tokens.ElementAt(0) is AtomicToken SecondType)
            {
                type1 = firstType;
                type2 = SecondType;
                return ElementMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            type1 = default;
            type2 = default;
            return ElementMatching.NotMatch(elementMatching.Tokens);
        }

        public static IsMatch<AtomicToken> KeyWord(string word)
        {
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

                return table[(first.IsNotMatch, second.IsNotMatch)]();
            };
        }

        public static IsMatch<T> Xor<T>(this IsMatch<T> self, IsMatch<T> other)
        {
            return Backing;

            ElementMatching Backing(ElementMatching element, out T t)
            {
                var first = self(element, out var t1);
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
                        return ElementMatching.NotMatch(element.Tokens);
                    }
                }
            };
        }

    }
    
}
