using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter
{

    internal class Definitions: IOpenBoxesContext<IInterpetedOperation<IInterpetedAnyType>>
    {
        private readonly Dictionary<object, IInterpetedOperation<IInterpetedAnyType>> backing = new Dictionary<object, IInterpetedOperation<IInterpetedAnyType>>();

        public Definitions()
        {
        }

        public IInterpetedOperation<IInterpetedAnyType> MemberDefinition(IMemberDefinition member)
        {
            var method = GetMethod(new Type[] { MapType(member.Type) }, nameof(MemberDefinition));
            return method.Invoke(this, new object[] { member }).Cast<IInterpetedOperation<IInterpetedAnyType>>();
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
                return interpetedMemberDefinition.Init(member.Key);
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<BoxedDouble>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<BoxedDouble>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> AssignOperation(IAssignOperation co)
        {
            var method = GetMethod(new Type[] { MapType(co.Right.Returns()) }, nameof(AssignOperation));
            return method.Invoke(this, new object[] { co }).Cast<IInterpetedOperation<IInterpetedAnyType>>();
        }

        private IInterpetedOperation<IInterpetedAnyType> AssignOperation<T>(IAssignOperation co)
            where T : class, IInterpetedAnyType
        {
            if (backing.TryGetValue(co, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedAssignOperation<T>();
                backing.Add(co, op);
                op.Init(
                    co.Left.Convert(this).Cast<IInterpetedOperation<T>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<T>>());
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
                    new InterpetedScopeTemplate(codeElement.Scope));
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<BoxedBool>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<IInterpedEmpty>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> GenericTypeDefinition(IGenericInterfaceDefinition codeElement)
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<BoxedBool>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<IInterpedEmpty>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> ImplementationDefinition(IImplementationDefinition codeElement)
        {
            var method = GetMethod(new Type[] { MapType(codeElement.ContextType), MapType(codeElement.InputType), MapType(codeElement.OutputType) }, nameof(ImplementationDefinition));
            return method.Invoke(this, new object[] { codeElement }).Cast<IInterpetedOperation<IInterpetedAnyType>>();
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
                    MemberDefinition(codeElement.ParameterDefinition).Cast<InterpetedMemberDefinition<TIn>>(),
                    MemberDefinition(codeElement.ContextDefinition).Cast<InterpetedMemberDefinition<TContext>>(),
                    codeElement.MethodBody.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope));
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> LastCallOperation(ILastCallOperation co)
        {
            var methodType = co.Left.Returns().Cast<IMethodType>();
            var method = GetMethod(new Type[] { MapType(methodType.InputType), MapType(methodType.OutputType) } ,nameof(LastCallOperation));
            return method.Invoke(this, new object[] { co }).Cast<IInterpetedOperation<IInterpetedAnyType>>();
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<IInterpetedCallable<TIn, TOut>>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<TIn>>());
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<BoxedDouble>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<BoxedDouble>>());
                return op;
            }
        }
        
        public IInterpetedOperation<IInterpetedAnyType> MemberReferance(IMemberReferance codeElement)
        {
            var method = GetMethod(new Type[] { MapType(codeElement.MemberDefinition.Type) }, nameof(MemberReferance));
            return method.Invoke(this, new object[] { codeElement }).Cast<IInterpetedOperation<IInterpetedAnyType>>();

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
                    MemberDefinition(codeElement.MemberDefinition).Cast<InterpetedMemberDefinition<T>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> MethodDefinition(IInternalMethodDefinition codeElement)
        {
            var method = GetMethod(new Type[] { MapType(codeElement.InputType), MapType(codeElement.OutputType) }, nameof(MethodDefinition));
            return method.Invoke(this, new object[] { codeElement }).Cast<IInterpetedOperation<IInterpetedAnyType>>();

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
                    MemberDefinition(codeElement.ParameterDefinition).Cast<InterpetedMemberDefinition<TIn>>(),
                    codeElement.Body.Select(x => x.Convert(this)).ToArray(),
                    new InterpetedScopeTemplate(codeElement.Scope));
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
                op.Init(new InterpetedScopeTemplate(codeElement.Scope),
                    codeElement.StaticInitialization.Select(x => x.Convert(this)).ToArray()
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<BoxedDouble>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<BoxedDouble>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> NextCallOperation(INextCallOperation co)
        {
            var methodType = co.Right.Returns().Cast<IMethodType>();

            var method = GetMethod( new Type[] { MapType(methodType.InputType), MapType(methodType.OutputType) } ,nameof(NextCallOperation));
            return method.Invoke(this, new object[] { co }).Cast<IInterpetedOperation<IInterpetedAnyType>>();
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<TIn>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<IInterpetedCallable<TIn, TOut>>>());
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
                op.Init(new InterpetedScopeTemplate(codeElement.Scope),
                    codeElement.Assignments.Select(x => AssignOperation(x).Cast<IInterpetedAssignOperation<IInterpetedAnyType>>()).ToArray()
                    );
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> PathOperation(IPathOperation co)
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<IInterpetedScope>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<IInterpetedMemberReferance>>());
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> ReturnOperation(IReturnOperation co)
        {
            var method = GetMethod(new Type[] { MapType(co.Result.Returns()) }, nameof(ReturnOperation));
            return method.Invoke(this,new object[] { co }).Cast<IInterpetedOperation<IInterpetedAnyType>>();
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
                    co.Result.Convert(this).Cast<IInterpetedOperation<T>>());
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
                    co.Left.Convert(this).Cast<IInterpetedOperation<BoxedDouble>>(),
                    co.Right.Convert(this).Cast<IInterpetedOperation<BoxedDouble>>());
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

        public IInterpetedOperation<IInterpetedAnyType> TypeReferance(ITypeReferance codeElement)
        {
            if (backing.TryGetValue(codeElement, out var res))
            {
                return res;
            }
            else
            {
                var op = new InterpetedTypeReferance();
                backing.Add(codeElement, op);
                op.Init(codeElement.TypeDefinition);
                return op;
            }
        }

        public IInterpetedOperation<IInterpetedAnyType> InternalMethodDefinition(IInternalMethodDefinition codeElement) => MethodDefinition(codeElement);

        #region Help

        // you are here
        // I need to use interfaces here
        // espesally for anytime
        // my type system is not hat mcuh likes C#
        // maybe leaning on C# type system is not a good idea??
        
        // I need to use IAnyTime no the class
        // so I can assign IIO<Boxeddouble> to IIO<IAny>
        // but that means I wll have to drop the class requirement
        // which is good becasue I don't understand where that is coming form anyway

        public static Type MapType(IVerifiableType verifiableType)
        {
            if (verifiableType is INumberType)
            {
                return typeof(BoxedDouble);
            }
            if (verifiableType is IBooleanType)
            {
                return typeof(BoxedBool);
            }
            if (verifiableType is IStringType)
            {
                return typeof(BoxedString);
            }
            if (verifiableType is IBlockType)
            {
                return typeof(RunTimeEmpty);
            }
            if (verifiableType is IEmptyType)
            {
                return typeof(RunTimeEmpty);
            }
            if (verifiableType is IAnyType)
            {
                return typeof(IInterpetedAnyType);
            }
            if (verifiableType is IModuleType || verifiableType is IInterfaceType)
            {
                return typeof(InterpetedInstanceScope);
            }
            if (verifiableType is IMethodType method)
            {
                return typeof(IInterpetedMethod<,>).MakeGenericType(
                    MapType(method.InputType),
                    MapType(method.OutputType)
                    );
            }
            if (verifiableType is IGenericMethodType)
            {
                throw new NotImplementedException();
            }
            if (verifiableType is IImplementationType implementation)
            {
                return typeof(IInterpetedImplementation<,,>).MakeGenericType(
                    MapType(implementation.ContextType),
                    MapType(implementation.InputType),
                    MapType(implementation.OutputType)
                    );
            }
            if (verifiableType is IGenericImplementationType)
            {
                throw new NotImplementedException();
            }
            if (verifiableType is IMemberReferance memberReferance)
            {
                return MapType(memberReferance.MemberDefinition.Type);
            }
            if (verifiableType is ITypeReferance typeReferance)
            {
                return MapType(typeReferance.TypeDefinition);
            }
            if (verifiableType is IGenericInterfaceDefinition) {
                return typeof(RunTimeType);
            }

            throw new NotImplementedException();
        }

        private static MethodInfo GetMethod(Type[] types, string name)
        {
            var method = typeof(Definitions).GetMethods(BindingFlags.NonPublic|BindingFlags.Instance).Single(x => 
            x.Name == name && x.IsGenericMethod);
            return method.MakeGenericMethod(types);
        }


        #endregion


    }
}
