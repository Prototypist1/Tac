using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedContext
    {

        private InterpetedContext(IInterpetedScope[] scopes)
        {
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }

        public IReadOnlyList<IInterpetedScope> Scopes { get; }

        public InterpetedContext Child(IInterpetedScope scope)
        {
            var scopes = new List<IInterpetedScope> { scope };
            scopes.AddRange(Scopes);
            return new InterpetedContext(scopes.ToArray());
        }

        public static InterpetedContext Root()
        {
            return new InterpetedContext(new IInterpetedScope[0]);
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
            AddOperation = InterpetedAddOperation.MakeNew;
            SubtractOperation = InterpetedSubtractOperation.MakeNew;
            MultiplyOperation = InterpetedMultiplyOperation.MakeNew;
            IfTrueOperation = InterpetedIfTrueOperation.MakeNew;
            ElseOperation = InterpetedElseOperation.MakeNew;
            LessThanOperation = InterpetedLessThanOperation.MakeNew;
            NextCallOperation = InterpetedNextCallOperation.MakeNew;
            AssignOperation = InterpetedAssignOperation.MakeNew;
            ReturnOperation = InterpetedReturnOperation.MakeNew;
            PathOperation = InterpetedPathOperation.MakeNew;
        }

        public BinaryOperation.Make<AddOperation> AddOperation { get; }
        public BinaryOperation.Make<SubtractOperation> SubtractOperation { get; }
        public BinaryOperation.Make<MultiplyOperation> MultiplyOperation { get; }
        public BinaryOperation.Make<IfTrueOperation> IfTrueOperation { get; }
        public BinaryOperation.Make<ElseOperation> ElseOperation { get; }
        public BinaryOperation.Make<LessThanOperation> LessThanOperation { get; }
        public BinaryOperation.Make<NextCallOperation> NextCallOperation { get; }
        public BinaryOperation.Make<AssignOperation> AssignOperation { get; }
        public BinaryOperation.Make<PathOperation> PathOperation { get; }
        public TrailingOperation.Make<ReturnOperation> ReturnOperation { get; }
    }


    public class InterpeterElementBuilder : IElementBuilders
    {
        public MemberDefinition.Make MemberDefinition { get; } = InterpetedMemberDefinition.MakeNew;
        public ObjectDefinition.Make ObjectDefinition { get; } = InterpetedObjectDefinition.MakeNew;
        public ModuleDefinition.Make ModuleDefinition { get; } = InterpetedModuleDefinition.MakeNew;
        public MethodDefinition.Make MethodDefinition { get; } = InterpetedMethodDefinition.MakeNew;
        public TypeDefinition.Make TypeDefinition { get; } = InterpetedTypeDefinition.MakeNew;
        public GenericTypeDefinition.Make GenericTypeDefinition { get; } = InterpetedGenericTypeDefinition.MakeNew;
        public ImplementationDefinition.Make ImplementationDefinition { get; } = InterpetedImplementationDefinition.MakeNew;
        public BlockDefinition.Make BlockDefinition { get; } = InterpetedBlockDefinition.MakeNew;
        public ConstantNumber.Make ConstantNumber { get; } = InterpetedConstantNumber.MakeNew;
        public MemberReferance.Make PathPart { get; } = InterpetedPathPart.MakeNew;
        public NumberType.Make NumberType { get; } = InterpetedNumberType.MakeNew;
        public StringType.Make StringType { get; } = InterpetedStringType.MakeNew;
        public EmptyType.Make EmptyType { get; } = InterpetedEmptyType.MakeNew;
        public BooleanType.Make BooleanType { get; } = InterpetedBooleanType.MakeNew;
        public AnyType.Make AnyType { get; } = InterpetedAnyType.MakeNew;
    }
}