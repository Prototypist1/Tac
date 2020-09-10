using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Backend.Emit.Walkers
{
    class AssemblerWalker : IOpenBoxesContext<Nothing>
    {

        public  IIsPossibly<ILGenerator> generator;
        public AssemblerWalker(TypeChangeLookup typeChangeLookup)
        {
        }

        public Nothing AddOperation(IAddOperation co)
        {
            Walk(co.Operands);
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Add_Ovf);
            return new Nothing();
        }

        public Nothing AssignOperation(IAssignOperation co)
        {
            throw new NotImplementedException("we have to generate the converters");

            // there is different store command for different targets
            // is it an arg?
            // is it a field
            // is it a local?

            return Walk(co.Operands);
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            // this is nothing to MSIL
            return Walk(codeElement.Body);
        }

        public Nothing ConstantBool(IConstantBool constantBool) {
            if (constantBool.Value)
            {
                generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
            }
            else
            {
                generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
            }
            return new Nothing();
        }
        public Nothing ConstantNumber(IConstantNumber codeElement) {
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_R8, codeElement.Value);
            return new Nothing();
        }
        public Nothing ConstantString(IConstantString co)
        {
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldstr, co.Value);
            return new Nothing();
        }
        public Nothing EmptyInstance(IEmptyInstance co)
        {
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldnull);
            return new Nothing();
        }
        public Nothing TypeDefinition(IInterfaceType codeElement) => new Nothing();


        public Nothing ElseOperation(IElseOperation co)
        {
            throw new NotImplementedException("");
            // we need to put a branch 
            // but we need to know who far to brach
            
            // I think I am going to need different walkers
            // one that walks stuff like if blocks
            // and just ques up a list of op codes to write
            // then this can count how many, and put the correct jump in

            return Walk(co.Operands);
        }


        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {

            throw new NotImplementedException("");
            return Walk(entryPointDefinition.Body);
        }

        public Nothing IfTrueOperation(IIfOperation co)
        {
            throw new NotImplementedException("");
            // we need to put a branch 
            // but we need to know who far to brach
            return Walk(co.Operands);
        }

        public Nothing ImplementationDefinition(IImplementationDefinition implementation)
        {
            return Walk(implementation.MethodBody);
        }

        public Nothing LastCallOperation(ILastCallOperation co)
        {
            return Walk(co.Operands);
        }

        public Nothing LessThanOperation(ILessThanOperation co)
        {
            Walk(co.Operands);
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Clt);
            return new Nothing();
        }

        public Nothing MemberDefinition(IMemberDefinition codeElement)
        {
            return new Nothing();
        }

        public Nothing MemberReferance(IMemberReferance memberReferance)
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
            Walk(co.Operands);
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Mul_Ovf);
            return new Nothing();
        }

        public Nothing NextCallOperation(INextCallOperation co)
        {
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
            Walk(co.Operands);
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Sub_Ovf);
            return new Nothing();
        }

        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {
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
