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

    public class InterpeterElementBuilder : IElementBuilders
    {
        public InterpeterElementBuilder(){
            AddOperation = Include(new Element<Func<ICodeElement, ICodeElement, AddOperation>>((a, b) => new InterpetedAddOperation(a, b), "+"));
            SubtractOperation = Include(new Element<Func<ICodeElement, ICodeElement, SubtractOperation>>((a, b) => new InterpetedSubtractOperation(a, b), "-"));
            MultiplyOperation = Include(new Element<Func<ICodeElement, ICodeElement, MultiplyOperation>>((a, b) => new InterpetedMultiplyOperation(a, b), "*"));
            IfTrueOperation = Include(new Element<Func<ICodeElement, ICodeElement, IfTrueOperation>>((a, b) => new InterpetedIfTrueOperation(a, b), "if"));
            ElseOperation = Include(new Element<Func<ICodeElement, ICodeElement, ElseOperation>>((a, b) => new InterpetedElseOperation(a, b), "else"));
            LessThanOperation = Include(new Element<Func<ICodeElement, ICodeElement, LessThanOperation>>((a, b) => new InterpetedLessThanOperation(a, b), "<?"));
            NextCallOperation = Include(new Element<Func<ICodeElement, ICodeElement, NextCallOperation>>((a, b) => new InterpetedNextCallOperation(a, b), ">"));
            AssignOperation = Include(new Element<Func<ICodeElement, ICodeElement, AssignOperation>>((a, b) => new InterpetedAssignOperation(a, b), "=:"));
            ReturnOperation = Include(new Element<Func<ICodeElement, ReturnOperation>>(x => new InterpetedReturnOperation(x), "return"));
            PathOperation = Include(new Element<Func<ICodeElement, MemberDefinition, PathOperation>>((a, b) => new InterpetedPathOperation(a, b), "."));
        }

        private readonly List<string> _operations = new List<string>();
        
        public IReadOnlyList<string> Operations
        {
            get
            {
                return _operations;
            }
        }

        private T Include<T>(T t) where T : Element {
            _operations.Add(t.Expressed);
            return t;
        }

        public Element<Func<ICodeElement, ICodeElement, AddOperation>> AddOperation { get; }
        public Element<Func<ICodeElement, ICodeElement, SubtractOperation>> SubtractOperation { get; }
        public Element<Func<ICodeElement, ICodeElement, MultiplyOperation>> MultiplyOperation { get; }
        public Element<Func<ICodeElement, ICodeElement, IfTrueOperation>> IfTrueOperation { get; }
        public Element<Func<ICodeElement, ICodeElement, ElseOperation>> ElseOperation { get; }
        public Element<Func<ICodeElement, ICodeElement, LessThanOperation>> LessThanOperation { get; }
        public Element<Func<ICodeElement, ICodeElement, NextCallOperation>> NextCallOperation { get; }
        public Element<Func<ICodeElement, ICodeElement, AssignOperation>> AssignOperation { get; }
        public Element<Func<ICodeElement, ReturnOperation>> ReturnOperation { get; }
        public Element<Func<ICodeElement, MemberDefinition, PathOperation>> PathOperation { get; } 

        public Func<bool, ExplicitMemberName, ITypeDefinition, MemberDefinition> MemberDefinition { get; } = (readOnly, key, type) => new MemberDefinition(readOnly, key, type);
        public Func<string, ExplicitMemberName> ExplicitMemberName { get; } = (name) => new ExplicitMemberName(name);
        public Func<string, ExplicitTypeName> ExplicitTypeName { get; } = (x) => new ExplicitTypeName(x);
        public Func<string, ITypeDefinition[], GenericExplicitTypeName> GenericExplicitTypeName { get; } = (a, b) => new GenericExplicitTypeName(a, b);
        public Func<ObjectScope, IReadOnlyList<AssignOperation>, ObjectDefinition> ObjectDefinition { get; } = (a, b) => new  InterpetedObjectDefinition(a, b);
        public Func<StaticScope, IReadOnlyList<AssignOperation>, ModuleDefinition> ModuleDefinition { get; } = (a, b) => new InterpetedModuleDefinition(a, b);
        public Func<ITypeDefinition, MemberDefinition, ICodeElement[], MethodScope, ICodeElement[], MethodDefinition> MethodDefinition { get; } = (a, b, c, d, e) => new InterpetedMethodDefinition(a, b, c, d, e);
        public Func<ObjectScope, TypeDefinition> TypeDefinition { get; } = a => new TypeDefinition(a);
        public Func<NameKey, ObjectScope, NamedTypeDefinition> NamedTypeDefinition { get; } = (a, b) => new NamedTypeDefinition(a, b);
        public Func<NameKey, ObjectScope, GenericTypeParameterDefinition[], GenericTypeDefinition> GenericTypeDefinition { get; } = (a, b, c) => new GenericTypeDefinition(a, b, c);
        public Func<MemberDefinition, ITypeDefinition, MemberDefinition, ICodeElement[], MethodScope, ICodeElement[], ImplementationDefinition> ImplementationDefinition { get; } = (a, b, c, d, e, f) => new InterpetedImplementationDefinition(a, b, c, d, e, f);
        public Func<ICodeElement[], LocalStaticScope, ICodeElement[], BlockDefinition> BlockDefinition { get; } = (a, b, c) => new InterpetedBlockDefinition(a, b, c);
        public Func<double, ConstantNumber> ConstantNumber { get; } = x => new InterpetedConstantNumber(x);
        public Func<int, MemberDefinition, MemberPath> MemberPath { get; } = (x, y) => new InterpetedMemberPath(x, y);

    }
}
