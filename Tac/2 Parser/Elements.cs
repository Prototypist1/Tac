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
    public interface IElementBuilder<
        out TMemberDefinition,
        out TExplicitMemberName,
        out TExplicitTypeName,
        out TGenericExplicitTypeName,
        out TImplicitTypeReferance,
        out TObjectDefinition,
        out TModuleDefinition,
        out TMethodDefinition,
        out TNamedTypeDefinition,
        out TTypeDefinition,
        out TGenericTypeDefinition,
        out TImplementationDefinition,
        out TBlockDefinition,
        out TConstantNumber,
        out TAddOperation,
        out TSubtractOperation,
        out TMultiplyOperation,
        out TIfTrueOperation,
        out TElseOperation,
        out TLessThanOperation,
        out TNextCallOperation,
        out TLastCallOperation,
        out TAssignOperation,
        out TReturnOperation,
        T
        >
        where TMemberDefinition : MemberDefinition, T
        where TExplicitMemberName : ExplicitMemberName, T
        where TExplicitTypeName : ExplicitTypeName, T
        where TGenericExplicitTypeName : GenericExplicitTypeName, T
        where TImplicitTypeReferance : ImplicitTypeReferance, T
        where TObjectDefinition : ObjectDefinition, T
        where TModuleDefinition : ModuleDefinition, T
        where TMethodDefinition : MethodDefinition, T
        where TNamedTypeDefinition : NamedTypeDefinition, T
        where TTypeDefinition : TypeDefinition, T
        where TGenericTypeDefinition : GenericTypeDefinition, T
        where TImplementationDefinition : ImplementationDefinition, T
        where TBlockDefinition : BlockDefinition, T
        where TConstantNumber : ConstantNumber, T
        where TAddOperation : AddOperation, T
        where TSubtractOperation : SubtractOperation, T
        where TMultiplyOperation : MultiplyOperation, T
        where TIfTrueOperation : IfTrueOperation, T
        where TElseOperation : ElseOperation, T
        where TLessThanOperation : LessThanOperation, T
        where TNextCallOperation : NextCallOperation, T
        where TLastCallOperation : LastCallOperation, T
        where TAssignOperation : AssignOperation, T
        where TReturnOperation : ReturnOperation, T
    {
        TMemberDefinition MemberDefinition(bool readOnly, ExplicitMemberName explicitMemberName, ITypeSource explicitTypeName);
        TAddOperation AddOperation(ICodeElement codeElement1, ICodeElement codeElement2);
        TSubtractOperation SubtractOperation(ICodeElement codeElement1, ICodeElement codeElement2);
        TExplicitMemberName ExplicitMemberName(string item);
        TMultiplyOperation MultiplyOperation(ICodeElement codeElement1, ICodeElement codeElement2);
        TExplicitTypeName ExplicitTypeName(string item);
        TIfTrueOperation IfTrueOperation(ICodeElement codeElement1, ICodeElement codeElement2);
        TElseOperation ElseOperation(ICodeElement codeElement1, ICodeElement codeElement2);
        TGenericExplicitTypeName GenericExplicitTypeName(string item, ITypeSource[] tokenSources);
        TLessThanOperation LessThanOperation(ICodeElement codeElement1, ICodeElement codeElement2);
        TNextCallOperation NextCallOperation(ICodeElement codeElement1, ICodeElement codeElement2);
        TImplicitTypeReferance ImplicitTypeReferance(ICodeElement left);
        TObjectDefinition ObjectDefinition(ObjectScope scope, IReadOnlyList<AssignOperation> assignOperations);
        TModuleDefinition ModuleDefinition(StaticScope scope, IReadOnlyList<AssignOperation> assignOperations);
        TMethodDefinition MethodDefinition(ExplicitTypeName explicitTypeName, MemberDefinition parameterDefinition, ICodeElement[] elements, MethodScope methodScope, ICodeElement[] codeElement);
        TAssignOperation AssignOperation(ICodeElement codeElement, IMemberSource memberSource);
        TTypeDefinition TypeDefinition(ObjectScope scope);
        TNamedTypeDefinition NamedTypeDefinition(NameKey nameKey, ObjectScope scope);
        TGenericTypeDefinition GenericTypeDefinition(NameKey nameKey, ObjectScope scope, GenericTypeParameterDefinition[] genericParameters);
        TReturnOperation ReturnOperation(ICodeElement codeElement);
        TImplementationDefinition ImplementationDefinition(MemberDefinition contextDefinition, ExplicitTypeName explicitTypeName, MemberDefinition parameterDefinition, ICodeElement[] elements, MethodScope methodScope, ICodeElement[] codeElement);
        TBlockDefinition BlockDefinition(ICodeElement[] elements, LocalStaticScope scope, ICodeElement[] codeElement);
        TConstantNumber ConstantNumber(double dub);
    }

    public class ElementMatchingContext
    {

        public ElementMatchingContext Child(IScope scope) => new ElementMatchingContext(new ScopeStack(EnclosingScope, scope), ElementBuilder);

        private object ParseParenthesisOrElement(IToken token) {
            if (token is ElementToken elementToken)
            {
                // smells 
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken)
                {
                    return ParseLine(parenthesisToken.Tokens);
                }

                foreach (var tryMatch in ElementMatchingContext.ElementMatchers)
                {
                    if (tryMatch(elementToken, this, out var obj))
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

        private object ParseLine(IEnumerable<IToken> tokens) {
            foreach (var operationMatcher in OperationMatchers)
            {
                if (operationMatcher(tokens, this, out var obj))
                {
                    return obj;
                }
            }
            throw new Exception("");
        }

        public object[] ParseFile(FileToken file)
        {
            return file.Tokens.Select(x => ParseLine(x.Cast<LineToken>().Tokens)).ToArray();
        }

        public object[] ParseBlock(CurleyBacketToken block)
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


        public ElementMatchingContext(ScopeStack enclosingScope, IElementBuilder<MemberDefinition, ExplicitMemberName,
ExplicitTypeName, GenericExplicitTypeName, ImplicitTypeReferance, ObjectDefinition, ModuleDefinition, MethodDefinition, NamedTypeDefinition,
TypeDefinition, GenericTypeDefinition, ImplementationDefinition, BlockDefinition, ConstantNumber, AddOperation, SubtractOperation, MultiplyOperation, IfTrueOperation, ElseOperation, LessThanOperation, NextCallOperation, LastCallOperation, AssignOperation, ReturnOperation, object> elementBuilder)
        {
            EnclosingScope = enclosingScope ?? throw new ArgumentNullException(nameof(enclosingScope));
            ElementBuilder = elementBuilder ?? throw new ArgumentNullException(nameof(elementBuilder));
        }

        public IElementBuilder<MemberDefinition, ExplicitMemberName,
ExplicitTypeName, GenericExplicitTypeName, ImplicitTypeReferance, ObjectDefinition, ModuleDefinition, MethodDefinition, NamedTypeDefinition,
TypeDefinition, GenericTypeDefinition, ImplementationDefinition, BlockDefinition, ConstantNumber, AddOperation, SubtractOperation, MultiplyOperation, IfTrueOperation, ElseOperation, LessThanOperation, NextCallOperation, LastCallOperation, AssignOperation, ReturnOperation, object> ElementBuilder
        { get; }
        public ScopeStack EnclosingScope { get; }
        
        public static IEnumerable<TryMatch> ElementMatchers { get; } = new List<TryMatch> {
                    MatchObjectDefinition,
                    MatchLocalDefinition_Var,
                    MatchGenericTypeDefinition,
                    MatchTypeDefinition,
                    MatchMethodDefinition,
                    MatchImplementationDefinition,
                    MatchBlockDefinition,
                    MatchConstantNumber,
                    MatchReferance
                };

        public IEnumerable<OperationMatcher> OperationMatchers { get; } = new List<OperationMatcher>
        {
            MatchBinary("+",x=> (object y,object z) => x.ElementBuilder.AddOperation(y.Cast<ICodeElement>(),z.Cast<ICodeElement>()).Cast<object>())
        };

        public delegate bool TryMatch(ElementToken elementToken, ElementMatchingContext matchingContext, out object element);

        public static bool MatchLocalDefinition_Var(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            // if I do a good job on ElementMatching it can pick up var
            // all the definitions matchers can probably rolled in togther

            // when they day comes I should make the type entry optional

            // "readonly x" is a totally legitiment method definition

            // while I am thinking about including updates

            // there are a lot of things here that use () that should use []

            if (TokenMatching.Start(elementToken.Tokens)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var readOnly = readonlyToken != default;

                element = new ImplicitlyTypedMemberDefinition(readOnly, new ExplicitMemberName(nameToken.Item));

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchMemberDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            // TODO use ElementMatcher.IsType

            if (TokenMatching.Start(elementToken.Tokens)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsName, out AtomicToken typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var readOnly = readonlyToken != default;

                element = matchingContext.ElementBuilder.MemberDefinition(
                    readOnly,
                    matchingContext.ElementBuilder.ExplicitMemberName(nameToken.Item),
                    matchingContext.ElementBuilder.ExplicitTypeName(typeToken.Item));

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchGenericMemberDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            // TODO use ElementMatcher.IsType

            if (TokenMatching.Start(elementToken.Tokens)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsName, out AtomicToken typeToken)
                .Has(ElementMatcher.GenericN, out ITypeSource[] tokenSources)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var readOnly = readonlyToken != default;

                element = matchingContext.ElementBuilder.MemberDefinition(
                    readOnly,
                    matchingContext.ElementBuilder.ExplicitMemberName(nameToken.Item),
                    matchingContext.ElementBuilder.GenericExplicitTypeName(typeToken.Item, tokenSources));

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

        public static bool MatchObjectDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("object"), out var keyword)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken block)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var scope = new ObjectScope();

                var elements = matchingContext.Child(scope).ParseBlock(block);

                if (elements.ExtractTopLevelAssignOperations(out var assignOperations).Any())
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
                    scope.TryAddLocalMember(memberDefinition);
                }

                foreach (var (left, memberReferance) in memberReferances)
                {
                    scope.TryAddLocalMember(
                        matchingContext.ElementBuilder.MemberDefinition(
                            false,
                            memberReferance,
                            matchingContext.ElementBuilder.ImplicitTypeReferance(left)));
                }

                element = matchingContext.ElementBuilder.ObjectDefinition(scope, assignOperations);

                return true;
            }
            element = default;
            return false;
        }

        public static bool MatchModuleDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("module"), out var frist)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken third)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var scope = new StaticScope();

                var elements = matchingContext.Child(scope).ParseBlock(third);

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
                    scope.TryAddStaticMember(matchingContext.ElementBuilder.MemberDefinition(false, memberReferance, matchingContext.ElementBuilder.ImplicitTypeReferance(left)));
                }


                foreach (var type in types.OfType<NamedTypeDefinition>())
                {
                    scope.TryAddStaticType(type);
                }

                element = matchingContext.ElementBuilder.ModuleDefinition(scope, assignOperations);

            }
            element = default;
            return false;
        }

        public static bool MatchMethodDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("method"), out var _)
                .Has(ElementMatcher.Generic2, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var methodScope = new MethodScope();

                var innerMatchingScope = matchingContext.Child(methodScope);
                var elements = innerMatchingScope.ParseBlock(body);

                var definitions = elements.DeepMemberDefinitions();

                foreach (var definition in definitions)
                {
                    methodScope.TryAddLocal(definition);
                }

                var parameterDefinition = matchingContext.ElementBuilder.MemberDefinition(
                        false,
                        matchingContext.ElementBuilder.ExplicitMemberName(parameterName?.Item ?? "input"),
                        matchingContext.ElementBuilder.ExplicitTypeName(inputType.Item)
                        );


                methodScope.TryAddParameter(parameterDefinition);

                var referances = elements.DeepMemberReferances();

                foreach (var referance in referances)
                {
                    if (innerMatchingScope.EnclosingScope.GetMemberOrDefault(referance) == null)
                    {
                        methodScope.TryAddLocal(matchingContext.ElementBuilder.MemberDefinition(
                            false,
                            referance,
                            RootScope.AnyType));
                    }
                }

                element = matchingContext.ElementBuilder.MethodDefinition(
                    matchingContext.ElementBuilder.ExplicitTypeName(outputType.Item),
                    parameterDefinition,
                    elements,
                    methodScope,
                    new ICodeElement[0]);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchTypeDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("type"), out var _)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken typeName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .IsMatch)
            {



                var scope = new ObjectScope();

                var elements = matchingContext.Child(methodScope).ParseBlock(body);

                if (elements
                    .ExtractMemberDefinitions(out var memberDefinitions)
                    .ExtractMemberReferances(out var memberReferances)
                    .Any().Not())
                {
                    throw new Exception("Types should only contain member definitions and member referances");
                }

                foreach (var memberDef in memberDefinitions)
                {
                    scope.TryAddLocalMember(memberDef);
                }

                foreach (var memberRef in memberReferances)
                {
                    scope.TryAddLocalMember(new MemberDefinition(false, memberRef.Item2, RootScope.AnyType));
                }

                if (typeName == default)
                {
                    element = matchingContext.ElementBuilder.TypeDefinition(scope);
                }
                else
                {
                    element = matchingContext.ElementBuilder.NamedTypeDefinition(
                        new NameKey(typeName.Item),
                        scope);
                }
                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchGenericTypeDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("type"), out var _)
                .Has(ElementMatcher.DefineGenericN, out AtomicToken[] genericTypes)
                .Has(ElementMatcher.IsName, out AtomicToken typeName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .IsMatch)
            {

                var scope = new ObjectScope();

                var elements = matchingContext.Child(scope).ParseBlock(body);

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
                    scope.TryAddLocalMember(
                        matchingContext.ElementBuilder.MemberDefinition(
                            false,
                            memberRef.Item2,
                            RootScope.AnyType));
                }

                var genericParameters = genericTypes.Select(x => new GenericTypeParameterDefinition(x.Item)).ToArray();

                element = matchingContext.ElementBuilder.GenericTypeDefinition(new NameKey(typeName.Item), scope, genericParameters);
                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchImplementationDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("method"), out var _)
                .Has(ElementMatcher.Generic3, out AtomicToken contextType, out AtomicToken inputType, out AtomicToken outputType)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken contextName)
                .OptionalHas(ElementMatcher.IsName, out AtomicToken parameterName)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var methodScope = new MethodScope();

                var newMatchingContext = matchingContext.Child(methodScope);
                var elements = newMatchingContext.ParseBlock(body);

                var definitions = elements.DeepMemberDefinitions();

                foreach (var definition in definitions)
                {
                    methodScope.TryAddLocal(definition);
                }

                var contextDefinition = matchingContext.ElementBuilder.MemberDefinition(
                        false,
                        matchingContext.ElementBuilder.ExplicitMemberName(parameterName?.Item ?? "context"),
                        matchingContext.ElementBuilder.ExplicitTypeName(contextType.Item)
                        );

                methodScope.TryAddParameter(contextDefinition);

                var parameterDefinition = matchingContext.ElementBuilder.MemberDefinition(
                        false,
                        matchingContext.ElementBuilder.ExplicitMemberName(parameterName?.Item ?? "input"),
                        matchingContext.ElementBuilder.ExplicitTypeName(inputType.Item)
                        );


                methodScope.TryAddParameter(parameterDefinition);

                var referances = elements.DeepMemberReferances();

                foreach (var referance in referances)
                {
                    if (newMatchingContext.EnclosingScope.GetMemberOrDefault(referance) == null)
                    {
                        methodScope.TryAddLocal(
                            matchingContext.ElementBuilder.MemberDefinition(
                                false,
                                referance,
                                RootScope.AnyType));
                    }
                }

                element = matchingContext.ElementBuilder.ImplementationDefinition(
                    contextDefinition,
                    matchingContext.ElementBuilder.ExplicitTypeName(outputType.Item),
                    parameterDefinition,
                    elements,
                    methodScope,
                    new ICodeElement[0]);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchBlockDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var scope = new LocalStaticScope();

                var innerMatchingContext = matchingContext.Child(scope);
                var elements = innerMatchingContext.ParseBlock(body);

                var definitions = elements.DeepMemberDefinitions();

                foreach (var definition in definitions)
                {
                    scope.TryAddLocal(definition);
                }

                var referances = elements.DeepMemberReferances();

                foreach (var referance in referances)
                {
                    if (innerMatchingContext.EnclosingScope.GetMemberOrDefault(referance) == null)
                    {
                        scope.TryAddLocal(matchingContext.ElementBuilder.MemberDefinition(false, referance, RootScope.AnyType));
                    }
                }

                element = matchingContext.ElementBuilder.BlockDefinition(
                    elements, scope, new ICodeElement[0]);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchConstantNumber(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsNumber, out double dub)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                element = matchingContext.ElementBuilder.ConstantNumber(dub);

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchReferance(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                element = matchingContext.ElementBuilder.ExplicitMemberName(first.Item);

                return true;
            }

            element = default;
            return false;
        }

        public delegate bool OperationMatcher(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out object result);

        public static OperationMatcher MatchBinary(string name, Func<ElementMatchingContext,Func<object, object, object>> builder) => (IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out object result) =>
            {
                if (TokenMatching.Start(tokens)
                    .Has(ElementMatcher.IsBinaryOperation(name), out var perface, out var token, out var rhs)
                    .IsMatch)
                {
                    result = builder(matchingContext)(matchingContext.ParseLine(perface), matchingContext.ParseParenthesisOrElement(rhs));
                    return true;
                }

                result = default;
                return false;
            };

        public static OperationMatcher MatchTrailing(string name, Func<ElementMatchingContext, Func<object, object>> builder) => (IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out object result) =>
        {
            if (TokenMatching.Start(tokens)
                .Has(ElementMatcher.IsTrailingOperation(name), out var perface, out var token)
                .IsMatch)
            {
                result = builder(matchingContext)(matchingContext.ParseLine(perface));
                return true;
            }

            result = default;
            return false;
        };

    }

    public class TokenMatching
    {

        private TokenMatching(IEnumerable<IToken> tokens, bool isNotMatch)
        {
            this.IsNotMatch = isNotMatch;
            this.Tokens = tokens;
        }

        public bool IsMatch { get => !IsNotMatch; }
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

        public static TokenMatching IsType(TokenMatching self, out ITypeSource typeSource)
        {

            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                var at = TokenMatching.Match(self.Tokens.Skip(1));
                if (GenericN(at, out ITypeSource[] generics).IsMatch)
                {
                    typeSource = new GenericExplicitTypeName(first.Item, generics);
                    return TokenMatching.Match(self.Tokens.Skip(2).ToArray());
                }

                typeSource = new ExplicitTypeName(first.Item);
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
                elementMatching.Tokens.First() is ParenthesisToken typeParameters &&
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

        public static TokenMatching GenericN(TokenMatching elementMatching, out ITypeSource[] typeSources)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is ParenthesisToken typeParameters &&
                typeParameters.Tokens.All(x => x is ElementToken) &&
                TryToToken(out var res))
            {
                typeSources = res;
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            typeSources = default;
            return TokenMatching.NotMatch(elementMatching.Tokens);

            bool TryToToken(out ITypeSource[] typeSourcesInner)
            {
                var typeSourcesBuilding = new List<ITypeSource>();
                foreach (var elementToken in typeParameters.Tokens.OfType<ElementToken>())
                {
                    var matcher = TokenMatching.Start(elementToken.Tokens);
                    if (matcher.Has(ElementMatcher.IsType, out ITypeSource typeSource).Has(IsDone).IsMatch)
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
                return TokenMatching.Match(elementMatching.Tokens.Skip(1).ToArray());
            }

            type1 = default;
            type2 = default;
            return TokenMatching.NotMatch(elementMatching.Tokens);
        }

        public static IsMatch<IEnumerable<IToken>, AtomicToken, IToken> IsBinaryOperation(string s) => (TokenMatching elementMatching, out IEnumerable<IToken> preface,  out AtomicToken operation, out IToken rhs) =>

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

        public static IsMatch<IEnumerable<IToken>, AtomicToken> IsTrailingOperation(string s) => (TokenMatching elementMatching, out IEnumerable<IToken> preface, out AtomicToken operation)=>
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
