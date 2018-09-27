using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

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

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<Member> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                result = new MemberPopulateScope(matchingContext.ScopeStack.TopScope.Cast<LocalStaticScope>(), first.Item, Make);
                return true;
            }

            result = default;
            return false;
        }
    }
    
    public class MemberPopulateScope : IPopulateScope<Member>
    {
        private readonly LocalStaticScope topScope;
        private readonly string memberName;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;

        public MemberPopulateScope(LocalStaticScope topScope, string item, Func<int, IBox<MemberDefinition>, Member> make)
        {
            this.topScope = topScope ?? throw new ArgumentNullException(nameof(topScope));
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReferance<Member> Run(IPopulateScopeContext context)
        {
            int depth;
            IBox<MemberDefinition> memberDef;
            var scopeStack = new ScopeStack(context.Tree, topScope);
            if (!scopeStack.TryGetMemberPath(new Names.ExplicitMemberName(memberName), out depth, out memberDef))
            {
                memberDef = new Box<MemberDefinition>(new MemberDefinition(false, new ExplicitMemberName(memberName), scopeStack.GetType(RootScope.AnyType)));
                if (!topScope.TryAddLocal(new NameKey(memberName), memberDef))
                {
                    throw new Exception("bad bad bad!");
                }
            }

            return new MemberResolveReferance(depth, memberDef, make);
        }

    }

    public class MemberResolveReferance : IResolveReferance<Member>
    {
        private readonly int depth;
        private readonly IBox<MemberDefinition> memberDef;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;

        public MemberResolveReferance(int depth, IBox<MemberDefinition> memberDef, Func<int, IBox<MemberDefinition>, Member> make)
        {
            this.depth = depth;
            this.memberDef = memberDef ?? throw new ArgumentNullException(nameof(memberDef));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public Member Run(IResolveReferanceContext context)
        {
            return make(depth, memberDef);
        }
    }

    public class ImplicitMemberMaker : IMaker<Member>
    {
        public ImplicitMemberMaker(Func<int, IBox<MemberDefinition>, Member> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<int, IBox<MemberDefinition>, Member> Make { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<Member> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("var"), out var _)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                result = new MemberPopulateScope(matchingContext.ScopeStack.TopScope.Cast<LocalStaticScope>(), first.Item, Make);
                return true;
            }

            result = default;
            return false;
        }

    }

    public class ImplicitMemberPopulateScope : IPopulateScope<Member>
    {
        private readonly LocalStaticScope topScope;
        private readonly string memberName;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;

        public ImplicitMemberPopulateScope(LocalStaticScope topScope, string item, Func<int, IBox<MemberDefinition>, Member> make)
        {
            this.topScope = topScope ?? throw new ArgumentNullException(nameof(topScope));
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReferance<Member> Run(IPopulateScopeContext context)
        {

            var typeDef = new FollowBox<ITypeDefinition>();
            var innerType = new MemberDefinition(false, new ExplicitMemberName(memberName), typeDef);
            IBox<MemberDefinition> memberDef = new Box<MemberDefinition>(innerType);

            if (!topScope.TryAddLocal(new NameKey(memberName), memberDef))
            {
                throw new Exception("bad bad bad!");
            }


            return new ImplicitMemberResolveReferance(innerType, make, typeDef);
        }

    }

    public class ImplicitMemberResolveReferance : IResolveReferance<Member>
    {
        private readonly MemberDefinition memberDef;
        private readonly Func<int, MemberDefinition, Member> make;
        private readonly FollowBox<ITypeDefinition> typeDef;

        public ImplicitMemberResolveReferance(MemberDefinition innerType, Func<int, MemberDefinition, Member> make, FollowBox<ITypeDefinition> typeDef)
        {
            this.memberDef = innerType ?? throw new ArgumentNullException(nameof(innerType));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.typeDef = typeDef;
        }

        public Member Run(IResolveReferanceContext context)
        {
            if (context.TryGetParent<BinaryResolveReferance<AssignOperation>>(out var op)) {
                typeDef.Follow(op.right.GetReturnType());
            }

            return make(0, memberDef);
        }
    }
}