using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Tac.Backend.Emit.Lookup;
//using Tac.Backend.Emit.Support;
using Tac.Backend.Emit.Walkers;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Prototypist.TaskChain;

namespace Tac.Backend.Emit.Visitors
{
    internal class EnclosedObjectMember
    {
        public readonly FieldInfo funcField;

        public EnclosedObjectMember(FieldInfo funcField)
        {
            this.funcField = funcField ?? throw new ArgumentNullException(nameof(funcField));
        }
    }

    internal class MethodMakerVisitor : IOpenBoxesContext<Nothing>
    {
        private readonly ModuleBuilder moduleBuilder;
        private readonly WhoDefinedMemberByMethodlike extensionLookup;
        private readonly RealizedMethodLookup realizedMethodLookup;
        public readonly AssemblerTypeTracker typeTracker;
        
        public MethodMakerVisitor(ModuleBuilder moduleBuilder, WhoDefinedMemberByMethodlike extensionLookup, RealizedMethodLookup realizedMethodLookup, AssemblerTypeTracker typeCache)
        {
            this.moduleBuilder = moduleBuilder ?? throw new ArgumentNullException(nameof(moduleBuilder));
            this.extensionLookup = extensionLookup ?? throw new ArgumentNullException(nameof(extensionLookup));
            this.realizedMethodLookup = realizedMethodLookup ?? throw new ArgumentNullException(nameof(realizedMethodLookup));
            this.typeTracker = typeCache ?? throw new ArgumentNullException(nameof(typeCache));
        }

        private void Walk(IEnumerable<ICodeElement> codeElements, ICodeElement current)
        {
            foreach (var element in codeElements)
            {
                element.Convert(this);
            }
        }

        public Nothing AddOperation(IAddOperation co)
        {
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing AssignOperation(IAssignOperation co)
        {
            Walk(co.Operands,co);
            return new Nothing();
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            Walk(codeElement.Body, codeElement);
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
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing EmptyInstance(IEmptyInstance co)
        {
            return new Nothing();
        }



        public Nothing IfTrueOperation(IIfOperation co)
        {
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing ImplementationDefinition(IImplementationDefinition codeElement)
        {
            var name = GenerateName();
            var typeBuilder = moduleBuilder.DefineType(name);

            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new System.Type[] { });
            var myConstructorIL = constructor.GetILGenerator();
            myConstructorIL.Emit(OpCodes.Ldarg_0);
            myConstructorIL.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
            myConstructorIL.Emit(OpCodes.Ret);

            var map = new Dictionary<IMemberDefinition, IOrType<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>>();

            if (extensionLookup.implementationLookup.TryGetValue(codeElement, out var closure))
            {

                foreach (var member in closure.closureMember)
                {
                    PopulateMap(typeBuilder, map, member);
                }
            }

            {
                var field = typeBuilder.DefineField(TranslateName(codeElement.ContextDefinition.Key.SafeCastTo(out NameKey _).Name), TranslateType(codeElement.ContextDefinition.Type), FieldAttributes.Public);
                map[codeElement.ContextDefinition] = OrType.Make<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>(field);
            }

            realizedMethodLookup.Add(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(codeElement), new RealizedMethod(map, typeBuilder, constructor));

            Walk(codeElement.MethodBody, codeElement);
            return new Nothing();
        }

        public Nothing MethodDefinition(IInternalMethodDefinition co)
        {
            var name = GenerateName();
            var typeBuilder = moduleBuilder.DefineType(name);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new System.Type[] { });
            var myConstructorIL = constructor.GetILGenerator();
            myConstructorIL.Emit(OpCodes.Ldarg_0);
            myConstructorIL.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
            myConstructorIL.Emit(OpCodes.Ret);

            var map = new Dictionary<IMemberDefinition, IOrType<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>>();

            if (extensionLookup.methodLookup.TryGetValue(co, out var closure))
            {
                foreach (var member in closure.closureMember)
                {
                    PopulateMap(typeBuilder, map, member);
                }
            }

            realizedMethodLookup.Add(OrType.Make< IInternalMethodDefinition , IImplementationDefinition , IEntryPointDefinition >( co), new RealizedMethod(map, typeBuilder, constructor));

            Walk(co.Body, co);
            return new Nothing();
        }

        private void PopulateMap(TypeBuilder typeBuilder, Dictionary<IMemberDefinition, IOrType<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>> map, KeyValuePair<IMemberDefinition, IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition,IBlockDefinition,IRootScope, IObjectDefiniton>> member)
        {
            member.Value.Switch(
                method =>
                {
                    // everything is enclosed
                    var myType = typeof(Enclosed<>).MakeGenericType(TranslateType(member.Key.Type));
                    var field = typeBuilder.DefineField(TranslateName(member.Key.Key.SafeCastTo(out NameKey _).Name), myType, FieldAttributes.Public);
                    map[member.Key] = OrType.Make<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>((field, myType.GetField(nameof(Enclosed<int>.value))));
                },
                imp =>
                {
                    // everything is enclosed
                    var myType = typeof(Enclosed<>).MakeGenericType(TranslateType(member.Key.Type));
                    var field = typeBuilder.DefineField(TranslateName(member.Key.Key.SafeCastTo(out NameKey _).Name), myType, FieldAttributes.Public);
                    map[member.Key] = OrType.Make<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>((field, myType.GetField(nameof(Enclosed<int>.value))));
                },
                entryPoint =>
                {
                    var myType = typeof(Enclosed<>).MakeGenericType(TranslateType(member.Key.Type));
                    var field = typeBuilder.DefineField(TranslateName(member.Key.Key.SafeCastTo(out NameKey _).Name), myType, FieldAttributes.Public);
                    map[member.Key] = OrType.Make<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>((field, myType.GetField(nameof(Enclosed<int>.value))));
                },
                block =>
                {
                    // everything is enclosed
                    var myType = typeof(Enclosed<>).MakeGenericType(TranslateType(member.Key.Type));
                    var field = typeBuilder.DefineField(TranslateName(member.Key.Key.SafeCastTo(out NameKey _).Name), myType, FieldAttributes.Public);
                    map[member.Key] = OrType.Make<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>((field, myType.GetField(nameof(Enclosed<int>.value))));
                },
                rootScope =>
                {
                    // everything is enclosed
                    var myType = typeof(Enclosed<>).MakeGenericType(TranslateType(member.Key.Type));
                    var field = typeBuilder.DefineField(TranslateName(member.Key.Key.SafeCastTo(out NameKey _).Name), myType, FieldAttributes.Public);
                    map[member.Key] = OrType.Make<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>((field, myType.GetField(nameof(Enclosed<int>.value))));
                },
                obj =>
                {
                    var myType = typeTracker.ResolvePossiblyPrimitive(obj.Returns());
                    var field = typeBuilder.DefineField(TranslateName(member.Key.Key.SafeCastTo(out NameKey _).Name), myType, FieldAttributes.Public);
                    map[member.Key] = OrType.Make<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>( new EnclosedObjectMember(field));
                });
        }

        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            var name = GenerateName();
            var typeBuilder = moduleBuilder.DefineType(name);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new System.Type[] { });
            var myConstructorIL = constructor.GetILGenerator();
            myConstructorIL.Emit(OpCodes.Ldarg_0);
            myConstructorIL.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
            myConstructorIL.Emit(OpCodes.Ret);

