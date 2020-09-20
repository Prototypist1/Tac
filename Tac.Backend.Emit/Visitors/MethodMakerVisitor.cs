﻿using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Tac.Backend.Emit.Lookup;
using Tac.Backend.Emit.Walkers;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Backend.Emit.Visitors
{
    internal class MethodMakerVisitor : IOpenBoxesContext<Nothing>
    {
        private readonly ModuleBuilder moduleBuilder;
        private readonly ExtensionLookup extensionLookup;
        private readonly RealizedMethodLookup realizedMethodLookup;

        private void Walk(IEnumerable<ICodeElement> codeElements)
        {
            foreach (var element in codeElements)
            {
                element.Convert(this);
            }
        }

        public Nothing AddOperation(IAddOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing AssignOperation(IAssignOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            Walk(codeElement.Body);
            return new Nothing();
        }

        public Nothing ConstantBool(IConstantBool constantBool)
        {
            return new Nothing();
        }

        public Nothing ConstantNumber(IConstantNumber codeElement)
        {
            return new Nothing();
        }

        public Nothing ConstantString(IConstantString co)
        {
            return new Nothing();
        }

        public Nothing ElseOperation(IElseOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing EmptyInstance(IEmptyInstance co)
        {
            return new Nothing();
        }



        public Nothing IfTrueOperation(IIfOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing ImplementationDefinition(IImplementationDefinition codeElement)
        {
            var name = GenerateName();
            var typeBuilder = moduleBuilder.DefineType(name);

            var map = new Dictionary<IMemberDefinition, FieldInfo>();

            if (extensionLookup.implementationLookup.TryGetValue(codeElement, out var closure))
            {

                foreach (var member in closure.closureMember)
                {
                    var field = typeBuilder.DefineField(TranslateName(member.Key.SafeCastTo(out NameKey _).Name), TranslateType(member.Type), FieldAttributes.Public);
                    map[member] = field;
                }
            }

            {
                var field = typeBuilder.DefineField(TranslateName(codeElement.ContextDefinition.Key.SafeCastTo(out NameKey _).Name), TranslateType(codeElement.ContextDefinition.Type), FieldAttributes.Public);
                map[codeElement.ContextDefinition] = field;
            }

            realizedMethodLookup.Add(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(codeElement), new RealizedMethod(map, typeBuilder));

            Walk(codeElement.MethodBody);
            return new Nothing();
        }

        public Nothing MethodDefinition(IInternalMethodDefinition co)
        {
            var name = GenerateName();
            var typeBuilder = moduleBuilder.DefineType(name);

            var map = new Dictionary<IMemberDefinition, FieldInfo>();

            if (extensionLookup.methodLookup.TryGetValue(co, out var closure)) {

                foreach (var member in closure.closureMember)
                {
                    var field = typeBuilder.DefineField(TranslateName(member.Key.SafeCastTo(out NameKey _).Name), TranslateType(member.Type), FieldAttributes.Public);
                    map[member] = field;
                }
            }

            realizedMethodLookup.Add(OrType.Make< IInternalMethodDefinition , IImplementationDefinition , IEntryPointDefinition >( co), new RealizedMethod(map, typeBuilder));

            Walk(co.Body);
            return new Nothing();
        }

        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            var name = GenerateName();
            var typeBuilder = moduleBuilder.DefineType(name);

            var map = new Dictionary<IMemberDefinition, FieldInfo>();


            //if (extensionLookup.entryPointLookup.TryGetValue(entryPointDefinition, out var closure))
            //{

            //    foreach (var member in closure.closureMember)
            //    {
            //        var field = typeBuilder.DefineField(TranslateName(member.Key.SafeCastTo(out NameKey _).Name), TranslateType(member.Type), FieldAttributes.Public);
            //        map[member] = field;
            //    }
            //}

            realizedMethodLookup.Add(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entryPointDefinition), new RealizedMethod(map, typeBuilder));

            Walk(entryPointDefinition.Body);
            return new Nothing();
        }

        private System.Type TranslateType(IVerifiableType type)
        {
        }

        private string TranslateName(string name)
        {
        }

        private string GenerateName()
        {
        }



        public Nothing LastCallOperation(ILastCallOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing LessThanOperation(ILessThanOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing MemberDefinition(IMemberDefinition codeElement)
        {
            return new Nothing();
        }

        public Nothing MemberReferance(IMemberReference codeElement)
        {
            return new Nothing();
        }

        public Nothing ModuleDefinition(IModuleDefinition codeElement)
        {
            Walk(codeElement.StaticInitialization);
            return new Nothing();
        }

        public Nothing MultiplyOperation(IMultiplyOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing NextCallOperation(INextCallOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton codeElement)
        {
            Walk(codeElement.Assignments);
            return new Nothing();
        }

        public Nothing PathOperation(IPathOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing ReturnOperation(IReturnOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing SubtractOperation(ISubtractOperation co)
        {
            Walk(co.Operands);
            return new Nothing();
        }

        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {
            Walk(tryAssignOperation.Operands);
            return new Nothing();
        }

        public Nothing TypeDefinition(IInterfaceType codeElement)
        {
            return new Nothing();
        }
    }
}
