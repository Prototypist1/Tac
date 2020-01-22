using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Backend.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using static Tac.Backend.Public.AssemblyBuilder;
using Prototypist.Toolbox.Object;

namespace Tac.Backend.Syntaz_Model_Interpeter
{

    internal class Definitions: IOpenBoxesContext<IInterpetedOperation<IInterpetedAnyType>, InterpetedAssemblyBacking>
    {
        private readonly Dictionary<object, IInterpetedOperation<IInterpetedAnyType>> backing = new Dictionary<object, IInterpetedOperation<IInterpetedAnyType>>();

        public InterpetedEntryPointDefinition? entryPoint;
        public InterpetedEntryPointDefinition EntryPoint { get=> entryPoint?? throw new NullReferenceException(nameof(entryPoint)); private set=> entryPoint = value ?? throw new NullReferenceException(nameof(entryPoint)); }

        public Definitions()
        {
        }

        public IInterpetedOperation<IInterpetedAnyType> MemberDefinition(IMemberDefinition member)
        {
            var method = GetMethod(new Type[] { TypeMap.MapType(member.Type) }, nameof(MemberDefinition));
            return method.Invoke(this, new object[] { member }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> MemberDefinition<T>(IMemberDefinition member)
            where T:class, IInterpetedAnyType
        {
            if (backing.TryGetValue(member, out var res))
            {
                return res;
            }
            else
            {
                var interpetedMemberDefinition = new InterpetedMemberDefinition<T>();
                backing.Add(member, interpetedMemberDefinition);
                return interpetedMemberDefinition.Init(member.Key, member.Type);
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> AddOperation(IAddOperation co)
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else {
                var op = new InterpetedAddOperation();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).CastTo<IInterpetedOperation<IBoxedDouble>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<IBoxedDouble>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> AssignOperation(IAssignOperation co)
        {
            var method = GetMethod(new Type[] { TypeMap.MapType(co.Left.Returns()), TypeMap.MapType(co.Right.Returns()) }, nameof(AssignOperation));
            return method.Invoke(this, new object[] { co }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> AssignOperation<TLeft,TRight>(IAssignOperation co)
            where TRight : class, IInterpetedAnyType
            where TLeft: class,TRight
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedAssignOperation<TLeft,TRight>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).CastTo<IInterpetedOperation<TLeft>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<TRight>>());
                return op;
            }
        }


        public IInterpetedOperation<IInterpetedAnyType> TryAssignOperation(ITryAssignOperation co)
        {
            var method = GetMethod(new Type[] { TypeMap.MapType(co.Left.Returns()), TypeMap.MapType(co.Right.Returns()) }, nameof(TryAssignOperation));
            return method.Invoke(this, new object[] { co }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> TryAssignOperation<TLeft, TRight>(ITryAssignOperation co)
            where TRight : class, IInterpetedAnyType
            where TLeft : class, IInterpetedAnyType
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedTryAssignOperation<TLeft, TRight>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).CastTo<IInterpetedOperation<TLeft>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<TRight>>());
                return op;
            }
        }


        public IInterpetedOperation<IInterpetedAnyType> BlockDefinition(IBlockDefinition codeElement)
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

        public IInterpetedOperation<IInterpetedAnyType> ConstantNumber(IConstantNumber codeElement)
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

        public IInterpetedOperation<IInterpetedAnyType> ConstantString(IConstantString co)
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


        public IInterpetedOperation<IInterpetedAnyType> ConstantBool(IConstantBool constantBool)
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


        public IInterpetedOperation<IInterpetedAnyType> EmptyInstance(IEmptyInstance co)
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

