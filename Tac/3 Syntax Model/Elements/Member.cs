using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class Member : ICodeElement
    {
        public Member(int scopesUp, IBox<MemberDefinition> memberDefinition)
        {
            ScopesUp = scopesUp;
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public int ScopesUp { get; }
        public IBox<MemberDefinition> MemberDefinition { get; }

        public IBox<ITypeDefinition> ReturnType(ScopeStack scope)
        {
            return MemberDefinition.ReturnType(scope);
        }
    }

    public class MemberMaker : IMaker<Member>
    {
        public MemberMaker(Func<int, IBox<MemberDefinition>, Member> make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private Func<int, IBox<MemberDefinition>, Member> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<Member> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                result = PopulateScope(matchingContext.ScopeStack.TopScope,first.Item);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<Member> PopulateScope(IScope scope, string memberName)
        {
            return (tree) =>
            {
                int depth;
                IBox<MemberDefinition> memberDef;
                var scopeStack = new ScopeStack(tree, scope);
                if (!scopeStack.TryGetMemberPath(new Names.ExplicitMemberName(memberName), out depth, out memberDef)) {
                    memberDef = new Box<MemberDefinition>(new MemberDefinition(false,new ExplicitMemberName(memberName), scopeStack.GetType(RootScope.AnyType)));
                    if (!scopeStack.TopScope.Cast<LocalStaticScope>().TryAddLocal(new NameKey(memberName), memberDef)) {
                        throw new Exception("bad bad bad!");
                    }
                }

                return DetermineInferedTypes(scope, depth, memberDef);
            };
        }

        private Steps.DetermineInferedTypes<Member> DetermineInferedTypes(IScope scope,  int depth, IBox<MemberDefinition> box)
        {
            return () =>
            {
                return ResolveReferance(scope, depth, box);
            };
        }

        private Steps.ResolveReferance<Member> ResolveReferance(IScope scope, int depth, IBox<MemberDefinition> box)
        {
            return (tree) =>
            {
                return Make(depth,box);
            };
        }
    }

    public class ImplicitMemberMaker : IMaker<Member>
    {
        public ImplicitMemberMaker(Func<int, IBox<MemberDefinition>, Member> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<int, IBox<MemberDefinition>, Member> Make { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<Member> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                result = PopulateScope(matchingContext.ScopeStack.TopScope, first.Item);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<Member> PopulateScope(IScope scope, string memberName)
        {
            return (tree) =>
            {
                int depth;
                IBox<MemberDefinition> memberDef;
                var scopeStack = new ScopeStack(tree, scope);
                if (!scopeStack.TryGetMemberPath(new Names.ExplicitMemberName(memberName), out depth, out memberDef))
                {
                    memberDef = new Box<MemberDefinition>(new MemberDefinition(false, new ExplicitMemberName(memberName), scopeStack.GetType(RootScope.AnyType)));
                    if (!scopeStack.TopScope.Cast<LocalStaticScope>().TryAddLocal(new NameKey(memberName), memberDef))
                    {
                        throw new Exception("bad bad bad!");
                    }
                }

                return DetermineInferedTypes(scope, depth, memberDef);
            };
        }

        private Steps.DetermineInferedTypes<Member> DetermineInferedTypes(IScope scope, int depth, IBox<MemberDefinition> box)
        {
            return () =>
            {
                return ResolveReferance(scope, depth, box);
            };
        }

        private Steps.ResolveReferance<Member> ResolveReferance(IScope scope, int depth, IBox<MemberDefinition> box)
        {
            return (tree) =>
            {
                return Make(depth, box);
            };
        }
    }
}