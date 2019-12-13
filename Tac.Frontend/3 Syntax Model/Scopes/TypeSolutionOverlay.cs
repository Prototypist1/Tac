//using System;
//using System.Collections.Generic;
//using System.Text;
//using Prototypist.Toolbox;
//using Tac.Frontend.New.CrzayNamespace;
//using Tac.Model;
//using Tac.Semantic_Model;

//namespace Tac.Frontend._3_Syntax_Model.Scopes
//{
//    interface IScope { }

//    class Scope {
//        private readonly IReadOnlyDictionary<IKey, IBox<IWeakMemberDefinition>> members;

//        public Scope(IReadOnlyDictionary<IKey, IBox<IWeakMemberDefinition>> members)
//        {
//            this.members = members ?? throw new ArgumentNullException(nameof(members));
//        }
//    }

//    // hmmm maybe I need to simplify my model

//    class TypeSolutionOverlay
//    {
//        private readonly ITypeSolution typeSolution;

//        private readonly IReadOnlyDictionary<OrSolutionType, IFrontendType> orMap;
//        private readonly IReadOnlyDictionary<ConcreteSolutionType, IFrontendType> concreteMap;

//        public TypeSolutionOverlay(ITypeSolution typeSolution)
//        {
//            this.typeSolution = typeSolution ?? throw new ArgumentNullException(nameof(typeSolution));
//            var orMapToBe = new Dictionary<OrSolutionType, IBox<IFrontendType>>();
//            var concreteMapToBe = new  Dictionary<ConcreteSolutionType, IBox<IFrontendType>>();

//            foreach (var item in typeSolution.Types())
//            {
//                Convert(orMapToBe, concreteMapToBe, item);
//            }

//            orMap = orMapToBe;
//            concreteMap = concreteMapToBe;
//        }

//        private IBox<IFrontendType> Convert(Dictionary<OrSolutionType, IBox<IFrontendType>> orMapToBe, Dictionary<ConcreteSolutionType, IBox<IFrontendType>> concreteMapToBe, OrType<OrSolutionType, ConcreteSolutionType> item)
//        {
//            if (item.Is1(out var orSolutionType))
//            {
//                orMapToBe[orSolutionType] = Convert(orMapToBe, concreteMapToBe, orSolutionType);
//                return orMapToBe[orSolutionType];
//            }
//            else if (item.Is2(out var concreteType))
//            {
//                concreteMapToBe[concreteType] = Convert(orMapToBe, concreteMapToBe, concreteType);
//                return orMapToBe[orSolutionType];
//            }
//            throw new Bug();
//        }

//        private IBox<IFrontendType> Convert(Dictionary<OrSolutionType, IBox<IFrontendType>> orMapToBe, Dictionary<ConcreteSolutionType, IBox<IFrontendType>> concreteMapToBe, OrSolutionType orSolutionType )
//        {
//            throw new NotImplementedException();
//        }


//        private IBox<IFrontendType> Convert(Dictionary<OrSolutionType, IBox<IFrontendType>> orMapToBe, Dictionary<ConcreteSolutionType, IBox<IFrontendType>> concreteMapToBe, ConcreteSolutionType concreteType)
//        {
//            {
//                if (concreteMapToBe.TryGetValue(concreteType, out var res)) {
//                    return res;
//                }
//            }

//            var resBox = new Box<IFrontendType>();
//            concreteMapToBe[concreteType] = resBox;

//            var dict = new Dictionary<IKey, IBox<IWeakMemberDefinition>>();
//            foreach (var item in concreteType)
//            {
//                var box = new Box<IWeakMemberDefinition>();
//                dict.Add(item.Key, box);
//                box.Fill(new WeakMemberDefinition(item.Value.Item1, item.Key, Convert(orMapToBe, concreteMapToBe, item.Value.Item2)));
//            }

//            {
//                var res = new WeakTypeDefinition( new Scope(dict),Possibly.Is<IKey>());
//                resBox.Fill(res);
//                return res;
//            }
//        }

//        private IFrontendType LookUp(OrType<OrSolutionType, ConcreteSolutionType> orType) {
//            if (orType.Is1(out var orSolutionType))
//            {
//                if (orMap.TryGetValue(orSolutionType, out var res))
//                {
//                    return res;
//                }
//                throw new Bug("bug!!");
//            }
//            else if (orType.Is2(out var concreteType))
//            {
//                if (concreteMap.TryGetValue(concreteType, out var res))
//                {
//                    return res;
//                }
//                throw new Bug("bug!!");
//            }
//            else {
//                throw new Bug("bug!!");
//            }
//        }

//        public IFrontendType GetExplicitTypeType(Tpn.IExplicitType explicitType)
//        {
//            return LookUp(typeSolution.GetExplicitTypeType(explicitType));
//        }

//        public IFrontendType GetMemberType(Tpn.IMember member)
//        {
//            return LookUp(typeSolution.GetMemberType(member));
//        }

//        public IFrontendType GetMethodScopeType(Tpn.IMethod method)
//        {
//            return LookUp(typeSolution.GetMethodScopeType(method));
//        }

//        public IFrontendType GetObjectType(Tpn.IObject @object)
//        {
//            return LookUp(typeSolution.GetObjectType(@object));
//        }

//        public IFrontendType GetOrType(Tpn.IOrType orType)
//        {
//            return LookUp(typeSolution.GetOrType(orType));
//        }

//        public IFrontendType GetScopeType(Tpn.IScope scope)
//        {
//            return LookUp(typeSolution.GetScopeType(scope));
//        }

//        public IFrontendType GetTypeReferenceType(Tpn.ITypeReference typeReference)
//        {
//            return LookUp(typeSolution.GetTypeReferenceType(typeReference));
//        }

//        public IFrontendType GetValueType(Tpn.IValue value)
//        {
//            return LookUp(typeSolution.GetValueType(value));
//        }
//    }
//}