        public IInterpetedOperation<IInterpetedAnyType> ElseOperation(IElseOperation co)
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
                    co.Left.Convert(this).CastTo<IInterpetedOperation<IBoxedBool>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<IInterpedEmpty>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> IfTrueOperation(IIfOperation co)
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
                    co.Left.Convert(this).CastTo<IInterpetedOperation<IBoxedBool>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<IInterpedEmpty>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> ImplementationDefinition(IImplementationDefinition codeElement)
        {
            var method = GetMethod(new Type[] { TypeMap.MapType(codeElement.ContextDefinition.Type), TypeMap.MapType(codeElement.ParameterDefinition.Type), TypeMap.MapType(codeElement.OutputType) }, nameof(ImplementationDefinition));
            return method.Invoke(this, new object[] { codeElement }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> ImplementationDefinition<TContext,TIn,TOut>(IImplementationDefinition codeElement)
            where TContext : class, IInterpetedAnyType
            where TIn      : class, IInterpetedAnyType
            where TOut     : class, IInterpetedAnyType
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedImplementationDefinition<TContext, TIn, TOut>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.ParameterDefinition).CastTo<InterpetedMemberDefinition<TIn>>(),
                    MemberDefinition(codeElement.ContextDefinition).CastTo<InterpetedMemberDefinition<TContext>>(),
                    codeElement.MethodBody.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope, codeElement.Scope.ToVerifiableType()),
                    codeElement.Returns().CastTo<IImplementationType>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> LastCallOperation(ILastCallOperation co)
        {
            var methodType = co.Left.Returns().CastTo<IMethodType>();
            var method = GetMethod(new Type[] { TypeMap.MapType(methodType.InputType), TypeMap.MapType(methodType.OutputType) } ,nameof(LastCallOperation));
            return method.Invoke(this, new object[] { co }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> LastCallOperation<TIn,TOut>(ILastCallOperation co)
            where TIn:class, IInterpetedAnyType
            where TOut: class, IInterpetedAnyType
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedLastCallOperation<TIn, TOut>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).CastTo<IInterpetedOperation<IInterpetedCallable<TIn, TOut>>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<TIn>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> LessThanOperation(ILessThanOperation co)
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
                    co.Left.Convert(this).CastTo<IInterpetedOperation<IBoxedDouble>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<IBoxedDouble>>());
                return op;
            }
        }
        
        public IInterpetedOperation<IInterpetedAnyType> MemberReferance(IMemberReferance codeElement)
        {
            var method = GetMethod(new Type[] { TypeMap.MapType(codeElement.MemberDefinition.Type) }, nameof(MemberReferance));
            return method.Invoke(this, new object[] { codeElement }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();

        }

        private IInterpetedOperation<IInterpetedAnyType> MemberReferance<T>(IMemberReferance codeElement)
            where T:class, IInterpetedAnyType
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedMemberReferance<T>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.MemberDefinition).CastTo<InterpetedMemberDefinition<T>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> MethodDefinition(IInternalMethodDefinition codeElement)
        {
            var method = GetMethod(new Type[] { TypeMap.MapType(codeElement.InputType), TypeMap.MapType(codeElement.OutputType) }, nameof(MethodDefinition));
            return method.Invoke(this, new object[] { codeElement }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();

        }


        IInterpetedOperation<IInterpetedAnyType> IOpenBoxesContext<IInterpetedOperation<IInterpetedAnyType>, InterpetedAssemblyBacking>.EntryPoint(IEntryPointDefinition codeElement)
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


        private IInterpetedOperation<IInterpetedAnyType> MethodDefinition<TIn,TOut>(IInternalMethodDefinition codeElement)
            where TIn: class, IInterpetedAnyType
            where TOut: class, IInterpetedAnyType
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedMethodDefinition<TIn, TOut>();
                backing.Add(codeElement, op);
                op.Init(
                    MemberDefinition(codeElement.ParameterDefinition).CastTo<InterpetedMemberDefinition<TIn>>(),
                    codeElement.Body.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope, codeElement.Scope.ToVerifiableType()),
                    codeElement.Returns().CastTo<IMethodType>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> ModuleDefinition(IModuleDefinition codeElement)
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
                    (this as IOpenBoxesContext<IInterpetedOperation<IInterpetedAnyType>, InterpetedAssemblyBacking>).EntryPoint(codeElement.EntryPoint).CastTo<InterpetedEntryPointDefinition>()
                    );
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> MultiplyOperation(IMultiplyOperation co)
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
                    co.Left.Convert(this).CastTo<IInterpetedOperation<IBoxedDouble>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<IBoxedDouble>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> NextCallOperation(INextCallOperation co)
        {
            var methodType = co.Right.Returns().CastTo<IMethodType>();

            var method = GetMethod( new Type[] { TypeMap.MapType(methodType.InputType), TypeMap.MapType(methodType.OutputType) } ,nameof(NextCallOperation));
            return method.Invoke(this, new object[] { co }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> NextCallOperation<TIn,TOut>(INextCallOperation co)
            where TIn: class, IInterpetedAnyType
            where TOut : class, IInterpetedAnyType
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedNextCallOperation<TIn, TOut>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).CastTo<IInterpetedOperation<TIn>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<IInterpetedCallable<TIn, TOut>>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> ObjectDefinition(IObjectDefiniton codeElement)
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
                    codeElement.Assignments.Select(x => AssignOperation(x).CastTo<IInterpetedAssignOperation<IInterpetedAnyType>>()).ToArray()
                    );
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> PathOperation(IPathOperation co)
        {
            var method = GetMethod(new Type[] { TypeMap.MapType(co.Returns()) }, nameof(PathOperation));
            return method.Invoke(this, new object[] { co }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> PathOperation<T>(IPathOperation co)
            where T : class, IInterpetedAnyType
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedPathOperation<T>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).CastTo<IInterpetedOperation<IInterpetedScope>>(),
                    co.Right.Convert(this).CastTo<IInterpetedMemberReferance<T>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> ReturnOperation(IReturnOperation co)
        {
            var method = GetMethod(new Type[] { TypeMap.MapType(co.Result.Returns()) }, nameof(ReturnOperation));
            return method.Invoke(this,new object[] { co }).CastTo<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> ReturnOperation<T>(IReturnOperation co)
            where T:class,  IInterpetedAnyType
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedReturnOperation<T>();
                backing.Add(co, op);
                op.Init(
                    co.Result.Convert(this).CastTo<IInterpetedOperation<T>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> SubtractOperation(ISubtractOperation co)
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
                    co.Left.Convert(this).CastTo<IInterpetedOperation<IBoxedDouble>>(),
                    co.Right.Convert(this).CastTo<IInterpetedOperation<IBoxedDouble>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> TypeDefinition(IInterfaceType codeElement)
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

        // you are here
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
        private static MethodInfo GetMethod(Type[] types, string name)
        {
            var method = typeof(Definitions).GetMethods(BindingFlags.NonPublic|BindingFlags.Instance).Single(x => 
            x.Name == name && x.IsGenericMethod);
            return method.MakeGenericMethod(types);
        }

        #endregion

    }

    internal static class TypeMap {

        public static Type MapType(IVerifiableType verifiableType)
        {
            if (verifiableType is INumberType)
            {
                return typeof(IBoxedDouble);
            }
            if (verifiableType is IBooleanType)
            {
                return typeof(IBoxedBool);
            }
            if (verifiableType is IStringType)
            {
                return typeof(IBoxedString);
            }
            if (verifiableType is IBlockType)
            {
                return typeof(IInterpedEmpty);
            }
            if (verifiableType is IEmptyType)
            {
                return typeof(IInterpedEmpty);
            }
            if (verifiableType is IAnyType)
            {
                return typeof(IInterpetedAnyType);
            }
            if (verifiableType is IModuleType || verifiableType is IInterfaceType || verifiableType is IObjectDefiniton)
            {
                return typeof(IInterpetedScope);
            }
            if (verifiableType is IImplementationType implementation)
            {
                return typeof(IInterpetedImplementation<,,>).MakeGenericType(
                    MapType(implementation.ContextType),
                    MapType(implementation.InputType),
                    MapType(implementation.OutputType)
                    );
            }
            if (verifiableType is IMethodType method)
            {
                return typeof(IInterpetedMethod<,>).MakeGenericType(
                    MapType(method.InputType),
                    MapType(method.OutputType)
                    );
            }
            if (verifiableType is IMemberReferance memberReferance)
            {
                return MapType(memberReferance.MemberDefinition.Type);
            }
            if (verifiableType is ITypeOr) {
                // I am not really sure that is how it works over here...
                // it might just be a any type
                //return typeof(IInterpetedOrType<,>).MakeGenericType(
                //    MapType(orType.Left),
                //    MapType(orType.Right));
                return typeof(IInterpetedAnyType);
            }

            throw new NotImplementedException();
        }

    }

}
