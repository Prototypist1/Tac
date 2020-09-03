﻿using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tac.Backend.Emit.SyntaxModel.Elements;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;
using static Tac.Backend.Emit.Public.AssemblyBuilder;
using Prototypist.Toolbox.Object;

namespace Tac.Backend.Emit.SyntaxModel
{

    internal class Definitions: IOpenBoxesContext<IAssembledOperation, InterpetedAssemblyBacking>
    {
        private readonly Dictionary<object, IAssembledOperation> backing = new Dictionary<object, IAssembledOperation>();


        public InterpetedEntryPointDefinition? EntryPoint { get; private set; }

        public Definitions()
        {
        }

        public IAssembledOperation MemberDefinition(IMemberDefinition member)
        {
            if (backing.TryGetValue(member, out var res))
            {
                return res;
            }
            else
            {
                var interpetedMemberDefinition = new InterpetedMemberDefinition();
                backing.Add(member, interpetedMemberDefinition);
                return interpetedMemberDefinition.Init(member.Key, member.Type);
            }
        }

        public IAssembledOperation AddOperation(IAddOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else {
                var op = new InterpetedAddOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation AssignOperation(IAssignOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedAssignOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation TryAssignOperation(ITryAssignOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedTryAssignOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this),
                    co.Block.Convert(this),
                    new InterpetedScopeTemplate(co.Scope, co.Scope.ToVerifiableType()));
                return op;
            }
        }


        public IAssembledOperation BlockDefinition(IBlockDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedBlockDefinition();
                backing.Add(codeElement, op);
                op.Init(
                    codeElement.Body.Select(x=>x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope, codeElement.Scope.ToVerifiableType()));
                return op;
            }
        }

