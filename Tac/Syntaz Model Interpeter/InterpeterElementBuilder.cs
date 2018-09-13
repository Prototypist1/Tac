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
    public interface IInterpeted {

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
        IInterpeted
        >
    {
        public InterpetedBlockDefinition BlockDefinition(ICodeElement[] elements, LocalStaticScope scope, ICodeElement[] codeElement) => throw new NotImplementedException();
        public InterpetedConstantNumber ConstantNumber(double dub) => throw new NotImplementedException();
        public InterpetedExplicitMemberName ExplicitMemberName(string item) => throw new NotImplementedException();
        public InterpetedExplicitTypeName ExplicitTypeName(string item) => throw new NotImplementedException();
        public InterpetedGenericExplicitTypeName GenericExplicitTypeName(string item, ITypeSource[] tokenSources) => throw new NotImplementedException();
        public InterpetedGenericTypeDefinition GenericTypeDefinition(NameKey nameKey, ObjectScope scope, GenericTypeParameterDefinition[] genericParameters) => throw new NotImplementedException();
        public InterpetedImplementationDefinition ImplementationDefinition(MemberDefinition contextDefinition, ExplicitTypeName explicitTypeName, MemberDefinition parameterDefinition, ICodeElement[] elements, MethodScope methodScope, ICodeElement[] codeElement) => throw new NotImplementedException();
        public InterpetedImplicitTypeReferance ImplicitTypeReferance(ICodeElement left) => throw new NotImplementedException();
        public InterpetedMemberDefinition MemberDefinition(bool readOnly, ExplicitMemberName explicitMemberName, ITypeSource explicitTypeName) => throw new NotImplementedException();
        public InterpetedMethodDefinition MethodDefinition(ExplicitTypeName explicitTypeName, MemberDefinition parameterDefinition, ICodeElement[] elements, MethodScope methodScope, ICodeElement[] codeElement) => throw new NotImplementedException();
        public InterpetedModuleDefinition ModuleDefinition(StaticScope scope, IReadOnlyList<AssignOperation> assignOperations) => throw new NotImplementedException();
        public InterpetedNamedTypeDefinition NamedTypeDefinition(NameKey nameKey, ObjectScope scope) => throw new NotImplementedException();
        public InterpetedObjectDefinition ObjectDefinition(ObjectScope scope, IReadOnlyList<AssignOperation> assignOperations) => throw new NotImplementedException();
        public InterpetedTypeDefinition TypeDefinition(ObjectScope scope) => throw new NotImplementedException();
    }
}
