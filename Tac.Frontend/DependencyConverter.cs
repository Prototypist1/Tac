using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Semantic_Model;

namespace Tac.Frontend
{
    internal class DependencyConverter : IOpenBoxesContext<IFrontendCodeElement<ICodeElement>, IBacking>
    {
        private readonly Dictionary<object, IFrontendCodeElement<ICodeElement>> backing = new Dictionary<object, IFrontendCodeElement<ICodeElement>>();

        public DependencyConverter()
        {
        }

        public IFrontendCodeElement<ICodeElement> MemberDefinition(IMemberDefinition member)
        {
            if (backing.TryGetValue(member, out var res))
            {
                return res;
            }
            else
            {
                var interpetedMemberDefinition = new WeakMemberDefinition(
                    member.ReadOnly,
                    member.Key,
                    Possibly.Is(
                        new WeakTypeReference(
                            Possibly.Is(
                                new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(
                                    Possibly.Is(
                                        TypeMap.MapType(member.Type)))))));
                backing.Add(member, interpetedMemberDefinition);
                return interpetedMemberDefinition;
            }
        }

        public IFrontendCodeElement<ICodeElement> GenericTypeDefinition(IGenericInterfaceDefinition codeElement)
        {
            throw new NotImplementedException();
            //if (backing.TryGetValue(codeElement, out var res))
            //{
            //    return res;
            //}
            //else
            //{
            //    var op = new WeakGenericTypeDefinition(,,);
            //    backing.Add(codeElement, op);
            //    return op;
            //}
        }

        public IFrontendCodeElement<ICodeElement> TypeDefinition(IInterfaceType codeElement)
        {
            throw new NotImplementedException();
            //if (backing.TryGetValue(codeElement, out var res))
            //{
            //    return res;
            //}
            //else
            //{
            //    var op = new WeakTypeDefinition(,);
            //    backing.Add(codeElement, op);
            //    return op;
            //}
        }

        public IFrontendCodeElement<ICodeElement> AddOperation(IAddOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> AssignOperation(IAssignOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> BlockDefinition(IBlockDefinition codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ConstantNumber(IConstantNumber codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ElseOperation(IElseOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> IfTrueOperation(IIfOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ImplementationDefinition(IImplementationDefinition codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> LastCallOperation(ILastCallOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> LessThanOperation(ILessThanOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> MemberReferance(IMemberReferance codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> MethodDefinition(IInternalMethodDefinition _) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> MultiplyOperation(IMultiplyOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> NextCallOperation(INextCallOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ObjectDefinition(IObjectDefiniton codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> PathOperation(IPathOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ReturnOperation(IReturnOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> SubtractOperation(ISubtractOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> TypeReferance(ITypeReferance codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ModuleDefinition(IModuleDefinition codeElement)=> throw new NotImplementedException();
    }

    internal static class TypeMap
    {

        public static IFrontendType<IVerifiableType> MapType(IVerifiableType verifiableType)
        {
            if (verifiableType is INumberType)
            {
                return new NumberType();
            }
            if (verifiableType is IBooleanType)
            {
                return new BooleanType();
            }
            if (verifiableType is IStringType)
            {
                return new StringType();
            }
            if (verifiableType is IBlockType)
            {
                return new BlockType();
            }
            if (verifiableType is IEmptyType)
            {
                return new EmptyType();
            }
            if (verifiableType is IAnyType)
            {
                return new AnyType();
            }
            if (verifiableType is IMethodType method)
            {
                return new MethodType(
                    MapType(method.InputType),
                    MapType(method.OutputType)
                    );
            }
            if (verifiableType is IImplementationType implementation)
            {
                return new ImplementationType(
                    MapType(implementation.ContextType),
                    MapType(implementation.InputType),
                    MapType(implementation.OutputType)
                    );
            }

            throw new NotImplementedException();
        }

    }

}

