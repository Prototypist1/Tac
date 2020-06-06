//using Prototypist.Toolbox;
//using System.Collections.Generic;
//using Tac.Frontend.SyntaxModel.Operations;
//using Tac.Model;
//using Tac.SemanticModel;
//using Tac.SyntaxModel.Elements.AtomicTypes;

//namespace Tac.Frontend.New.CrzayNamespace
//{

//    internal partial class Tpn
//    {
//        internal interface ITypeSolution
//        {

//            IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Type type); 
//            IBox<PlaceholderValue> GetValue(TypeProblem2.Value value);
//            IBox<WeakMemberDefinition> GetMember(TypeProblem2.Member member);
//            IBox<IOrType<IFrontendType, IError>> GetTypeReference(TypeProblem2.TypeReference typeReference);
//            IBox<IFrontendType> GetInferredType(TypeProblem2.InferredType inferredType, IConvertTo<TypeProblem2.InferredType, IFrontendType> converter);
//            IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> GetScope(TypeProblem2.Scope scope);
//            // when I ungeneric this it should probably have the box inside the or..
//            IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type explicitType);
//            IBox<IOrType<WeakObjectDefinition, WeakModuleDefinition>> GetObject(TypeProblem2.Object @object);
//            IBox<MethodType> GetMethodType(TypeProblem2.MethodType methodType);
//            IBox<WeakTypeOrOperation> GetOrType(TypeProblem2.OrType orType);
//            IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition>> GetMethod(TypeProblem2.Method method);
//            IReadOnlyList<TypeProblem2.Member> GetMembers(IOrType<IHavePrivateMembers, IHavePublicMembers> from);
//            IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> GetType(ILookUpType from);
//            (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from);
//            bool TryGetResultMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, out TypeProblem2.TransientMember? transientMember);
//            bool TryGetInputMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, out TypeProblem2.Member? member);

//            TypeProblem2.TransientMember GetResultMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from);
//            TypeProblem2.Member GetInputMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from);
//            IIsPossibly<TypeProblem2.Scope> GetEntryPoint(IStaticScope from);
//        }
//    }
//}
