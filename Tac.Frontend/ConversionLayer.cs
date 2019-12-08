using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Semantic_Model;

namespace Tac.Frontend
{

    // Colin! you shit! why don't you just use real stuff!?
    internal interface ITestType
    {

    }

    internal interface IHasScope
    {
        IBox<IResolvableScope> MemberCollection { get; }
    }

    //internal class WeakTypeOrOperation : ITestType
    //{
    //    public readonly IBox<ITestType> Type1;
    //    public readonly IBox<ITestType> Type2;
    //}
    
    
    internal class PlaceholderValue
    {
        public readonly IBox<IFrontendType> Type;

        public PlaceholderValue(IBox<IFrontendType> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }


    internal class TestScopeConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Scope, WeakScope>
    {
        public WeakScope Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Scope from)
        {
            return Help.GetScope(typeSolution, from);
        }
    }

    internal static class Help {
        public static WeakScope GetScope(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.IHaveMembers haveMembers) {

            var members = typeSolution.GetMembers(haveMembers);
            var membersList = new List<IBox<WeakMemberDefinition>>();
            foreach (var member in members)
            {
                membersList.Add(typeSolution.GetMember(member));
            }
            return new WeakScope(membersList);
        }

        public static IBox<IFrontendType> GetType(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ILookUpType lookUpType) {
            var orType = typeSolution.GetType(lookUpType);

            IBox<IFrontendType> testType;
            if (orType.Is1(out var v1))
            {
                testType = typeSolution.GetExplicitType(v1);
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

    internal class WeakTypeDefinitionConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Type, WeakTypeDefinition>
    {
        //IIsPossibly<IKey> key;

        public WeakTypeDefinitionConverter()//IIsPossibly<IKey> key
        {
            //this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public WeakTypeDefinition Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Type from)
        {
            return new WeakTypeDefinition(Help.GetScope(typeSolution,from));//, key
        }
    }


    internal class PrimativeTypeConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Type, IFrontendType>
    {
        private readonly IFrontendType frontendType;

        public PrimativeTypeConverter(IFrontendType frontendType)
        {
            this.frontendType = frontendType ?? throw new ArgumentNullException(nameof(frontendType));
        }

        public IFrontendType Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Type from)
        {
            return frontendType;
        }
    }

    internal class WeakMethodDefinitionConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Method, WeakMethodDefinition>
    {
        private readonly IIsPossibly<IFrontendCodeElement>[] body;
        private readonly bool isEntryPoint;

        public WeakMethodDefinitionConverter(IIsPossibly<IFrontendCodeElement>[] body, bool isEntryPoint)
        {
            this.body = body ?? throw new ArgumentNullException(nameof(body));
            this.isEntryPoint = isEntryPoint;
        }

        public WeakMethodDefinition Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Method from)
        {
            return new WeakMethodDefinition(
                Help.GetType(typeSolution, typeSolution.GetResultType(from)),
                typeSolution.GetMember(typeSolution.GetInputMember(from)),
                body,
                Help.GetScope(typeSolution,from),
                Array.Empty<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>>(),
                isEntryPoint);
        }

        private class MemberTypeBox : IBox<IFrontendType>
        {
            private readonly IBox<WeakMemberDefinition> memberBox;

            public MemberTypeBox(IBox<WeakMemberDefinition> memberBox)
            {
                this.memberBox = memberBox ?? throw new ArgumentNullException(nameof(memberBox));
            }

            public IFrontendType GetValue()=>memberBox.GetValue().Type.GetValue();
            
        }
    }
    internal class WeakMemberDefinitionConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Member, WeakMemberDefinition>
    {
        private bool isReadonly;
        private IKey nameKey;

        public WeakMemberDefinitionConverter(bool isReadonly, IKey nameKey)
        {
            this.isReadonly = isReadonly;
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public WeakMemberDefinition Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Member from)
        {
            return new WeakMemberDefinition(isReadonly, nameKey, Help.GetType(typeSolution,from));
        }
    }
    internal class WeakTypeReferenceConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.TypeReference, WeakTypeReference>
    {
        public WeakTypeReference Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.TypeReference from)
        {
            return new WeakTypeReference(Help.GetType(typeSolution, from));
        }
    }
    internal class WeakTypeOrOperationConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.OrType, WeakTypeOrOperation>
    {
        public WeakTypeOrOperation Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.OrType from)
        {
            var (left,right) = typeSolution.GetOrTypeElements(from);
            return new WeakTypeOrOperation(Help.GetType(typeSolution,left), Help.GetType(typeSolution,right));
        }
    }

    internal class PlaceholderValueConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Value, PlaceholderValue>
    {
        public PlaceholderValue Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Value from)
        {
            return new PlaceholderValue(Help.GetType(typeSolution, from));
        }
    }

    internal class WeakScopeConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Scope, WeakScope>
    {
        public WeakScope Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Scope from)
        {
            return Help.GetScope(typeSolution,from);
        }
    }

    internal class WeakObjectConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Object, WeakObjectDefinition>
    {
        public WeakObjectDefinition Convert(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Object from)
        {
            return new WeakObjectDefinition(Help.GetScope(typeSolution, from),);
        }
    }

    internal class LocalTpn : Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>
    {
    }
}
