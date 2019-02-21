using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter
{

    internal class Definitions: IOpenBoxesContext<IInterpetedOperation<object>>
    {
        private readonly Dictionary<object, IInterpetedOperation<object>> backing = new Dictionary<object, IInterpetedOperation<object>>();

        public Definitions()
        {
        }

        public IInterpetedOperation<object> MemberDefinition(IMemberDefinition member)
        {
            var method = typeof(Definitions).GetMethod(nameof(MemberDefinition));
            var generic = method.MakeGenericMethod(MapType(member.Type));
            return generic.Invoke(this, new object[] { member }).Cast<IInterpetedOperation<object>>();

        }

        private IInterpetedOperation<object> MemberDefinition<T>(IMemberDefinition member)
            where T: class
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

        public IInterpetedOperation<object> AddOperation(IAddOperation co)
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

        public IInterpetedOperation<object> AssignOperation(IAssignOperation co)
        {
            var method = typeof(Definitions).GetMethod(nameof(AssignOperation));
            var generic = method.MakeGenericMethod(MapType(co.Right.Returns()));
            return generic.Invoke(this, new object[] { co }).Cast<IInterpetedOperation<object>>();

        }

        private IInterpetedOperation<object> AssignOperation<T>(IAssignOperation co)
            where T: class
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

        public IInterpetedOperation<object> BlockDefinition(IBlockDefinition codeElement)
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

        public IInterpetedOperation<object> ConstantNumber(IConstantNumber codeElement)
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

        public IInterpetedOperation<object> ElseOperation(IElseOperation co)
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

        public IInterpetedOperation<object> GenericTypeDefinition(IGenericInterfaceDefinition codeElement)
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

        public IInterpetedOperation<object> IfTrueOperation(IIfOperation co)
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

        public IInterpetedOperation<object> ImplementationDefinition(IImplementationDefinition codeElement)
        {
            var method = typeof(Definitions).GetMethod(nameof(ImplementationDefinition));
            
            var generic = method.MakeGenericMethod(MapType(codeElement.ContextType), MapType(codeElement.InputType), MapType(codeElement.OutputType));
            return generic.Invoke(this, new object[] { codeElement }).Cast<IInterpetedOperation<object>>();

        }

        private IInterpetedOperation<object> ImplementationDefinition<TContext,TIn,TOut>(IImplementationDefinition codeElement)
            where TContext : class
            where TIn      : class
            where TOut     : class
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

        public IInterpetedOperation<object> LastCallOperation(ILastCallOperation co)
        {
            var method = typeof(Definitions).GetMethod(nameof(LastCallOperation));
            var methodType = co.Left.Returns().Cast<IMethodType>();
            var generic = method.MakeGenericMethod(MapType(methodType.InputType), MapType(methodType.OutputType));
            return generic.Invoke(this, new object[] { co }).Cast<IInterpetedOperation<object>>();

        }

        private IInterpetedOperation<object> LastCallOperation<TIn,TOut>(ILastCallOperation co)
            where TIn:class
            where TOut: class
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

        public IInterpetedOperation<object> LessThanOperation(ILessThanOperation co)
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
        
        public IInterpetedOperation<object> MemberReferance(IMemberReferance codeElement)
        {
            var method = typeof(Definitions).GetMethod(nameof(MemberReferance));
            var generic = method.MakeGenericMethod(MapType(codeElement.MemberDefinition.Type));
            return generic.Invoke(this, new object[] { codeElement }).Cast<IInterpetedOperation<object>>();

        }

        private IInterpetedOperation<object> MemberReferance<T>(IMemberReferance codeElement)
            where T:class
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

        public IInterpetedOperation<object> MethodDefinition(IInternalMethodDefinition codeElement)
        {
            var method = typeof(Definitions).GetMethod(nameof(MethodDefinition));
            var generic = method.MakeGenericMethod(MapType(codeElement.InputType), MapType(codeElement.OutputType));
            return generic.Invoke(this, new object[] { codeElement }).Cast<IInterpetedOperation<object>>();

        }

        private IInterpetedOperation<object> MethodDefinition<TIn,TOut>(IInternalMethodDefinition codeElement)
            where TIn: class
            where TOut: class
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

        public IInterpetedOperation<object> ModuleDefinition(IModuleDefinition codeElement)
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

        public IInterpetedOperation<object> MultiplyOperation(IMultiplyOperation co)
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

        public IInterpetedOperation<object> NextCallOperation(INextCallOperation co)
        {
            var method = typeof(Definitions).GetMethod(nameof(NextCallOperation));
            var methodType = co.Right.Returns().Cast<IMethodType>();
            var generic = method.MakeGenericMethod(MapType( methodType.InputType), MapType(methodType.OutputType));
            return generic.Invoke(this, new object[] { co }).Cast<IInterpetedOperation<object>>();
        }

        private IInterpetedOperation<object> NextCallOperation<TIn,TOut>(INextCallOperation co)
            where TIn: class
            where TOut : class
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

        public IInterpetedOperation<object> ObjectDefinition(IObjectDefiniton codeElement)
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
                    codeElement.Assignments.Select(x => AssignOperation(x).Cast<IInterpetedAssignOperation<object>>()).ToArray()
                    );
                return op;
            }
        }

        public IInterpetedOperation<object> PathOperation(IPathOperation co)
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

        public IInterpetedOperation<object> ReturnOperation(IReturnOperation co)
        {
            var method = typeof(Definitions).GetMethod(nameof(ReturnOperation));
            var generic = method.MakeGenericMethod(MapType(co.Result.Returns()));
            return generic.Invoke(this,new object[] { co }).Cast<IInterpetedOperation<object>>();
        }

        private IInterpetedOperation<object> ReturnOperation<T>(IReturnOperation co)
            where T: class
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

        public IInterpetedOperation<object> SubtractOperation(ISubtractOperation co)
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

        public IInterpetedOperation<object> TypeDefinition(IInterfaceType codeElement)
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


        public IInterpetedOperation<object> TypeReferance(ITypeReferance codeElement)
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

        public IInterpetedOperation<object> InternalMethodDefinition(IInternalMethodDefinition codeElement) => MethodDefinition(codeElement);



        private static Type MapType(IVerifiableType verifiableType)
        {
            if (verifiableType is INumberType) {
                return typeof(BoxedDouble);
            }
            if (verifiableType is IBooleanType)
            {
                return typeof(BoxedBool);
            }
            if (verifiableType is IStringType)
            {
                return typeof(string);
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
                return typeof(InterpetedInstanceScope);
            }
            if (verifiableType is IModuleType)
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

            throw new NotImplementedException();
        }

    }
}
