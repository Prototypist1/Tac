using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Frontend
{
    internal class DependencyConverter : IOpenBoxesContext<IFrontendCodeElement<ICodeElement>>
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
                var interpetedMemberDefinition = new InterpetedMemberDefinition<T>();
                backing.Add(member, interpetedMemberDefinition);
                return interpetedMemberDefinition.Init(member.Key);
            }
        }

        public IFrontendCodeElement<ICodeElement> GenericTypeDefinition(IGenericInterfaceDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedGenericTypeDefinition();
                backing.Add(codeElement, op);
                op.Init();
                return op;
            }
        }

        public IFrontendCodeElement<ICodeElement> TypeDefinition(IInterfaceType codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedTypeDefinition();
                backing.Add(codeElement, op);
                op.Init();
                return op;
            }
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
        public IFrontendCodeElement<ICodeElement> MethodDefinition(IInternalMethodDefinition codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ModuleDefinition(IModuleDefinition codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> MultiplyOperation(IMultiplyOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> NextCallOperation(INextCallOperation co) => throw new NotImplementedException();
        private IFrontendCodeElement<ICodeElement> NextCallOperation<TIn, TOut>(INextCallOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ObjectDefinition(IObjectDefiniton codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> PathOperation(IPathOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> ReturnOperation(IReturnOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> SubtractOperation(ISubtractOperation co) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> TypeReferance(ITypeReferance codeElement) => throw new NotImplementedException();
        public IFrontendCodeElement<ICodeElement> InternalMethodDefinition(IInternalMethodDefinition codeElement) => MethodDefinition(codeElement);

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

