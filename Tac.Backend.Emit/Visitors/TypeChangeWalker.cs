using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Type;

namespace Tac.Backend.Emit.Walkers
{

    class TypeChangeVisitor : IOpenBoxesContext<Nothing>
    {

        private readonly TypeChangeLookup typeChangeLookup;

        public TypeChangeVisitor(TypeChangeLookup typeChangeLookup)
        {
            this.typeChangeLookup = typeChangeLookup ?? throw new ArgumentNullException(nameof(typeChangeLookup));
        }

        public Nothing AddOperation(IAddOperation co)
        {
            return Walk(co.Operands);
        }

        public Nothing AssignOperation(IAssignOperation co)
        {
            // there could be a conversion here!
            var fromType = co.Left.Returns();
            var toType = co.Right.Returns();

            if (fromType != toType)
            {
                typeChangeLookup.TryAdd(fromType, toType);
            }

            return Walk(co.Operands);
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            return  Walk(codeElement.Body);
        }

        public Nothing ConstantBool(IConstantBool constantBool) => new Nothing();
        public Nothing ConstantNumber(IConstantNumber codeElement) => new Nothing();
        public Nothing ConstantString(IConstantString co) => new Nothing();
        public Nothing EmptyInstance(IEmptyInstance co) => new Nothing();
        public Nothing TypeDefinition(IInterfaceType codeElement) => new Nothing();


        public Nothing ElseOperation(IElseOperation co)
        {
            return Walk(co.Operands);
        }


        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            return Walk(entryPointDefinition.Body);
        }

        public Nothing IfTrueOperation(IIfOperation co)
        {
            return Walk(co.Operands);
        }

        public Nothing ImplementationDefinition(IImplementationDefinition implementation)
        {
            return Walk(implementation.MethodBody);
        }

        public Nothing LastCallOperation(ILastCallOperation co)
        {
            // there could be a conversion here!
            var fromType = co.Right.Returns();
            var toType = co.Left.Returns().SafeCastTo(out IMethodType _).InputType;

            if (fromType != toType) {
                typeChangeLookup.TryAdd(fromType, toType);
            }

            return Walk(co.Operands);
        }

        public Nothing LessThanOperation(ILessThanOperation co)
        {
            return Walk(co.Operands);
        }

        public Nothing MemberDefinition(IMemberDefinition codeElement)
        {
            return new Nothing();
        }

        public Nothing MemberReferance(IMemberReference memberReferance)
        {
            return new Nothing();
        }

        public Nothing MethodDefinition(IInternalMethodDefinition method)
        {
            return Walk(method.Body);
        }

        public Nothing ModuleDefinition(IModuleDefinition module)
        {
            return Walk(module.StaticInitialization);
        }

        public Nothing MultiplyOperation(IMultiplyOperation co)
        {
            return Walk(co.Operands);
        }

        public Nothing NextCallOperation(INextCallOperation co)
        {
            // there could be a conversion here!
            var fromType = co.Left.Returns();
            var toType = co.Right.Returns().SafeCastTo(out IMethodType _).InputType;

            if (fromType != toType)
            {
                typeChangeLookup.TryAdd(fromType, toType);
            }

            return Walk(co.Operands);
        }

        public Nothing ObjectDefinition(IObjectDefiniton @object)
        {
            return Walk(@object.Assignments);
        }

        public Nothing PathOperation(IPathOperation path)
        {

            return Walk(path.Operands);
        }

        public Nothing ReturnOperation(IReturnOperation co)
        {
            // there could be a conversion here!
            return Walk(co.Operands);
        }

        public Nothing SubtractOperation(ISubtractOperation co)
        {
            return Walk(co.Operands);
        }

        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {
            // this sucks
            // we have to do a metadata based check to see if the type qualifies
            // and we have to do a metadata based converstion
            return Walk(tryAssignOperation.Operands);
        }


        private Nothing Walk(IEnumerable<ICodeElement> elements)
        {

            foreach (var line in elements)
            {
                line.Convert(this);
            }

            return new Nothing();
        }
    }
}
