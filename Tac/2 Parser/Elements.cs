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
        TMemberDefinition MemberDefinition(bool readOnly, ExplicitMemberName explicitMemberName, ITypeDefinition explicitTypeName);
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

        public ElementMatchingContext Child(IScope scope)
        {
            if (scope is LocalStaticScope lss)
            {

                return new ElementMatchingContext(new ScopeStack(EnclosingScope, scope), ElementBuilder, NormalElementMatcher,
                    x =>
                    {
                        if (!lss.TryAddLocal(x))
                        {
                            throw new Exception();
                        }
                    },
                     x =>
                     {
                         if (!lss.TryAddStaticType(x)){
                             throw new Exception();
                         }
                     },
                    x =>
                    {
                        if (!lss.TryAddStaticGenericType(x))
                        {
                            throw new Exception();
                        };
                    });
            }

            if (scope is StaticScope ss)
            {

                return new ElementMatchingContext(new ScopeStack(EnclosingScope, scope), ElementBuilder, ElementMatchers,
                    x =>
                    {
                            throw new Exception();
                    },
                     x =>
                     {
                         if (!ss.TryAddStaticType(x)){
                             throw new Exception();
                         }
                     },
                    x =>
                    {
                        if (!ss.TryAddStaticGenericType(x))
                        {
                            throw new Exception();
                        };
                    });
            }

            throw new Exception();
        }
        
        private ElementMatchingContext VarMatcher(ITypeDefinition typeDefinition) => new ElementMatchingContext(EnclosingScope, ElementBuilder, VarElementMatcher(typeDefinition),AddMember,AddType,AddGenerticType);

        public ElementMatchingContext(
            ScopeStack enclosingScope,
            IElementBuilder<MemberDefinition, ExplicitMemberName,
ExplicitTypeName, GenericExplicitTypeName, ImplicitTypeReferance, ObjectDefinition, ModuleDefinition, MethodDefinition, NamedTypeDefinition,
TypeDefinition, GenericTypeDefinition, ImplementationDefinition, BlockDefinition, ConstantNumber, AddOperation, SubtractOperation, MultiplyOperation, IfTrueOperation, ElseOperation, LessThanOperation, NextCallOperation, LastCallOperation, AssignOperation, ReturnOperation, object> elementBuilder,
            IEnumerable<TryMatch> elementMatchers,
            Action<MemberDefinition> addMember,
            Action<NamedTypeDefinition> addType,
            Action<GenericTypeDefinition> addGenerticType)
        {
            EnclosingScope = enclosingScope ?? throw new ArgumentNullException(nameof(enclosingScope));
            ElementBuilder = elementBuilder ?? throw new ArgumentNullException(nameof(elementBuilder));
            ElementMatchers = elementMatchers ?? throw new ArgumentNullException(nameof(elementMatchers));
            AddMember = addMember ?? throw new ArgumentNullException(nameof(addMember));
            AddType = addType ?? throw new ArgumentNullException(nameof(addType));
            AddGenerticType = addGenerticType ?? throw new ArgumentNullException(nameof(addGenerticType));
        }

        public IElementBuilder<MemberDefinition, ExplicitMemberName,
