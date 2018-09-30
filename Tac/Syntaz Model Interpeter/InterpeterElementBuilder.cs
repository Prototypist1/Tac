using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedContext {

        private InterpetedContext(IInterpetedScope[] scopes)
        {
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }

        public IReadOnlyList<IInterpetedScope> Scopes { get; }

        public InterpetedContext Child(IInterpetedScope scope) {
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
        private InterpetedResult(object value, bool isReturn, bool hasValue) {
            Value = value;
            HasValue = hasValue;
            IsReturn = isReturn;
        }
        
        public bool HasValue { get; }
        public bool IsReturn { get;  }
        private object Value { get; }

        public T Get<T>()
        {
            if (HasValue)
            {
                throw new Exception($"{nameof(InterpetedResult)} does not have a value");
            }
            return (T)Value;
        }

        public object Get()
        {
            if (HasValue)
            {
                throw new Exception($"{nameof(InterpetedResult)} does not have a value");
            }
            return Value;
        }

        public static InterpetedResult Return(object value) {
            return new InterpetedResult(value,true,true);
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

    public class InterpeterOperationBuilder : IOperationBuilder {
        public InterpeterOperationBuilder()
        {
            AddOperation = Include(Element.BinaryElement("+", (a, b) => new InterpetedAddOperation(a, b)));
            SubtractOperation = Include(Element.BinaryElement("-", (a, b) => new InterpetedSubtractOperation(a, b)));
            MultiplyOperation = Include(Element.BinaryElement("*", (a, b) => new InterpetedMultiplyOperation(a, b)));
            IfTrueOperation = Include(Element.BinaryElement("if", (a, b) => new InterpetedIfTrueOperation(a, b)));
            ElseOperation = Include(Element.BinaryElement("else", (a, b) => new InterpetedElseOperation(a, b)));
            LessThanOperation = Include(Element.BinaryElement("<?", (a, b) => new InterpetedLessThanOperation(a, b)));
            NextCallOperation = Include(Element.BinaryElement(">", (a, b) => new InterpetedNextCallOperation(a, b)));
            AssignOperation = Include(new Element("=:", ElementMatchingContext.MatchAssign((a, b) => new InterpetedAssignOperation(a, b))));
            ReturnOperation = Include(new Element("return", ElementMatchingContext.MatchTrailing("return", x => new InterpetedReturnOperation(x))));
            PathOperation = Include(new Element(".", ElementMatchingContext.MatchPath((a, b) => new InterpetedPathOperation(a, b))));
        }

        private readonly List<Element> _operations = new List<Element>();

        public IReadOnlyList<Element> Operations
        {
            get
            {
                return _operations;
            }
        }

        private T Include<T>(T t) where T : Element
        {
            _operations.Add(t);
            return t;
        }

        public Element AddOperation { get; }
        public Element SubtractOperation { get; }
        public Element MultiplyOperation { get; }
        public Element IfTrueOperation { get; }
        public Element ElseOperation { get; }
        public Element LessThanOperation { get; }
        public Element NextCallOperation { get; }
        public Element AssignOperation { get; }
        public Element ReturnOperation { get; }
        public Element PathOperation { get; }
    }


    public class InterpeterElementBuilder : IElementBuilders
    {
        

        public Func<bool, ExplicitMemberName, ITypeDefinition, MemberDefinition> MemberDefinition { get; } = (readOnly, key, type) => new MemberDefinition(readOnly, key, type);
        public Func<string, ExplicitMemberName> ExplicitMemberName { get; } = (name) => new ExplicitMemberName(name);
        public Func<string, ExplicitTypeName> ExplicitTypeName { get; } = (x) => new ExplicitTypeName(x);
        public Func<string, ITypeDefinition[], GenericNameKey> GenericExplicitTypeName { get; } = (a, b) => new GenericNameKey(a, b);
        public Func<ObjectScope, IReadOnlyList<AssignOperation>, ObjectDefinition> ObjectDefinition { get; } = (a, b) => new  InterpetedObjectDefinition(a, b);
        public Func<StaticScope, IReadOnlyList<AssignOperation>, ModuleDefinition> ModuleDefinition { get; } = (a, b) => new InterpetedModuleDefinition(a, b);
        public Func<ITypeDefinition, MemberDefinition, ICodeElement[], MethodScope, ICodeElement[], MethodDefinition> MethodDefinition { get; } = (a, b, c, d, e) => new InterpetedMethodDefinition(a, b, c, d, e);
        public Func<ObjectScope, TypeDefinition> TypeDefinition { get; } = a => new TypeDefinition(a);
        public Func<NameKey, ObjectScope, NamedTypeDefinition> NamedTypeDefinition { get; } = (a, b) => new NamedTypeDefinition(a, b);
        public Func<NameKey, ObjectScope, GenericTypeParameterDefinition[], GenericTypeDefinition> GenericTypeDefinition { get; } = (a, b, c) => new GenericTypeDefinition(a, b, c);
        public Func<MemberDefinition, ITypeDefinition, MemberDefinition, ICodeElement[], MethodScope, ICodeElement[], ImplementationDefinition> ImplementationDefinition { get; } = (a, b, c, d, e, f) => new InterpetedImplementationDefinition(a, b, c, d, e, f);
        public Func<ICodeElement[], LocalStaticScope, ICodeElement[], BlockDefinition> BlockDefinition { get; } = (a, b, c) => new InterpetedBlockDefinition(a, b, c);
        public Func<double, ConstantNumber> ConstantNumber { get; } = x => new InterpetedConstantNumber(x);
        public Func<int, MemberDefinition, Member> MemberPath { get; } = (x, y) => new InterpetedMemberPath(x, y);

    }
}
