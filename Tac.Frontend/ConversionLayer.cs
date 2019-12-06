using System;
using System.Collections.Generic;
using System.Text;
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

    internal class PlaceholderOrType : ITestType
    {
        public readonly IBox<ITestType> Type1;
        public readonly IBox<ITestType> Type2;
    }
    
    
    internal class PlaceholderValue
    {
        public readonly IBox<ITestType> Type;

        public PlaceholderValue(IBox<ITestType> testType)
        {
            Type = testType ?? throw new ArgumentNullException(nameof(testType));
        }
    }


    internal class TestScopeConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Scope, IResolvableScope>
    {
        public IResolvableScope Convert(Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Scope from)
        {
            var members = typeSolution.GetMembers(from);
            var res = new IResolvableScope();
            foreach (var member in members)
            {
                res.members.Add(typeSolution.GetMemberType(member));
            }
            return res;
        }
    }

    public static class Help {
        public static IResolvableScope GetScope(Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.IHaveMembers haveMembers) { 
        
        }
    }

    internal class WeakTypeDefinitionConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Type, WeakTypeDefinition>
    {
        IIsPossibly<IKey> key;

        public WeakTypeDefinitionConverter(IIsPossibly<IKey> key)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public WeakTypeDefinition Convert(Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Type from)
        {
            return new WeakTypeDefinition(Help.GetScope(from), key);
        }
    }
    internal class WeakMethodDefinitionConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Method, WeakMethodDefinition>
    {
        public WeakMethodDefinition Convert(Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Method from)
        {
            return new WeakMethodDefinition(
                
                
                );
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

        public WeakMemberDefinition Convert(Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Member from)
        {
            var orType = typeSolution.GetType(from);

            IBox<ITestType> testType;
            if (orType.Is1(out var v1))
            {
                testType = typeSolution.GetExplicitTypeType(v1);
            }
            else if (orType.Is2(out var v2))
            {

                testType = typeSolution.GetOrType(v2);
            }
            else
            {
                throw new Exception("well, should have been one of those");
            }
            return new WeakMemberDefinition(isReadonly, nameKey,);
        }
    }
    internal class TestTypeReferenceConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Member, WeakTypeReference>
    {
        public WeakTypeReference Convert(Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Member from)
        {
            var orType = typeSolution.GetType(from);

            IBox<ITestType> testType;
            if (orType.Is1(out var v1))
            {
                testType = typeSolution.GetExplicitTypeType(v1);
            }
            else if (orType.Is2(out var v2))
            {

                testType = typeSolution.GetOrType(v2);
            }
            else
            {
                throw new Exception("well, should have been one of those");
            }
            return new WeakTypeReference(testType);
        }
    }
    internal class TestValueConverter : LocalTpn.IConvertTo<LocalTpn.TypeProblem2.Value, PlaceholderValue>
    {
        public PlaceholderValue Convert(Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution typeSolution, Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Value from)
        {
            var orType = typeSolution.GetType(from);

            IBox<ITestType> testType;
            if (orType.Is1(out var v1))
            {
                testType = typeSolution.GetExplicitTypeType(v1);
            }
            else if (orType.Is2(out var v2))
            {

                testType = typeSolution.GetOrType(v2);
            }
            else
            {
                throw new Exception("well, should have been one of those");
            }
            return new PlaceholderValue(testType);
        }
    }

    internal class LocalTpn : Tpn<IResolvableScope, WeakTypeDefinition, WeakObjectDefinition, PlaceholderOrType, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>
    {
    }
}
