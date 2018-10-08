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

        public IBox<ITypeDefinition> ReturnType(RootScope rootScope)
        {
            return rootScope.MemberType(MemberDefinition.GetValue().Key);
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
                return ResultExtension.Good(new MemberPopulateScope( first.Item, Make)); ;
            }
            return ResultExtension.Bad<IPopulateScope<Member>>();
        }
    }
    
    public class MemberPopulateScope : IPopulateScope<Member>
    {
        private readonly string memberName;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;
        

        public MemberPopulateScope(string item, Func<int, IBox<MemberDefinition>, Member> make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<Member> Run(IPopulateScopeContext context)
        {
            var nameKey = new NameKey(memberName);
            IBox<MemberDefinition> memberDef = new Box<MemberDefinition>(new MemberDefinition(false, nameKey, context.RootScope.AnyType));
            if (!context.TryGetMemberPath(nameKey, out var depth, out memberDef) && !context.TryAddMember(nameKey, memberDef)) {
                throw new Exception("uhh that is not right");
            }
            
            return new MemberResolveReferance(depth, memberDef, make, RootKeys.AnyType);
        }

    }

    public class MemberResolveReferance : IResolveReference<Member>
    {
        private readonly int depth;
        private readonly IBox<MemberDefinition> memberDef;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;
        private readonly NameKey typeKey;

        public MemberResolveReferance(int depth, IBox<MemberDefinition> memberDef, Func<int, IBox<MemberDefinition>, Member> make, NameKey key)
        {
            this.depth = depth;
            this.memberDef = memberDef ?? throw new ArgumentNullException(nameof(memberDef));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return 
                context.RootScope.MemberType(typeKey);
        }

        public Member Run(IResolveReferanceContext context)
        {
            return make(depth, memberDef);
        }
    }

}