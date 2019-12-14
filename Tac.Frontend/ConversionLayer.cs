using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.New;
using Tac.Semantic_Model;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend
{
    internal class PlaceholderValue
    {
        public readonly IBox<IFrontendType> Type;

        public PlaceholderValue(IBox<IFrontendType> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }


    internal class TestScopeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Scope, WeakScope>
    {
        public WeakScope Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
        {
            return Help.GetScope(typeSolution, from);
        }
    }

    internal static class Help
    {
        public static WeakScope GetScope(Tpn.ITypeSolution typeSolution, Tpn.IHaveMembers haveMembers)
        {

            var members = typeSolution.GetMembers(haveMembers);
            var membersList = new List<IBox<WeakMemberDefinition>>();
            foreach (var member in members)
            {
                membersList.Add(typeSolution.GetMember(member));
            }
            return new WeakScope(membersList);
        }

        public static IBox<IFrontendType> GetType(Tpn.ITypeSolution typeSolution, Tpn.ILookUpType lookUpType)
        {
            var orType = typeSolution.GetType(lookUpType);

            IBox<IFrontendType> testType;
            if (orType.Is1(out var v1))
            {
                var inner = typeSolution.GetExplicitType(v1).GetValue();
                if (inner.Is1(out var inner1))
                {
                    testType = new Box<IFrontendType>(inner1);
                }
                else if (inner.Is1(out var inner2))
                {
                    testType = new Box<IFrontendType>(inner2);
                }
                else
                {
                    throw new Exception("wish there was a clearner way to do this");
                }
            }
            else if (orType.Is2(out var v2))
            {

                testType = typeSolution.GetOrType(v2);
            }
            else
            {
                throw new Exception("well, should have been one of those");
            }
            return testType;
        }
    }

    internal class WeakTypeDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition>>
    {
        //IIsPossibly<IKey> key;

        public WeakTypeDefinitionConverter()//IIsPossibly<IKey> key
        {
            //this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public OrType<WeakTypeDefinition, WeakGenericTypeDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            return new OrType<WeakTypeDefinition, WeakGenericTypeDefinition>(new WeakTypeDefinition(new Box<WeakScope>(Help.GetScope(typeSolution, from))));//, key
        }
    }

    internal class WeakGenericTypeDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition>>
    {

        private readonly NameKey key;
        private readonly SyntaxModel.Elements.AtomicTypes.PrimitiveTypes.IGenericTypeParameterPlacholder[] TypeParameterDefinitions;

        public WeakGenericTypeDefinitionConverter(NameKey key, PrimitiveTypes.IGenericTypeParameterPlacholder[] typeParameterDefinitions)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
        }

        public OrType<WeakTypeDefinition, WeakGenericTypeDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            return new OrType<WeakTypeDefinition, WeakGenericTypeDefinition>(
                new WeakGenericTypeDefinition(
                    Possibly.Is(key),
                    new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                    TypeParameterDefinitions.Select(x => Possibly.Is(x)).ToArray()));//, key
        }
    }

    internal class PrimativeTypeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Type, IFrontendType>
    {
        private readonly IFrontendType frontendType;

        public PrimativeTypeConverter(IFrontendType frontendType)
        {
            this.frontendType = frontendType ?? throw new ArgumentNullException(nameof(frontendType));
        }

        public IFrontendType Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Type from)
        {
            return frontendType;
        }
    }

    internal class WeakMethodDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>>
    {
        private readonly IBox<IResolve<IFrontendCodeElement>[]> body;
        private readonly bool isEntryPoint;

        public WeakMethodDefinitionConverter(IBox<IResolve<IFrontendCodeElement>[]> body, bool isEntryPoint)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
            this.isEntryPoint = isEntryPoint;
        }

        public OrType<WeakMethodDefinition, WeakImplementationDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {
            return new OrType<WeakMethodDefinition, WeakImplementationDefinition>( new WeakMethodDefinition(
                Help.GetType(typeSolution, typeSolution.GetResultType(from)),
                typeSolution.GetMember(typeSolution.GetInputMember(from)),
                body.GetValue().Select(x => x.Run(typeSolution)).ToArray(),
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                Array.Empty<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>>(),
                isEntryPoint));
        }

        
    }

    //internal class MemberTypeBox : IBox<IFrontendType>
    //{
    //    private readonly IBox<WeakMemberDefinition> memberBox;

    //    public MemberTypeBox(IBox<WeakMemberDefinition> memberBox)
    //    {
    //        this.memberBox = memberBox ?? throw new ArgumentNullException(nameof(memberBox));
    //    }

    //    public IFrontendType GetValue() => memberBox.GetValue().Type.GetValue();

    //}

    internal class WeakImplementationDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>>
    {

        private readonly IBox<IResolve<IFrontendCodeElement>[]> body;
        private readonly 
            IBox<
                Tpn.TypeProblem2.Method
            > inner;

        public WeakImplementationDefinitionConverter(IBox<IResolve<IFrontendCodeElement>[]> body, IBox<Tpn.TypeProblem2.Method> inner)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public OrType<WeakMethodDefinition, WeakImplementationDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Method from)
        {
            return new OrType<WeakMethodDefinition, WeakImplementationDefinition>(new WeakImplementationDefinition(
                typeSolution.GetMember(typeSolution.GetInputMember(from)),
                typeSolution.GetMember(typeSolution.GetInputMember(inner.GetValue())),
                Help.GetType(typeSolution, typeSolution.GetResultType(inner.GetValue())),
                body.GetValue().Select(x => x.Run(typeSolution)).ToArray(),
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                Array.Empty<IFrontendCodeElement>()));
        }

    }


    internal class WeakMemberDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Member, WeakMemberDefinition>
    {
        private readonly bool isReadonly;
        private readonly IKey nameKey;

        public WeakMemberDefinitionConverter(bool isReadonly, IKey nameKey)
        {
            this.isReadonly = isReadonly;
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public WeakMemberDefinition Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Member from)
        {
            return new WeakMemberDefinition(isReadonly, nameKey, Help.GetType(typeSolution, from));
        }
    }
    internal class WeakTypeReferenceConverter : Tpn.IConvertTo<Tpn.TypeProblem2.TypeReference, IFrontendType>
    {
        public IFrontendType Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.TypeReference from)
        {
            return Help.GetType(typeSolution, from).GetValue();
        }
    }
    internal class WeakTypeOrOperationConverter : Tpn.IConvertTo<Tpn.TypeProblem2.OrType, WeakTypeOrOperation>
    {
        public WeakTypeOrOperation Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.OrType from)
        {
            var (left, right) = typeSolution.GetOrTypeElements(from);
            return new WeakTypeOrOperation(Help.GetType(typeSolution, left), Help.GetType(typeSolution, right));
        }
    }

    internal class PlaceholderValueConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Value, PlaceholderValue>
    {
        public PlaceholderValue Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Value from)
        {
            return new PlaceholderValue(Help.GetType(typeSolution, from));
        }
    }

    internal class WeakBlockDefinitionConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Scope, OrType<WeakBlockDefinition, WeakScope>>
    {

        private readonly IBox<IResolve<IFrontendCodeElement>[]> body;

        public WeakBlockDefinitionConverter(IBox<IResolve<IFrontendCodeElement>[]> body)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public OrType<WeakBlockDefinition, WeakScope> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
        {
            return new OrType<WeakBlockDefinition, WeakScope>(
                new WeakBlockDefinition(
                    body.GetValue().Select(x => x.Run(typeSolution)).ToArray(),
                    new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                    Array.Empty<IIsPossibly<IFrontendCodeElement>>()));
        }
    }

    internal class WeakScopeConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Scope, OrType<WeakBlockDefinition, WeakScope>>
    {
        public WeakScopeConverter()
        {
        }

        public OrType<WeakBlockDefinition, WeakScope> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Scope from)
        {
            return new OrType<WeakBlockDefinition, WeakScope>(Help.GetScope(typeSolution, from));
        }
    }

    internal class WeakObjectConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Object, OrType<WeakObjectDefinition, WeakModuleDefinition>>
    {
        private readonly Box<IResolve<IFrontendCodeElement>[]> box;

        public WeakObjectConverter(Box<IResolve<IFrontendCodeElement>[]> box)
        {
            this.box = box;
        }

        public OrType<WeakObjectDefinition, WeakModuleDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Object from)
        {
            return new OrType<WeakObjectDefinition, WeakModuleDefinition>(new WeakObjectDefinition(
                new Box<WeakScope>(Help.GetScope(typeSolution, from)), 
                box.GetValue().Select(x=>x.Run(typeSolution)).ToArray()));
        }
    }

    internal class WeakModuleConverter : Tpn.IConvertTo<Tpn.TypeProblem2.Object, OrType<WeakObjectDefinition, WeakModuleDefinition>>
    {
        private readonly Box<IResolve<IFrontendCodeElement>[]> box;
        private readonly IKey key;

        public WeakModuleConverter(Box<IResolve<IFrontendCodeElement>[]> box, IKey key)
        {
            this.box = box;
            this.key = key;
        }

        public OrType<WeakObjectDefinition, WeakModuleDefinition> Convert(Tpn.ITypeSolution typeSolution, Tpn.TypeProblem2.Object from)
        {
            return new OrType<WeakObjectDefinition, WeakModuleDefinition>(new WeakModuleDefinition(
                new Box<WeakScope>(Help.GetScope(typeSolution, from)),
                box.GetValue().Select(x => x.Run(typeSolution)).ToArray(), 
                key));
        }
    }

}
