using Prototypist.LeftToRight;
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
            AddOperation = Add(new Operation<BinaryOperation.Make<AddOperation>>(InterpetedAddOperation.MakeNew, InterpetedAddOperation.Identifier));
            SubtractOperation = Add(new Operation<BinaryOperation.Make<SubtractOperation>>(InterpetedSubtractOperation.MakeNew, InterpetedSubtractOperation.Identifier));
            MultiplyOperation = Add(new Operation<BinaryOperation.Make<MultiplyOperation>>( InterpetedMultiplyOperation.MakeNew, InterpetedMultiplyOperation.Identifier));
            IfTrueOperation = Add(new Operation<BinaryOperation.Make<IfTrueOperation>>( InterpetedIfTrueOperation.MakeNew, InterpetedIfTrueOperation.Identifier));
            ElseOperation = Add(new Operation<BinaryOperation.Make<ElseOperation>>( InterpetedElseOperation.MakeNew, InterpetedElseOperation.Identifier));
            LessThanOperation = Add(new Operation<BinaryOperation.Make<LessThanOperation>>( InterpetedLessThanOperation.MakeNew, InterpetedLessThanOperation.Identifier));
            NextCallOperation = Add(new Operation<BinaryOperation.Make<NextCallOperation>>( InterpetedNextCallOperation.MakeNew, InterpetedNextCallOperation.Identifier));
            AssignOperation = Add(new Operation<BinaryOperation.Make<AssignOperation>>( InterpetedAssignOperation.MakeNew, InterpetedAssignOperation.Identifier));
            ReturnOperation = Add(new Operation<TrailingOperation.Make<ReturnOperation>>( InterpetedReturnOperation.MakeNew, InterpetedReturnOperation.Identifier));
            PathOperation = Add(new Operation<BinaryOperation.Make<PathOperation>>( InterpetedPathOperation.MakeNew, InterpetedPathOperation.Identifier));
        }

        private Operation<T> Add<T>(Operation<T> operation)
            where T: Delegate
        {
            identifiers.Add(operation.idenifier);
            return operation;
        }

        public Operation<BinaryOperation.Make<AddOperation>> AddOperation { get; }
        public Operation<BinaryOperation.Make<SubtractOperation>> SubtractOperation { get; }
        public Operation<BinaryOperation.Make<MultiplyOperation>> MultiplyOperation { get; }
        public Operation<BinaryOperation.Make<IfTrueOperation>> IfTrueOperation { get; }
        public Operation<BinaryOperation.Make<ElseOperation>> ElseOperation { get; }
        public Operation<BinaryOperation.Make<LessThanOperation>> LessThanOperation { get; }
        public Operation<BinaryOperation.Make<NextCallOperation>> NextCallOperation { get; }
        public Operation<BinaryOperation.Make<AssignOperation>> AssignOperation { get; }
        public Operation<BinaryOperation.Make<PathOperation>> PathOperation { get; }
        public Operation<TrailingOperation.Make<ReturnOperation>> ReturnOperation { get; }

        private readonly List<string> identifiers = new List<string>();
        public IReadOnlyList<string> Identifiers { get { return identifiers; }  } 
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
        public MemberReferance.Make PathPart { get; } = InterpetedMemberReferance.MakeNew;
        public PrimitiveType.Make NumberType { get; } = ()=> new InterpetedNumberType();
        public PrimitiveType.Make StringType { get; } = () => new InterpetedStringType();
        public PrimitiveType.Make EmptyType { get; } = () => new InterpetedEmptyType();
        public PrimitiveType.Make BooleanType { get; } = () => new InterpetedBooleanType();
        public PrimitiveType.Make AnyType { get; } = () => new InterpetedAnyType();
    }
}