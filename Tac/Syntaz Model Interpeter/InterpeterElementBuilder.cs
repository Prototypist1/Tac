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

    }

    public interface IInterpeted {
        InterpetedContext Interpet(InterpetedContext interpetedContext);
    }

    class InterpeterElementBuilder : IElementBuilder<
        InterpetedMemberDefinition,
        InterpetedExplicitMemberName,
        InterpetedExplicitTypeName,
        InterpetedGenericExplicitTypeName,
        InterpetedImplicitTypeReferance,
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
        IInterpeted
        >
    {
        public InterpetedAddOperation AddOperation(ICodeElement codeElement1, ICodeElement codeElement2) => new InterpetedAddOperation(codeElement1, codeElement2);
        public InterpetedAssignOperation AssignOperation(ICodeElement codeElement, IMemberSource memberSource) => new InterpetedAssignOperation(codeElement, memberSource);
        public InterpetedBlockDefinition BlockDefinition(ICodeElement[] elements, LocalStaticScope scope, ICodeElement[] codeElement) => new InterpetedBlockDefinition(elements, scope, codeElement);
        public InterpetedConstantNumber ConstantNumber(double dub) => new InterpetedConstantNumber(dub);
        public InterpetedElseOperation ElseOperation(ICodeElement codeElement1, ICodeElement codeElement2) => new InterpetedElseOperation(codeElement1, codeElement2);
        public InterpetedExplicitMemberName ExplicitMemberName(string item) => new InterpetedExplicitMemberName(item);
        public InterpetedExplicitTypeName ExplicitTypeName(string item) => new InterpetedExplicitTypeName(item);
        public InterpetedGenericExplicitTypeName GenericExplicitTypeName(string item, ITypeSource[] tokenSources) => new InterpetedGenericExplicitTypeName(item, tokenSources);
        public InterpetedGenericTypeDefinition GenericTypeDefinition(NameKey nameKey, ObjectScope scope, GenericTypeParameterDefinition[] genericParameters) => new InterpetedGenericTypeDefinition(nameKey, scope, genericParameters);
        public InterpetedIfTrueOperation IfTrueOperation(ICodeElement codeElement1, ICodeElement codeElement2) => new InterpetedIfTrueOperation(codeElement1, codeElement2);
        public InterpetedImplementationDefinition ImplementationDefinition(MemberDefinition contextDefinition, ExplicitTypeName explicitTypeName, MemberDefinition parameterDefinition, ICodeElement[] elements, MethodScope methodScope, ICodeElement[] codeElement) => new InterpetedImplementationDefinition(contextDefinition, explicitTypeName, parameterDefinition, elements, methodScope, codeElement);
        public InterpetedImplicitTypeReferance ImplicitTypeReferance(ICodeElement left) => new InterpetedImplicitTypeReferance(left);
        public InterpetedLessThanOperation LessThanOperation(ICodeElement codeElement1, ICodeElement codeElement2) => new InterpetedLessThanOperation(codeElement1, codeElement2);
        public InterpetedMemberDefinition MemberDefinition(bool readOnly, ExplicitMemberName explicitMemberName, ITypeSource explicitTypeName) => new InterpetedMemberDefinition(readOnly, explicitMemberName, explicitTypeName);
        public InterpetedMethodDefinition MethodDefinition(ExplicitTypeName explicitTypeName, MemberDefinition parameterDefinition, ICodeElement[] elements, MethodScope methodScope, ICodeElement[] codeElement) => new InterpetedMethodDefinition(explicitTypeName, parameterDefinition, elements, methodScope, codeElement);
        public InterpetedModuleDefinition ModuleDefinition(StaticScope scope, IReadOnlyList<AssignOperation> assignOperations) => new InterpetedModuleDefinition(scope, assignOperations);
        public InterpetedMultiplyOperation MultiplyOperation(ICodeElement codeElement1, ICodeElement codeElement2) => new InterpetedMultiplyOperation(codeElement1, codeElement2);
        public InterpetedNamedTypeDefinition NamedTypeDefinition(NameKey nameKey, ObjectScope scope) => new InterpetedNamedTypeDefinition(nameKey, scope);
        public InterpetedNextCallOperation NextCallOperation(ICodeElement codeElement1, ICodeElement codeElement2) => new InterpetedNextCallOperation(codeElement1, codeElement2);
        public InterpetedObjectDefinition ObjectDefinition(ObjectScope scope, IReadOnlyList<AssignOperation> assignOperations) => new InterpetedObjectDefinition(scope, assignOperations);
        public InterpetedReturnOperation ReturnOperation(ICodeElement codeElement) => new InterpetedReturnOperation(codeElement);
        public InterpetedSubtractOperation SubtractOperation(ICodeElement codeElement1, ICodeElement codeElement2) => new InterpetedSubtractOperation(codeElement1, codeElement2);
        public InterpetedTypeDefinition TypeDefinition(ObjectScope scope) => new InterpetedTypeDefinition(scope);
    }
}
