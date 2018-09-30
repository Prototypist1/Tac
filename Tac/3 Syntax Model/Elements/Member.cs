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

        public IResult<IPopulateScope<Member>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberPopulateScope(matchingContext.ScopeStack.TopScope.Cast<LocalStaticScope>(), first.Item, Make)); ;
            }
            return ResultExtension.Bad<IPopulateScope<Member>>();
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
            var scopeStack = new ScopeStack(context.Tree, topScope);
            if (!scopeStack.TryGetMemberPath(new Names.ExplicitMemberName(memberName), out var depth, out var memberDef))
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

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return context.Tree.Root.GetTypeOrThrow(RootScope.MemberType.Key);
        }

        public Member Run(IResolveReferanceContext context)
        {
            return make(depth, memberDef);
        }
    }

}