        public IAssembledOperation ConstantNumber(IConstantNumber codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedConstantNumber();
                backing.Add(codeElement, op);
                op.Init(codeElement.Value);
                return op;
            }
        }

        public IAssembledOperation ConstantString(IConstantString co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedConstantString();
                backing.Add(co, op);
                op.Init(co.Value);
                return op;
            }
        }


        public IAssembledOperation ConstantBool(IConstantBool constantBool)
        {
            if (backing.TryGetValue(constantBool, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedConstantBool();
                backing.Add(constantBool, op);
                op.Init(constantBool.Value);
                return op;
            }
        }


        public IAssembledOperation EmptyInstance(IEmptyInstance co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedEmptyInstance();
                backing.Add(co, op);
                op.Init();
                return op;
            }
        }

        public IAssembledOperation ElseOperation(IElseOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedElseOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation IfTrueOperation(IIfOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedIfTrueOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation ImplementationDefinition(IImplementationDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedImplementationDefinition();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.ParameterDefinition).CastTo<InterpetedMemberDefinition>(),
                    MemberDefinition(codeElement.ContextDefinition).CastTo<InterpetedMemberDefinition>(),
                    codeElement.MethodBody.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope, codeElement.Scope.ToVerifiableType()),
                    codeElement.Returns().CastTo<IMethodType>());
                return op;
            }
        }



        public IAssembledOperation LastCallOperation(ILastCallOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedLastCallOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation LessThanOperation(ILessThanOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedLessThanOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation MemberReferance(IMemberReferance codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedMemberReferance();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.MemberDefinition).CastTo<InterpetedMemberDefinition>());
                return op;
            }
        }



        IAssembledOperation IOpenBoxesContext<IAssembledOperation, InterpetedAssemblyBacking>.EntryPoint(IEntryPointDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedEntryPointDefinition();
                backing.Add(codeElement, op);
                op.Init(
                    codeElement.Body.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope, codeElement.Scope.ToVerifiableType()));
                if (EntryPoint == null)
                {
                    EntryPoint = op;
                }
                else {
                    throw new Exception("entry point already defined");
                }
                return op;
            }
        }


        public IAssembledOperation MethodDefinition(IInternalMethodDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedMethodDefinition();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.ParameterDefinition).CastTo<InterpetedMemberDefinition>(),
                    codeElement.Body.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope, codeElement.Scope.ToVerifiableType()),
                    codeElement.Returns().CastTo<IMethodType>());
                return op;
            }
        }

        public IAssembledOperation ModuleDefinition(IModuleDefinition codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedModuleDefinition();
                backing.Add(codeElement, op);
                op.Init(new InterpetedScopeTemplate(codeElement.Scope, codeElement.Scope.ToVerifiableType()),
                    codeElement.StaticInitialization.Select(x => x.Convert(this)).ToArray(),
                    // yikos yuckos
                    (this as IOpenBoxesContext<IAssembledOperation, InterpetedAssemblyBacking>).EntryPoint(codeElement.EntryPoint).CastTo<InterpetedEntryPointDefinition>()
                    );
                return op;
            }
        }

        public IAssembledOperation MultiplyOperation(IMultiplyOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedMultiplyOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation NextCallOperation(INextCallOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedNextCallOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation ObjectDefinition(IObjectDefiniton codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedObjectDefinition();
                backing.Add(codeElement, op);
                op.Init(new InterpetedScopeTemplate(codeElement.Scope, codeElement.Scope.ToVerifiableType()),
                    codeElement.Assignments.Select(x => AssignOperation(x).CastTo<IInterpetedAssignOperation>()).ToArray()
                    );
                return op;
            }
        }

        public IAssembledOperation PathOperation(IPathOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedPathOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this).CastTo<IInterpetedMemberReferance>());
                return op;
            }
        }


        public IAssembledOperation ReturnOperation(IReturnOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedReturnOperation();
                backing.Add(co, op);
                op.Init(
                    co.Result.Convert(this));
                return op;
            }
        }

        public IAssembledOperation SubtractOperation(ISubtractOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedSubtractOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this),
                    co.Right.Convert(this));
                return op;
            }
        }

        public IAssembledOperation TypeDefinition(IInterfaceType codeElement)
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


        #region Help

        // 
        // I need to use interfaces here
        // espesally for anytime
        // my type system is not that much likes C#'s
        // maybe leaning on C# type system is not a good idea??

        // I need to use IAnyTime no the class
        // so I can assign IIO<Boxeddouble> to IIO<IAny>
        // but that means I wll have to drop the class requirement
        // which is good becasue I don't understand where that is coming form anyway

        // this does not really live here


        // grumble, reflection sucks
        // I get a lot of bugs here
        // need to fix this generics + refection thing
        //private static MethodInfo GetMethod(System.Type[] types, string name)
        //{
        //    var method = typeof(Definitions).GetMethods(BindingFlags.NonPublic|BindingFlags.Instance).Single(x => 
        //    x.Name == name && x.IsGenericMethod);
        //    return method.MakeGenericMethod(types);
        //}

        #endregion

    }

    internal static class TypeMap {

        //public static System.Type MapType(IVerifiableType verifiableType)
        //{
        //    if (verifiableType is INumberType)
        //    {
        //        return typeof(IBoxedDouble);
        //    }
        //    if (verifiableType is IBooleanType)
        //    {
        //        return typeof(IBoxedBool);
        //    }
        //    if (verifiableType is IStringType)
        //    {
        //        return typeof(IBoxedString);
        //    }
        //    if (verifiableType is IBlockType)
        //    {
        //        return typeof(IInterpedEmpty);
        //    }
        //    if (verifiableType is IEmptyType)
        //    {
        //        return typeof(IInterpedEmpty);
        //    }
        //    if (verifiableType is IAnyType)
        //    {
        //        return typeof(IInterpetedAnyType);
        //    }
        //    if (verifiableType is IModuleType || verifiableType is IInterfaceType || verifiableType is IObjectDefiniton)
        //    {
        //        return typeof(IInterpetedScope);
        //    }
        //    if (verifiableType is IMethodType method)
        //    {
        //        return typeof(IInterpetedMethod<,>).MakeGenericType(
        //            MapType(method.InputType),
        //            MapType(method.OutputType)
        //            );
        //    }
        //    if (verifiableType is IMemberReferance memberReferance)
        //    {
        //        return MapType(memberReferance.MemberDefinition.Type);
        //    }
        //    if (verifiableType is ITypeOr typeOr)
        //    {
        //        // we try to find the intersection of the types
        //        return MergeTypes(typeOr.Left, typeOr.Right);
        //    }

        //    throw new NotImplementedException();
        //}

        //private static System.Type MergeTypes(IVerifiableType left, IVerifiableType right)
        //{

        //    var leftType = MapType(left); ;
        //    var rightType = MapType(right);

        //    // if they are the same we are happy
        //    if (leftType == rightType)
        //    {
        //        return leftType;
        //    }

        //    // if they are both methods 
        //    // we have re merge the method io
        //    if (left.TryGetInput().Is(out var leftInput) &&
        //        right.TryGetInput().Is(out var rightInput) &&
        //        left.TryGetReturn().Is(out var leftReturn) &&
        //        right.TryGetReturn().Is(out var rightReturn))
        //    {

        //        return typeof(IInterpetedMethod<,>).MakeGenericType(
        //            MergeTypes(leftInput, rightInput),
        //            MergeTypes(leftReturn, rightReturn));
        //    }

        //    // we really can't merge them
        //    // so call it empty?
        //    return typeof(IInterpedEmpty);
        //}
    }

}
