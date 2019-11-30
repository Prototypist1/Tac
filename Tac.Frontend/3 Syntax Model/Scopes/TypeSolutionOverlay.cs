using System;
using System.Collections.Generic;
using System.Text;
using Prototypist.Fluent;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Semantic_Model;

namespace Tac.Frontend._3_Syntax_Model.Scopes
{
    interface IScope { }

    class Scope {
        private readonly Dictionary<IKey, IBox<IWeakMemberDefinition>> members = new Dictionary<IKey, IBox<IWeakMemberDefinition>>();
    }

    // hmmm maybe I need to simplify my model

    class TypeSolutionOverlay
    {
        private readonly ITypeSolution typeSolution;

        private readonly IReadOnlyDictionary<OrSolutionType, IScope> orMap;
        private readonly IReadOnlyDictionary<ConcreteSolutionType, IScope> concreteMap;

        public TypeSolutionOverlay(ITypeSolution typeSolution)
        {
            this.typeSolution = typeSolution ?? throw new ArgumentNullException(nameof(typeSolution));
            var orMapToBe = new Dictionary<OrSolutionType, IScope>();
            var concreteMapToBe = new  Dictionary<ConcreteSolutionType, IScope>();

            foreach (var item in typeSolution.Types())
            {
                if (item.Is1(out var orSolutionType))
                {
                    orMapToBe[orSolutionType] = Convert(orSolutionType);
                }
                else if (item.Is2(out var concreteType))
                {
                    concreteMapToBe[concreteType] = Convert(concreteType);
                }
            }

            orMap = orMapToBe;
            concreteMap = concreteMapToBe;
        }

        private IScope Convert(OrSolutionType concreteType)
        {
            throw new NotImplementedException();
        }


        private IScope Convert(ConcreteSolutionType orSolutionType)
        {
            throw new NotImplementedException();
        }

        private IScope LookUp(OrType<OrSolutionType, ConcreteSolutionType> orType) {
            if (orType.Is1(out var orSolutionType))
            {
                if (orMap.TryGetValue(orSolutionType, out var res))
                {
                    return res;
                }
                throw new Bug("bug!!");
            }
            else if (orType.Is2(out var concreteType))
            {
                if (concreteMap.TryGetValue(concreteType, out var res))
                {
                    return res;
                }
                throw new Bug("bug!!");
            }
            else {
                throw new Bug("bug!!");
            }
        }

        public IScope GetExplicitTypeType(Tpn.IExplicitType explicitType)
        {
            return LookUp(typeSolution.GetExplicitTypeType(explicitType));
        }

        public IScope GetMemberType(Tpn.IMember member)
        {
            return LookUp(typeSolution.GetMemberType(member));
        }

        public IScope GetMethodScopeType(Tpn.IMethod method)
        {
            return LookUp(typeSolution.GetMethodScopeType(method));
        }

        public IScope GetObjectType(Tpn.IObject @object)
        {
            return LookUp(typeSolution.GetObjectType(@object));
        }

        public IScope GetOrType(Tpn.IOrType orType)
        {
            return LookUp(typeSolution.GetOrType(orType));
        }

        public IScope GetScopeType(Tpn.IScope scope)
        {
            return LookUp(typeSolution.GetScopeType(scope));
        }

        public IScope GetTypeReferenceType(Tpn.ITypeReference typeReference)
        {
            return LookUp(typeSolution.GetTypeReferenceType(typeReference));
        }

        public IScope GetValueType(Tpn.IValue value)
        {
            return LookUp(typeSolution.GetValueType(value));
        }
    }
}
