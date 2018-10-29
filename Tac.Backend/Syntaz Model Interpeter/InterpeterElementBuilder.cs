﻿using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedContext
    {
        public readonly IElementBuilders elementBuilders;

        private InterpetedContext(IElementBuilders elementBuilders,IInterpetedScope[] scopes)
        {
            this.elementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }

        public IReadOnlyList<IInterpetedScope> Scopes { get; }

        public InterpetedContext Child(IInterpetedScope scope)
        {
            var scopes = new List<IInterpetedScope> { scope };
            scopes.AddRange(Scopes);
            return new InterpetedContext(elementBuilders,scopes.ToArray());
        }

        public static InterpetedContext Root(IElementBuilders elementBuilders)
        {
            return new InterpetedContext(elementBuilders,new IInterpetedScope[0]);
        }
    }

    public class InterpetedResult
    {
        private InterpetedResult(object value, bool isReturn, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
            IsReturn = isReturn;
        }

        public bool HasValue { get; }
        public bool IsReturn { get; }
        private object Value { get; }

        public T Get<T>()
        {
            if (HasValue)
            {
                throw new Exception($"{nameof(InterpetedResult)} does not have a value");
            }
            return Value.Cast<T>();
        }

        public T GetAndUnwrapMemberWhenNeeded<T>()
        {
            if (HasValue)
            {
                throw new Exception($"{nameof(InterpetedResult)} does not have a value");
            }
            if (Value is InterpetedMember member) {
                return member.Value.Cast<T>();
            }
            return Value.Cast<T>();
        }


        public object GetAndUnwrapMemberWhenNeeded()
        {
            return GetAndUnwrapMemberWhenNeeded<object>();
        }

        public object Get()
        {
            return Get<object>();
        }

        public static InterpetedResult Return(object value)
        {
            return new InterpetedResult(value, true, true);
        }


        public static InterpetedResult Return()
        {
            return new InterpetedResult(null, true, false);
        }


        public static InterpetedResult Create(object value)
        {
            return new InterpetedResult(value, false, true);
        }


        public static InterpetedResult Create()
        {

            return new InterpetedResult(null, false, false);
        }

    }

    public interface IInterpeted
    {
        InterpetedResult Interpet(InterpetedContext interpetedContext);
    }

    public class InterpeterOperationBuilder : IOperationBuilder
    {


        public InterpeterOperationBuilder()
        {
            AddOperation = Add(new Operation<BinaryOperation.Make<WeakAddOperation>>(InterpetedAddOperation.MakeNew, InterpetedAddOperation.Identifier));
            SubtractOperation = Add(new Operation<BinaryOperation.Make<WeakSubtractOperation>>(InterpetedSubtractOperation.MakeNew, InterpetedSubtractOperation.Identifier));
            MultiplyOperation = Add(new Operation<BinaryOperation.Make<WeakMultiplyOperation>>( InterpetedMultiplyOperation.MakeNew, InterpetedMultiplyOperation.Identifier));
            IfTrueOperation = Add(new Operation<BinaryOperation.Make<WeakIfTrueOperation>>( InterpetedIfTrueOperation.MakeNew, InterpetedIfTrueOperation.Identifier));
            ElseOperation = Add(new Operation<BinaryOperation.Make<WeakElseOperation>>( InterpetedElseOperation.MakeNew, InterpetedElseOperation.Identifier));
            LessThanOperation = Add(new Operation<BinaryOperation.Make<WeakLessThanOperation>>( InterpetedLessThanOperation.MakeNew, InterpetedLessThanOperation.Identifier));
            NextCallOperation = Add(new Operation<BinaryOperation.Make<WeakNextCallOperation>>( InterpetedNextCallOperation.MakeNew, InterpetedNextCallOperation.Identifier));
            AssignOperation = Add(new Operation<BinaryOperation.Make<WeakAssignOperation>>( InterpetedAssignOperation.MakeNew, InterpetedAssignOperation.Identifier));
            ReturnOperation = Add(new Operation<TrailingOperation.Make<WeakReturnOperation>>( InterpetedReturnOperation.MakeNew, InterpetedReturnOperation.Identifier));
            PathOperation = Add(new Operation<BinaryOperation.Make<WeakPathOperation>>( InterpetedPathOperation.MakeNew, InterpetedPathOperation.Identifier));
        }

        private Operation<T> Add<T>(Operation<T> operation)
            where T: Delegate
        {
            identifiers.Add(operation.idenifier);
            return operation;
        }

        public Operation<BinaryOperation.Make<WeakAddOperation>> AddOperation { get; }
        public Operation<BinaryOperation.Make<WeakSubtractOperation>> SubtractOperation { get; }
        public Operation<BinaryOperation.Make<WeakMultiplyOperation>> MultiplyOperation { get; }
        public Operation<BinaryOperation.Make<WeakIfTrueOperation>> IfTrueOperation { get; }
        public Operation<BinaryOperation.Make<WeakElseOperation>> ElseOperation { get; }
        public Operation<BinaryOperation.Make<WeakLessThanOperation>> LessThanOperation { get; }
        public Operation<BinaryOperation.Make<WeakNextCallOperation>> NextCallOperation { get; }
        public Operation<BinaryOperation.Make<WeakAssignOperation>> AssignOperation { get; }
        public Operation<BinaryOperation.Make<WeakPathOperation>> PathOperation { get; }
        public Operation<TrailingOperation.Make<WeakReturnOperation>> ReturnOperation { get; }

        private readonly List<string> identifiers = new List<string>();
        public IReadOnlyList<string> Identifiers { get { return identifiers; }  } 
    }

    public class InterpeterElementBuilder : IElementBuilders
    {
        public WeakMemberDefinition.Make MemberDefinition { get; } = InterpetedMemberDefinition.MakeNew;
        public WeakObjectDefinition.Make ObjectDefinition { get; } = InterpetedObjectDefinition.MakeNew;
        public WeakModuleDefinition.Make ModuleDefinition { get; } = InterpetedModuleDefinition.MakeNew;
        public WeakMethodDefinition.Make MethodDefinition { get; } = InterpetedMethodDefinition.MakeNew;
        public WeakTypeDefinition.Make TypeDefinition { get; } = InterpetedTypeDefinition.MakeNew;
        public WeakGenericTypeDefinition.Make GenericTypeDefinition { get; } = InterpetedGenericTypeDefinition.MakeNew;
        public WeakImplementationDefinition.Make ImplementationDefinition { get; } = InterpetedImplementationDefinition.MakeNew;
        public WeakBlockDefinition.Make BlockDefinition { get; } = InterpetedBlockDefinition.MakeNew;
        public WeakConstantNumber.Make ConstantNumber { get; } = InterpetedConstantNumber.MakeNew;
        public WeakMemberReferance.Make MemberReferance { get; } = InterpetedMemberReferance.MakeNew;
        public PrimitiveType.Make NumberType { get; } = ()=> new InterpetedNumberType();
        public PrimitiveType.Make StringType { get; } = () => new InterpetedStringType();
        public PrimitiveType.Make EmptyType { get; } = () => new InterpetedEmptyType();
        public PrimitiveType.Make BooleanType { get; } = () => new InterpetedBooleanType();
        public PrimitiveType.Make AnyType { get; } = () => new InterpetedAnyType();
    }
}