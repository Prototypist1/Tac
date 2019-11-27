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
        private readonly Dictionary<IKey, IWeakTypeReference> members = new Dictionary<IKey, IWeakTypeReference>();
    }

    // hmmm maybe I need to simplify my model

    class TypeSolutionOverlay
    {
        private readonly ITypeSolution typeSolution;

        private readonly Dictionary<OrSolutionType, IScope> orMap = new Dictionary<OrSolutionType, IScope>();
        private readonly Dictionary<ConcreteSolutionType, IScope> concreteMap = new Dictionary<ConcreteSolutionType, IScope>();


        private IScope Overlay(OrType<OrSolutionType, ConcreteSolutionType> orType) {
            if (orType.Is1(out var orSolutionType))
            {
                if (orMap.TryGetValue(orSolutionType, out var res))
                {
                    return res;
                }

                ... 
            }
            else if (orType.Is2(out var concreteType))
            {
                if (concreteMap.TryGetValue(concreteType, out var res))
                {
                    return res;
                }

                ... 
            }
            else {
                throw new Bug("bug!!");
            }
        }

        public IScope GetExplicitTypeType(Tpn.IExplicitType explicitType)
        {
            return Overlay(typeSolution.GetExplicitTypeType(explicitType));
        }

        public IScope GetMemberType(Tpn.IMember member)
        {
            return Overlay(typeSolution.GetMemberType(member));
        }

        public IScope GetMethodScopeType(Tpn.IMethod method)
        {
            return Overlay(typeSolution.GetMethodScopeType(method));
        }

        public IScope GetObjectType(Tpn.IObject @object)
        {
            return Overlay(typeSolution.GetObjectType(@object));
        }

        public IScope GetOrType(Tpn.IOrType orType)
        {
            return Overlay(typeSolution.GetOrType(orType));
        }

        public IScope GetScopeType(Tpn.IScope scope)
        {
            return Overlay(typeSolution.GetScopeType(scope));
        }

        public IScope GetTypeReferenceType(Tpn.ITypeReference typeReference)
        {
            return Overlay(typeSolution.GetTypeReferenceType(typeReference));
        }

        public IScope GetValueType(Tpn.IValue value)
        {
            return Overlay(typeSolution.GetValueType(value));
        }
    }
}