            var map = new Dictionary<IMemberDefinition, IOrType<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>>();


            if (extensionLookup.entryPointLookup.TryGetValue(entryPointDefinition, out var closure))
            {
                foreach (var member in closure.closureMember)
                {
                    PopulateMap(typeBuilder, map, member);
                }
            }

            realizedMethodLookup.Add(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entryPointDefinition), new RealizedMethod(map, typeBuilder, constructor));

            Walk(entryPointDefinition.Body, entryPointDefinition);
            return new Nothing();
        }

        private System.Type TranslateType(IVerifiableType type)
        {
            return typeTracker.ResolvePossiblyPrimitive(type);
        }

        private string TranslateName(string name)
        {
            return name.Replace("_", "__").Replace("-", "_");
        }

        // {4E963BB1-1C86-4F75-BD4C-3F9BE16386A9}
        private static readonly Random random = new Random();


        private string GenerateName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 20)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Nothing LastCallOperation(ILastCallOperation co)
        {
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing LessThanOperation(ILessThanOperation co)
        {
            Walk(co.Operands, co);
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

        public Nothing MultiplyOperation(IMultiplyOperation co)
        {
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing NextCallOperation(INextCallOperation co)
        {
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton codeElement)
        {
            Walk(codeElement.Assignments, codeElement);
            return new Nothing();
        }

        public Nothing PathOperation(IPathOperation co)
        {
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing ReturnOperation(IReturnOperation co)
        {
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing SubtractOperation(ISubtractOperation co)
        {
            Walk(co.Operands, co);
            return new Nothing();
        }

        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {
            Walk(tryAssignOperation.Operands, tryAssignOperation);
            return new Nothing();
        }

        public Nothing TypeDefinition(IInterfaceType codeElement)
        {
            return new Nothing();
        }

        public Nothing RootScope(IRootScope co)
        {
            foreach (var assignment in co.Assignments)
            {
                assignment.Convert(this);
            }
            co.EntryPoint.Convert(this);
            return new Nothing();
        }

        public Nothing GenericMethodDefinition(IGenericMethodDefinition co)
        {
            throw new NotImplementedException();
        }
    }
}
