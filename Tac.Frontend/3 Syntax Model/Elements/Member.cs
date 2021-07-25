using System;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.Operations;
using Tac.Frontend.SyntaxModel.Operations;
using Prototypist.Toolbox;
using Tac.Frontend.Parser;
using System.Collections.Generic;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticMemberMaker = AddElementMakers(
            () => new MemberMaker());
#pragma warning disable CA1823 
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> MemberMaker = StaticMemberMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel
{



    internal class MemberMaker : IMaker<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>>
    {
        public MemberMaker()
        {
        }
        
        public ITokenMatching<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
           return tokenMatching
                .Has(new NameMaker())
                .ConvertIfMatched(token => new MemberPopulateScope(token.Item), tokenMatching);
        }

        //public static ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> PopulateScope(string item)
        //{
        //    return new MemberPopulateScope(item);
        //}

    }

    internal class MemberPopulateScope : ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>
    {
        public readonly string memberName;

        public MemberPopulateScope(string item)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
        }

        public ISetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            // this is a bit werid
            // it creates member possibly on parent for code like
            // type {x;y;}
            // or
            // object {1 =: x; 2 =: y;}
            // the second is really bad tho, if you had:
            // 1 =: x;
            // object {1 =: x; 2 =: y;}
            // the possible member for x in the object would not result in a real member 

            // {48146F3A-6D75-4F24-B857-BED24CE846EA}
            // here is a painful situaltion
            // 1 =: x;
            // object {x =: x; 2 =: y;}
            // in object the LHS x is resolves up 
            // the RHS x resolves to create a new member

            if (!(scope is Tpn.IHavePossibleMembers possibleScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            var scopeList = new List<Tpn.IStaticScope>();

            scopeList.Add(scope);

            var at = scope;
            while (at.Parent.Is(out var nextScope))
            {
                at = nextScope;
                scopeList.Add(at);
            }

            var nameKey = new NameKey(memberName);
            var member = GetMember(scope, context, possibleScope, nameKey);
            return new SetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>(new MemberResolveReferance(nameKey, scopeList), OrType.Make<Tpn.TypeProblem2.Member, IError>(member));
        }

        private static Tpn.TypeProblem2.Member GetMember(Tpn.IStaticScope scope, ISetUpContext context, Tpn.IHavePossibleMembers possibleScope, NameKey nameKey)
        {



            // TODO
            // there needs to be a case about being directly in a type

            // this handles this case
            // type example{
            // x;
            // type inner { x;} y;
            //}
            // and this case
            // 1 =: x;
            // object {x =: x; 2 =: y;}
            // in object the LHS x is resolves up 
            // the RHS x resolves to create a new member
            if ((context.EnclosingSetUp is WeakAssignOperationPopulateScope &&
                            context.Parent.Is(out var parent) &&
                            (parent is ObjectDefinitionPopulateScope)) || context.EnclosingSetUp is TypeDefinitionPopulateScope)
            {
                if (!(scope is Tpn.IHavePublicMembers havePublicMember))
                {
                    // this should only be used in object and type definitions 
                    throw new NotImplementedException("this should be an ierror");
                }

                return context.TypeProblem.CreatePublicMember(scope, havePublicMember, nameKey); ;
            }
            else
            {
               return context.TypeProblem.CreateMemberPossiblyOnParent(scope, possibleScope, nameKey);
            }
        }
    }

    internal class MemberResolveReferance : IResolve<IBox<WeakMemberReference>>
    {
        private readonly IKey key;
        private readonly List<Tpn.IStaticScope> staticScopes;

        public MemberResolveReferance(IKey key, List<Tpn.IStaticScope> staticScopes)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.staticScopes = staticScopes ?? throw new ArgumentNullException(nameof(staticScopes));
        }

        public IBox<WeakMemberReference> Run(Tpn.TypeSolution context)
        {
            foreach (var scope in staticScopes)
            {
                if (context.TryGetMember(scope, key, out var res)) {
                    return new Box<WeakMemberReference>(new WeakMemberReference(new Box<WeakMemberDefinition>(res.Is1OrThrow())));
                }
            }
            throw new Exception("should have found that!");
        }
    }
}