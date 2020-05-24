using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend.New.CrzayNamespace
{

    internal partial class Tpn
    {
        // is this interface doing me any good?
        // no, it just cause me pain
        internal interface ISetUpTypeProblem
        {
            // a =: x
            TypeProblem2.Type NumberType { get; }
            TypeProblem2.Type StringType { get; }
            TypeProblem2.Type BooleanType { get; }
            TypeProblem2.Type EmptyType { get; }
            void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo);
            TypeProblem2.Value CreateValue(
                IScope scope, 
                IKey typeKey, 
                IConvertTo<TypeProblem2.Value, PlaceholderValue> converter);
            TypeProblem2.Member CreatePrivateMember<T>(
                T scope, 
                IKey key, 
                IOrType<IKey, IError> typeKey, 
                IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter)
                 where T : IStaticScope, IHavePrivateMembers;
            TypeProblem2.Member CreatePrivateMember(
                IHavePrivateMembers scope,
                IKey key,
                IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> type, 
                IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreatePrivateMember<T>(
                T scope,
                IKey key, 
                IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter) 
                where T : IStaticScope, IHavePrivateMembers;

            TypeProblem2.Member CreateMember<T>(
                T scope,
                IKey key,
                IOrType<IKey, IError> typeKey,
                IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter)
                 where T : IStaticScope, IHavePublicMembers;
            TypeProblem2.Member CreateMember(
                IHavePublicMembers scope,
                IKey key,
                IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> type,
                IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMember<T>(
                T scope,
                IKey key,
                IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter)
                where T : IStaticScope, IHavePublicMembers;
            TypeProblem2.Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.TypeReference CreateTypeReference(IStaticScope context, IKey typeKey, IConvertTo<TypeProblem2.TypeReference, IOrType<IFrontendType, IError>> converter);
            TypeProblem2.Scope CreateScope(IStaticScope parent, IConvertTo<TypeProblem2.Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter);
            TypeProblem2.Type CreateType(IStaticScope parent, IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Type CreateType(IStaticScope parent, IOrType<NameKey, ImplicitKey> key, IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Type CreateGenericType(IScope parent, IOrType<NameKey, ImplicitKey> key, IReadOnlyList<TypeAndConverter> placeholders, IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Object CreateObjectOrModule(IScope parent, IKey key, IConvertTo<TypeProblem2.Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter);
            TypeProblem2.Method CreateMethod(IScope parent, string inputName, IConvertTo<TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
            TypeProblem2.Method CreateMethod(IScope parent, IOrType<TypeProblem2.TypeReference, IError> inputType, IOrType<TypeProblem2.TypeReference, IError> outputType, string inputName, IConvertTo<TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
            TypeProblem2.TransientMember GetReturns(IValue s);
            TypeProblem2.TransientMember GetReturns(IScope s);
            TypeProblem2.Member CreateHopefulMember(IValue scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.OrType CreateOrType(IScope s, IKey key, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode1, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode2, IConvertTo<TypeProblem2.OrType, WeakTypeOrOperation> converter);
            IIsPossibly<IKey> GetKey(TypeProblem2.TypeReference type);
            TypeProblem2.Member GetInput(IValue method);
            TypeProblem2.Member GetInput(TypeProblem2.Method method);

            TypeProblem2.MethodType GetMethod(IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> input, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> output);
            void IsNumber(IScope parent, ILookUpType target);
            void IsString(IScope parent, ILookUpType target);
            void IsEmpty(IScope parent, ILookUpType target);
            void IsBool(IScope parent, ILookUpType target);
            void IsBlock(IScope parent, ILookUpType target);

            void HasEntryPoint(IScope parent, TypeProblem2.Scope entry);
            TypeProblem2.Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
        }
    }
}
