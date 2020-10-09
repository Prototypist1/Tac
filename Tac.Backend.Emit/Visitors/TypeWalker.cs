using Prototypist.Toolbox;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Tac.Backend.Emit.Support;
//using Tac.Backend.Emit.SyntaxModel;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Type;

namespace Tac.Backend.Emit.Walkers
{
    class Empty { }
    class Nothing { }
    class TypeVisitor : IOpenBoxesContext<Nothing>
    {

        public readonly Dictionary<IVerifiableType, System.Type> typeCache;

        public TypeVisitor(Dictionary<IVerifiableType, System.Type> typeCache)
        {
            this.typeCache = typeCache ?? throw new ArgumentNullException(nameof(typeCache));
        }

        private void HandleLines(IEnumerable<ICodeElement> lines)
        {
            foreach (var line in lines)
            {
                line.Convert(this);
            } 
        }

        private Nothing HandleOp(IOperation operation) {
            foreach (var line in operation.Operands)
            {
                line.Convert(this);
            }
            return new Nothing();
        }

        private void HandleScope(IFinalizedScope scope)
        {
            foreach (var member in scope.Members) {
                member.Value.Value.Convert(this);
            }
        }
        private System.Type HandleType(IVerifiableType verifiableType)
        {
            return typeCache.GetOrAdd(verifiableType, () => InnerMapType(verifiableType));
        }


        private System.Type InnerMapType(IVerifiableType verifiableType)
        {
            if (verifiableType is INumberType)
            {
                return typeof(double);
            }
            if (verifiableType is IBooleanType)
            {
                return typeof(bool);
            }
            if (verifiableType is IStringType)
            {
                return typeof(string);
            }
            if (verifiableType is IBlockType)
            {
                throw new NotImplementedException();
                // ??
                return typeof(Action);
            }
            if (verifiableType is IEmptyType)
            {
                return typeof(Empty);
            }
            if (verifiableType is IAnyType)
            {
                return typeof(object);
            }
            if (verifiableType.SafeIs(out IInterfaceModuleType moduleType))
            {
                return typeof(ITacObject);
            }
            if (verifiableType is IMethodType method)
            {
                return typeof(ITacObject);
            }
            if (verifiableType is IMemberReference memberReferance)
            {
                // I have to fresh up on what this means....
                // I think it is ref<T> 
                // used on the target of assignment 
                return HandleType(memberReferance.MemberDefinition.Type);
            }
            if (verifiableType is ITypeOr typeOr)
            {
                // we try to find the intersection of the types
                return MergeTypes(typeOr.Left, typeOr.Right);
            }

            throw new NotImplementedException();
        }

        
        private System.Type MergeTypes(IVerifiableType left, IVerifiableType right)
        {

            var leftType = InnerMapType(left); ;
            var rightType = InnerMapType(right);

            // if they are the same we are happy'
            if (leftType == rightType)
            {
                return leftType;
            }

            // if either is a primitive type... return empty?
            if (left.SafeIs(out IPrimitiveType _) || right.SafeIs(out IPrimitiveType _))
            {
                return typeof(Empty);
            }

            // if they are both methods 
            // we have re merge the method io
            if (left.TryGetInput().Is(out var leftInput) &&
                right.TryGetInput().Is(out var rightInput) &&
                left.TryGetReturn().Is(out var leftReturn) &&
                right.TryGetReturn().Is(out var rightReturn))
            {

                return typeof(ITacObject);
            }

            if (left.SafeIs(out IInterfaceModuleType _) && right.SafeIs(out IInterfaceModuleType _)) {

                return typeof(ITacObject);
            }

            return typeof(Empty);
        }

        public Nothing AddOperation(IAddOperation co) =>HandleOp(co);
        public Nothing AssignOperation(IAssignOperation co) => HandleOp(co);
        public Nothing ElseOperation(IElseOperation co) => HandleOp(co);
        public Nothing IfTrueOperation(IIfOperation co) => HandleOp(co);
        public Nothing LastCallOperation(ILastCallOperation co) => HandleOp(co);
        public Nothing LessThanOperation(ILessThanOperation co) => HandleOp(co);
        public Nothing MultiplyOperation(IMultiplyOperation co) => HandleOp(co);
        public Nothing NextCallOperation(INextCallOperation co) => HandleOp(co);
        public Nothing PathOperation(IPathOperation co) => HandleOp(co);
        public Nothing ReturnOperation(IReturnOperation co) => HandleOp(co);
        public Nothing SubtractOperation(ISubtractOperation co) => HandleOp(co);
        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation) => HandleOp(tryAssignOperation);

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            HandleLines(codeElement.Body);
            HandleLines(codeElement.StaticInitailizers);
            HandleScope(codeElement.Scope);
            return new Nothing();
        }


        public Nothing ConstantBool(IConstantBool constantBool) { HandleType(constantBool.Returns()); return new Nothing(); }
        public Nothing ConstantNumber(IConstantNumber codeElement) { HandleType(codeElement.Returns()); return new Nothing(); }
        public Nothing ConstantString(IConstantString co) { HandleType(co.Returns()); return new Nothing(); }
        public Nothing EmptyInstance(IEmptyInstance co) { HandleType(co.Returns()); return new Nothing(); }


        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            HandleLines(entryPointDefinition.Body);
            HandleLines(entryPointDefinition.StaticInitailizers);
            HandleScope(entryPointDefinition.Scope);
            return new Nothing();
        }



        public Nothing ImplementationDefinition(IImplementationDefinition codeElement)
        {
            HandleLines(codeElement.MethodBody);
            HandleLines(codeElement.StaticInitialzers);
            HandleScope(codeElement.IntermediateScope);
            HandleScope(codeElement.Scope);
            codeElement.ContextDefinition.Convert(this);
            codeElement.ParameterDefinition.Convert(this);
            HandleType(codeElement.OutputType);
            return new Nothing();
        }


        public Nothing MemberDefinition(IMemberDefinition codeElement)
        {
            HandleType(codeElement.Type);
            return new Nothing();
        }

        public Nothing MemberReferance(IMemberReference codeElement)
        {
            codeElement.MemberDefinition.Convert(this);
            return new Nothing();
        }

        public Nothing MethodDefinition(IInternalMethodDefinition co)
        {
            HandleLines(co.Body);
            HandleLines(co.StaticInitailizers);
            HandleScope(co.Scope);
            co.ParameterDefinition.Convert(this);
            HandleType(co.InputType);
            HandleType(co.OutputType);
            return new Nothing();
        }

        public Nothing ModuleDefinition(IModuleDefinition codeElement)
        {
            HandleScope(codeElement.Scope);
            codeElement.EntryPoint.Convert(this);
            HandleLines(codeElement.StaticInitialization);
            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton codeElement)
        {
            HandleLines(codeElement.Assignments);
            HandleScope(codeElement.Scope);
            return new Nothing();
        }

        public Nothing TypeDefinition(IInterfaceType codeElement)
        {
            HandleType(codeElement);
            return new Nothing();
        }
    }
}
