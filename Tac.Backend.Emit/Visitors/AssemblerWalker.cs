using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Tac.Backend.Emit.Lookup;
using Tac.Backend.Emit.Support;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Backend.Emit.Walkers
{
    class AssemblerVisitor : IOpenBoxesContext<Nothing>
    {

        private readonly TypeChangeLookup typeChangeLookup;
        private readonly MemberKindLookup memberKindLookup;
        private readonly ExtensionLookup extensionLookup;
        private readonly RealizedMethodLookup realizedMethodLookup;
        private readonly Dictionary<IVerifiableType, System.Type> typeCache;


        private IReadOnlyList<ICodeElement> stack;
        public  IIsPossibly<ILGenerator> generator;
        public AssemblerVisitor(TypeChangeLookup typeChangeLookup, IReadOnlyList<ICodeElement> stack)
        {
            this.typeChangeLookup = typeChangeLookup;
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        public AssemblerVisitor Push(ICodeElement another) {
            var list = stack.ToList();
            list.Add(another);
            return new AssemblerVisitor(typeChangeLookup, list);
        }

        public Nothing AddOperation(IAddOperation co)
        {
            Walk(co.Operands, co);
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

            // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            // be careful this does not leave anything on the stack

            return Walk(co.Operands, co);
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            // this is nothing to MSIL
            return Walk(codeElement.Body, codeElement);
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

            var next = this.Push(co);

            var myIf = co.Operands[0].SafeCastTo(out IIfOperation _);

            var nextNext = next.Push(myIf);
            var topOfElseLabel = generator.GetOrThrow().DefineLabel();
            var bottomOfElse = generator.GetOrThrow().DefineLabel();
            myIf.Operands[0].Convert(nextNext);
            // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            if (this.stack.Last().SafeIs(out IOperation _)) {
                // we dup so that we return
                // else in tac returns false if it ran, true otherwise
                // so you can do
                // ... else {} > someMethod
                generator.GetOrThrow().Emit(OpCodes.Dup);
                // this is a very important assumption
                // the {} of the if CANNOT leave anything on the stack
                // I don't think that should happen very often since each statement tend to clear it's stack
                // often but not always, right here we are leaving something on the statck
                // that is why we need to check something is consuming it 
            }
            generator.GetOrThrow().Emit(OpCodes.Brfalse, topOfElseLabel);
            myIf.Operands[1].Convert(nextNext);
            generator.GetOrThrow().Emit(OpCodes.Br, bottomOfElse);
            generator.GetOrThrow().MarkLabel(topOfElseLabel);
            co.Operands[1].Convert(next);
            generator.GetOrThrow().MarkLabel(bottomOfElse);

            return new Nothing();
        }


        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {

            throw new NotImplementedException("");
            return Walk(entryPointDefinition.Body, entryPointDefinition);
        }

        public Nothing IfTrueOperation(IIfOperation co)
        {
            var next = this.Push(co);

            var label = generator.GetOrThrow().DefineLabel();
            co.Operands[0].Convert(next);
            // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            if (this.stack.Last().SafeIs(out IOperation _))
            {
                // we dup so that we return
                // if in tac returns true if it ran, false otherwise
                // so you can do
                // ... if {} > someMethod
                generator.GetOrThrow().Emit(OpCodes.Dup);
                // this is a very important assumption
                // the {} of the if CANNOT leave anything on the stack
                // I don't think that should happen very often since each statement tend to clear it's stack
                // often but not always, right here we are leaving something on the statck
                // that is why we need to check something is consuming it 
            }
            generator.GetOrThrow().Emit(OpCodes.Brfalse, label);
            co.Operands[1].Convert(next);
            generator.GetOrThrow().MarkLabel(label);
            return new Nothing();
        }

        public Nothing ImplementationDefinition(IImplementationDefinition implementation)
        {
            return Walk(implementation.MethodBody, implementation);
        }

        public Nothing LastCallOperation(ILastCallOperation co)
        {
            return Walk(co.Operands,co);
        }

        public Nothing LessThanOperation(ILessThanOperation co)
        {
            Walk(co.Operands,co);
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Clt);
            return new Nothing();
        }

        public Nothing MemberDefinition(IMemberDefinition codeElement)
        {
            return new Nothing();
        }

        private IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition> ConvertToMethodlike(ICodeElement frame) {
            if (frame.SafeIs(out IInternalMethodDefinition method)) {
                return OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(method);
            }
            if (frame.SafeIs(out IImplementationDefinition imp))
            {
                return OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(imp);
            }
            if (frame.SafeIs(out IEntryPointDefinition entry))
            {
                return OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entry);
            }
            throw new Exception("should have been one of those");
        }

        public Nothing MemberReferance(IMemberReference memberReference)
        {
            // we need to determine what type of member reference it is 
            // is it a local?
            // ldloc
            // is it an argument 
            // ldarg


            // see if it is on the closure 
            // walk up the stack and hope you run in to it
            foreach (var frame in stack.Reverse())
            {
                if (extensionLookup.TryGetClosure(frame, out var closure)) {
                    // these are fields!!

                    if (closure.closureMember.Contains(memberReference.MemberDefinition)) {

                        var realizedMethod =  realizedMethodLookup.GetValueOrThrow(ConvertToMethodlike(frame));

                        var field = realizedMethod.fields[memberReference.MemberDefinition];

                        generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldfld, field);

                        return new Nothing();
                    }
                }
            } 

            if (memberKindLookup.IsArgument(memberReference.MemberDefinition, out var orTypeArg)) {
                return orTypeArg.SwitchReturns(
                    imp =>
                    {
                        // I only allow 1 argument 
                        generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                        return new Nothing();
                    },
                    method =>
                    {
                        // I only allow 1 argument 
                        generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                        return new Nothing();
                    });
            }

            if (memberKindLookup.IsLocal(memberReference.MemberDefinition, out var orTypeLocal))
            {
                return orTypeLocal.SwitchReturns(
                    entryPoint => {

                       var index = Array.IndexOf(entryPoint.Scope.Members.Values.Select(x => x.Value).ToArray(), memberReference.MemberDefinition);
                       LoadLocal(index);
                        return new Nothing();
                    },
                    imp => {
                        var index = Array.IndexOf(imp.Scope.Members.Values.Select(x => x.Value).ToArray(), memberReference.MemberDefinition);
                        LoadLocal(index);
                        return new Nothing();
                    },
                    method => {
                        var index = Array.IndexOf(method.Scope.Members.Values.Select(x => x.Value).ToArray(), memberReference.MemberDefinition);
                        LoadLocal(index);
                        return new Nothing();
                    });
            }

            if (memberKindLookup.IsField(memberReference.MemberDefinition, out var orTypeField)) {
                return orTypeField.SwitchReturns(
                    imp => {
                        // this is the closure
                        // I need the field info...

                        var realizedMethod = realizedMethodLookup.GetValueOrThrow(ConvertToMethodlike(imp));

                        var field = realizedMethod.fields[memberReference.MemberDefinition];

                        generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldfld, field);

                        return new Nothing();
                    },
                    obj =>
                    {

                        // this "b" inside a path like: a.b
                        // we count on "a" to have already been load

                        var index = Array.IndexOf(obj.Scope.Members.Values.Select(x => x.Value).ToArray(), memberReference.MemberDefinition);
                        LoadInt(index);

                        if (typeCache[memberReference.MemberDefinition.Type] == typeof(TacCastObject)) {
                            generator.GetOrThrow().EmitCall(OpCodes.Call, getComplexMember.Value, new[] {typeof( int)});
                        }
                        else
                        {

                            

                            generator.GetOrThrow().EmitCall(OpCodes.Call, getSimpleMember.Value.MakeGenericMethod(typeCache[memberReference.MemberDefinition.Type]), new[] { typeof(int) });
                        }

                        throw new NotImplementedException("");

                        return new Nothing();
                    });
            }

            return new Nothing();
        }

        private Lazy<MethodInfo> getComplexMember = new Lazy<MethodInfo>(() => {
            return typeof(TacCastObject).GetMethod(nameof(TacCastObject.GetComplexMember));
        });

        private Lazy<MethodInfo> getSimpleMember = new Lazy<MethodInfo>(() => {
            return typeof(TacCastObject).GetMethod(nameof(TacCastObject.GetSimpleMember));
        });

        private void LoadInt(int value)
        {
            switch (value)
            {
                case 0:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4_7);
                    return;
                default:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldc_I4, value);
                    return;
            }
        }

        private void LoadLocal(int index) {
            switch (index)
            {
                case 0:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
                    return;
                case 1:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_1);
                    return;
                case 2:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_2);
                    return;
                case 3:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_3);
                    return;
                default:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_S, index);
                    return;
            }
        }

        private void LoadLocalAddress(int index)
        {
            switch (index)
            {
                case 0:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloca,index);
                    return;
                case 1:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_1);
                    return;
                case 2:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_2);
                    return;
                case 3:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_3);
                    return;
                default:
                    generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Ldloc_S, index);
                    return;
            }
        }

        public Nothing MethodDefinition(IInternalMethodDefinition method)
        {
            return Walk(method.Body, method);
        }

        public Nothing ModuleDefinition(IModuleDefinition module)
        {
            return Walk(module.StaticInitialization, module);
        }

        public Nothing MultiplyOperation(IMultiplyOperation co)
        {
            Walk(co.Operands, co);
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Mul_Ovf);
            return new Nothing();
        }

        public Nothing NextCallOperation(INextCallOperation co)
        {
            return Walk(co.Operands, co);
        }

        public Nothing ObjectDefinition(IObjectDefiniton @object)
        {
            return Walk(@object.Assignments, @object);
        }

        public Nothing PathOperation(IPathOperation path)
        {

            return Walk(path.Operands,path);
        }

        public Nothing ReturnOperation(IReturnOperation co)
        {
            // there could be a conversion here!
            return Walk(co.Operands, co);
        }

        public Nothing SubtractOperation(ISubtractOperation co)
        {
            Walk(co.Operands,co);
            generator.GetOrThrow().Emit(System.Reflection.Emit.OpCodes.Sub_Ovf);
            return new Nothing();
        }

        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {
            return Walk(tryAssignOperation.Operands, tryAssignOperation);
        }


        private Nothing Walk(IEnumerable<ICodeElement> elements, ICodeElement element)
        {

            foreach (var line in elements)
            {
                line.Convert(this.Push(element));
            }

            return new Nothing();
        }
    }
}