ExplicitTypeName, GenericExplicitTypeName, ImplicitTypeReferance, ObjectDefinition, ModuleDefinition, MethodDefinition, NamedTypeDefinition,
TypeDefinition, GenericTypeDefinition, ImplementationDefinition, BlockDefinition, ConstantNumber, AddOperation, SubtractOperation, MultiplyOperation, IfTrueOperation, ElseOperation, LessThanOperation, NextCallOperation, LastCallOperation, AssignOperation, ReturnOperation, object> ElementBuilder
        { get; }
        public ScopeStack EnclosingScope { get; }

        public IEnumerable<TryMatch> ElementMatchers { get; }

        private Action<MemberDefinition> AddMember { get; }
        private Action<NamedTypeDefinition> AddType { get; }
        private Action<GenericTypeDefinition> AddGenerticType { get; }

        public static IEnumerable<TryMatch> NormalElementMatcher { get; } = new List<TryMatch> {
                    MatchObjectDefinition,
                    MatchGenericTypeDefinition,
                    MatchMemberDefinition,
                    MatchTypeDefinition,
                    MatchMethodDefinition,
                    MatchImplementationDefinition,
                    MatchBlockDefinition,
                    MatchConstantNumber,
                    MatchReferance
                };

        public static IEnumerable<TryMatch> VarElementMatcher (ITypeDefinition typeDefinition) => new List<TryMatch> {
                    MatchObjectDefinition,
                    MatchGenericTypeDefinition,
                    MatchMemberDefinition,
                    MatchTypeDefinition,
                    MatchLocalDefinition_Var(typeDefinition),
                    MatchMethodDefinition,
                    MatchImplementationDefinition,
                    MatchBlockDefinition,
                    MatchConstantNumber,
                    MatchReferance
                };

        public IEnumerable<OperationMatcher> OperationMatchers { get; } = new List<OperationMatcher>
        {
            MatchBinary("+",x=> (object y,object z) => x.ElementBuilder.AddOperation(y.Cast<ICodeElement>(),z.Cast<ICodeElement>())),
            MatchBinary("=:",x=> (object y,object z) => x.ElementBuilder.AddOperation(y.Cast<ICodeElement>(),z.Cast<ICodeElement>()))
        };


        #region Parse

        private object ParseParenthesisOrElement(IToken token)
        {
            if (token is ElementToken elementToken)
            {
                // smells 
                if (elementToken.Tokens.Count() == 1 && elementToken.Tokens.First() is ParenthesisToken parenthesisToken)
                {
                    return ParseLine(parenthesisToken.Tokens);
                }

                foreach (var tryMatch in ElementMatchers)
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

        private ICodeElement ParseLine(IEnumerable<IToken> tokens)
        {
            foreach (var operationMatcher in OperationMatchers)
            {
                if (operationMatcher(tokens, this, out var obj))
                {
                    return obj;
                }
            }
            throw new Exception("");
        }

        public ICodeElement[] ParseFile(FileToken file)
        {
            return file.Tokens.Select(x => ParseLine(x.Cast<LineToken>().Tokens)).ToArray();
        }

        public ICodeElement[] ParseBlock(CurleyBacketToken block)
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
        
        public delegate bool TryMatch(ElementToken elementToken, ElementMatchingContext matchingContext, out object element);

        public static TryMatch MatchLocalDefinition_Var(ITypeDefinition typeDefinition) => (ElementToken elementToken, ElementMatchingContext matchingContext, out object element) =>
         {
             if (TokenMatching.Start(elementToken.Tokens)
                 .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                 .Has(ElementMatcher.KeyWord("var"), out var _)
                 .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                 .Has(ElementMatcher.IsDone)
                 .IsMatch)
             {

                 var readOnly = readonlyToken != default;

                 var memberDefinition = matchingContext.ElementBuilder.MemberDefinition(readOnly, new ExplicitMemberName(nameToken.Item), typeDefinition);

                 matchingContext.AddMember(memberDefinition);

                 element = memberDefinition;

                 return true;
             }

             element = default;
             return false;
         };

        public static bool MatchMemberDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsType, out ITypeSource typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                var readOnly = readonlyToken != default;

                var memberDefinition = matchingContext.ElementBuilder.MemberDefinition(
                    readOnly,
                    matchingContext.ElementBuilder.ExplicitMemberName(nameToken.Item),
                    typeToken.GetTypeDefinition(matchingContext.EnclosingScope));

                matchingContext.AddMember(memberDefinition);

                element = memberDefinition;

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

                var memberDefinition = matchingContext.ElementBuilder.MemberDefinition(
                    readOnly,
                    matchingContext.ElementBuilder.ExplicitMemberName(nameToken.Item),
                    matchingContext.ElementBuilder.GenericExplicitTypeName(typeToken.Item, tokenSources).GetTypeDefinition(matchingContext.EnclosingScope));

                matchingContext.AddMember(memberDefinition);

                element = memberDefinition;

                return true;
            }

            element = default;
            return false;
        }

        public static bool MatchObjectDefinition(ElementToken elementToken, ElementMatchingContext matchingContext, out object element)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("object"), out var keyword)
                .Has(ElementMatcher.IsBody, out CurleyBacketToken block)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                var scope = new ObjectScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(block);

                if (elements.ExtractTopLevelAssignOperations(out var assignOperations).Any())
                {
                    throw new Exception("objects should only contain assign operations");
                }

                if (assignOperations
                    .ExtractMemberDefinitions(out var _)
                    .ExtractMemberReferances(out var _)
                    .Any())
                {
                    throw new Exception("objects should only assign to member definitions or member referances");
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

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(third);

                if (elements
                    .ExtractTopLevelAssignOperations(out var assignOperations)
                    .ExtractTopLevelTypeDefinitions(out var types)
                    .Any())
                {
                    throw new Exception("objects should only contain assign operations");
                }

                if (assignOperations
                    .ExtractMemberDefinitions(out var _)
                    .ExtractMemberReferances(out var _)
                    .Any())
                {
                    throw new Exception("objects should only assign to member definitions or member referances");
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

                var parameterDefinition = matchingContext.ElementBuilder.MemberDefinition(
                        false,
                        matchingContext.ElementBuilder.ExplicitMemberName(parameterName?.Item ?? "input"),
                        matchingContext.ElementBuilder.ExplicitTypeName(inputType.Item).GetTypeDefinition(matchingContext.EnclosingScope)
                        );


                methodScope.TryAddParameter(parameterDefinition);

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

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(body);

                if (elements
                    .ExtractMemberDefinitions(out var _)
                    .ExtractMemberReferances(out var _)
                    .Any().Not())
                {
                    throw new Exception("Types should only contain member definitions and member referances");
                }

                if (typeName == default)
                {
                    element = matchingContext.ElementBuilder.TypeDefinition(scope);
                }
                else
                {
                    var namedType = matchingContext.ElementBuilder.NamedTypeDefinition(
                        new NameKey(typeName.Item),
                        scope);

                    matchingContext.AddType(namedType);

                    element = namedType;
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

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(body);

                if (elements
                    .ExtractMemberDefinitions(out var _)
                    .ExtractMemberReferances(out var _)
                    .Any()
                    .Not())
                {
                    throw new Exception("Types should only contain member definitions and member referances");
                }

                var genericParameters = genericTypes.Select(x => new GenericTypeParameterDefinition(x.Item)).ToArray();

                var genericTypeDefinition = matchingContext.ElementBuilder.GenericTypeDefinition(new NameKey(typeName.Item), scope, genericParameters);

                matchingContext.AddGenerticType(genericTypeDefinition);

                element = genericTypeDefinition;

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

                var contextDefinition = matchingContext.ElementBuilder.MemberDefinition(
                        false,
                        matchingContext.ElementBuilder.ExplicitMemberName(parameterName?.Item ?? "context"),
                        matchingContext.ElementBuilder.ExplicitTypeName(contextType.Item).GetTypeDefinition(matchingContext.EnclosingScope)
                        );

                methodScope.TryAddParameter(contextDefinition);

                var parameterDefinition = matchingContext.ElementBuilder.MemberDefinition(
                        false,
                        matchingContext.ElementBuilder.ExplicitMemberName(parameterName?.Item ?? "input"),
                        matchingContext.ElementBuilder.ExplicitTypeName(inputType.Item).GetTypeDefinition(matchingContext.EnclosingScope)
                        );


                methodScope.TryAddParameter(parameterDefinition);

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


                var refrence = matchingContext.ElementBuilder.ExplicitMemberName(first.Item);

                if (matchingContext.EnclosingScope.GetMemberOrDefault(refrence) == default)
                {

                    var memberDefinition = matchingContext.ElementBuilder.MemberDefinition(
                                    false,
                                    refrence,
                                    RootScope.AnyType.GetTypeDefinition(matchingContext.EnclosingScope));


                    matchingContext.AddMember(memberDefinition);

                    element = memberDefinition;
                }

                element = refrence;

                return true;
            }

            element = default;
            return false;
        }

        public delegate bool OperationMatcher(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out ICodeElement result);

        public static OperationMatcher MatchBinary(string name, Func<ElementMatchingContext, Func<object, object, ICodeElement>> builder) => (IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out ICodeElement result) =>
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

        public static bool MatchAssign(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out ICodeElement result)
        {
            if (TokenMatching.Start(tokens)
                .Has(ElementMatcher.IsBinaryOperation("=:"), out var perface, out var token, out var rhs)
                .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                
                var varMachtingContext = matchingContext.VarMatcher(left.ReturnType(matchingContext.EnclosingScope));
                
                var right = matchingContext.ParseParenthesisOrElement(rhs);
                
                result = matchingContext.ElementBuilder.AssignOperation(left, right.Cast<MemberDefinition>());
                return true;
            }

            result = default;
            return false;
        }

        public static OperationMatcher MatchTrailing(string name, Func<ElementMatchingContext, Func<object, ICodeElement>> builder) => (IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out ICodeElement result) =>
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

        public static IsMatch<IEnumerable<IToken>, AtomicToken, IToken> IsBinaryOperation(string s) => (TokenMatching elementMatching, out IEnumerable<IToken> preface, out AtomicToken operation, out IToken rhs) =>

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

        public static IsMatch<IEnumerable<IToken>, AtomicToken> IsTrailingOperation(string s) => (TokenMatching elementMatching, out IEnumerable<IToken> preface, out AtomicToken operation) =>
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
