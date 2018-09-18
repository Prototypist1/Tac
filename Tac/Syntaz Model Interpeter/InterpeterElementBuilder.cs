using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedContext {

        private InterpetedContext(IInterpetedScope[] scopes)
        {
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }

        public IReadOnlyList<IInterpetedScope> Scopes { get; }

        public InterpetedContext Child(IInterpetedScope scope) {
            var scopes = new List<IInterpetedScope> { scope };
            scopes.AddRange(Scopes);
            return new InterpetedContext(scopes.ToArray());
        }

        public static InterpetedContext Root()
        {
            return new InterpetedContext(new IInterpetedScope[0]);
        }
    }

    public class InterpetedResult
    {
        public InterpetedResult(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            HasValue = true;
        }
        public InterpetedResult()
        {
            HasValue = false;
        }

        public bool HasValue { get; }
        private object Value { get; }

        public T Get<T>()
        {
            if (HasValue)
            {
                throw new Exception($"{nameof(InterpetedResult)} does not have a value");
            }
            return (T)Value;
        }

        public object Get()
        {
            if (HasValue)
            {
                throw new Exception($"{nameof(InterpetedResult)} does not have a value");
            }
            return Value;
        }

    }

    public interface IInterpeted
    {
        InterpetedResult Interpet(InterpetedContext interpetedContext);
    }

    internal class InterpeterElementBuilder : IElementBuilder<
        InterpetedMemberDefinition,
        InterpetedExplicitMemberName,
        InterpetedExplicitTypeName,
        InterpetedGenericExplicitTypeName,
        InterpetedObjectDefinition,
        InterpetedModuleDefinition,
        InterpetedMethodDefinition,
        InterpetedNamedTypeDefinition,
        InterpetedTypeDefinition,
        InterpetedGenericTypeDefinition,
        InterpetedImplementationDefinition,
        InterpetedBlockDefinition,
        InterpetedConstantNumber,
        InterpetedAddOperation,
        InterpetedSubtractOperation,
        InterpetedMultiplyOperation,
        InterpetedIfTrueOperation,
        InterpetedElseOperation,
        InterpetedLessThanOperation,
        InterpetedNextCallOperation,
        InterpetedLastCallOperation,
        InterpetedAssignOperation,
        InterpetedReturnOperation,
        InterpetedPathOperation,
        IInterpeted
        >
    {
        public InterpetedAddOperation AddOperation(ICodeElement codeElement1, ICodeElement codeElement2)
        {
            return new InterpetedAddOperation(codeElement1, codeElement2);
        }

        public InterpetedAssignOperation AssignOperation(ICodeElement codeElement, IMemberSource memberSource)
        {
            return new InterpetedAssignOperation(codeElement, memberSource);
        }

        public InterpetedAssignOperation AssignOperation(ICodeElement codeElement, ICodeElement target)
        {
            throw new NotImplementedException();
        }

        public InterpetedBlockDefinition BlockDefinition(ICodeElement[] elements, LocalStaticScope scope, ICodeElement[] codeElement)
        {
            return new InterpetedBlockDefinition(elements, scope, codeElement);
        }

        public InterpetedConstantNumber ConstantNumber(double dub)
        {
            return new InterpetedConstantNumber(dub);
        }

        public InterpetedElseOperation ElseOperation(ICodeElement codeElement1, ICodeElement codeElement2)
        {
            return new InterpetedElseOperation(codeElement1, codeElement2);
        }

        public InterpetedExplicitMemberName ExplicitMemberName(string item)
        {
            return new InterpetedExplicitMemberName(item);
        }

        public InterpetedExplicitTypeName ExplicitTypeName(string item)
        {
            return new InterpetedExplicitTypeName(item);
        }

        public InterpetedGenericExplicitTypeName GenericExplicitTypeName(string item, ITypeSource[] tokenSources)
        {
            return new InterpetedGenericExplicitTypeName(item, tokenSources);
        }

        public InterpetedGenericExplicitTypeName GenericExplicitTypeName(string item, ITypeDefinition[] tokenSources)
        {
            throw new NotImplementedException();
        }

        public InterpetedGenericTypeDefinition GenericTypeDefinition(NameKey nameKey, ObjectScope scope, GenericTypeParameterDefinition[] genericParameters)
        {
            return new InterpetedGenericTypeDefinition(nameKey, scope, genericParameters);
        }

        public InterpetedIfTrueOperation IfTrueOperation(ICodeElement codeElement1, ICodeElement codeElement2)
        {
            return new InterpetedIfTrueOperation(codeElement1, codeElement2);
        }

        public InterpetedImplementationDefinition ImplementationDefinition(MemberDefinition contextDefinition, ExplicitTypeName explicitTypeName, MemberDefinition parameterDefinition, ICodeElement[] elements, MethodScope methodScope, ICodeElement[] codeElement)
        {
            return new InterpetedImplementationDefinition(contextDefinition, explicitTypeName, parameterDefinition, elements, methodScope, codeElement);
        }
        
        public InterpetedLessThanOperation LessThanOperation(ICodeElement codeElement1, ICodeElement codeElement2)
        {
            return new InterpetedLessThanOperation(codeElement1, codeElement2);
        }

        public InterpetedMemberDefinition MemberDefinition(bool readOnly, ExplicitMemberName explicitMemberName, ITypeSource explicitTypeName)
        {
            return new InterpetedMemberDefinition(readOnly, explicitMemberName, explicitTypeName);
        }

        public InterpetedMemberDefinition MemberDefinition(bool readOnly, ExplicitMemberName explicitMemberName, ITypeDefinition explicitTypeName)
        {
            throw new NotImplementedException();
        }

        public InterpetedMethodDefinition MethodDefinition(ExplicitTypeName explicitTypeName, MemberDefinition parameterDefinition, ICodeElement[] elements, MethodScope methodScope, ICodeElement[] codeElement)
        {
            return new InterpetedMethodDefinition(explicitTypeName, parameterDefinition, elements, methodScope, codeElement);
        }

        public InterpetedModuleDefinition ModuleDefinition(StaticScope scope, IReadOnlyList<AssignOperation> assignOperations)
        {
            return new InterpetedModuleDefinition(scope, assignOperations);
        }

        public InterpetedMultiplyOperation MultiplyOperation(ICodeElement codeElement1, ICodeElement codeElement2)
        {
            return new InterpetedMultiplyOperation(codeElement1, codeElement2);
        }

        public InterpetedNamedTypeDefinition NamedTypeDefinition(NameKey nameKey, ObjectScope scope)
        {
            return new InterpetedNamedTypeDefinition(nameKey, scope);
        }

        public InterpetedNextCallOperation NextCallOperation(ICodeElement codeElement1, ICodeElement codeElement2)
        {
            return new InterpetedNextCallOperation(codeElement1, codeElement2);
        }

        public InterpetedObjectDefinition ObjectDefinition(ObjectScope scope, IReadOnlyList<AssignOperation> assignOperations)
        {
            return new InterpetedObjectDefinition(scope, assignOperations);
        }

        public InterpetedPathOperation PathOperation(ICodeElement left, ICodeElement right)
        {
            throw new NotImplementedException();
        }

        public InterpetedReturnOperation ReturnOperation(ICodeElement codeElement)
        {
            return new InterpetedReturnOperation(codeElement);
        }

        public InterpetedSubtractOperation SubtractOperation(ICodeElement codeElement1, ICodeElement codeElement2)
        {
            return new InterpetedSubtractOperation(codeElement1, codeElement2);
        }

        public InterpetedTypeDefinition TypeDefinition(ObjectScope scope)
        {
            return new InterpetedTypeDefinition(scope);
        }
    }
}
