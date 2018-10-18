using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    //public class Member : ICodeElement
    //{
    //    public delegate Member Make(int scopesUp, IBox<MemberDefinition> memberDefinition);

    //    public Member(int scopesUp, IReadOnlyList<IBox<MemberDefinition>> memberDefinitions)
    //    {
    //        ScopesUp = scopesUp;
    //        MemberDefinitions = memberDefinitions ?? throw new ArgumentNullException(nameof(memberDefinitions));
    //    }
        
    //    public int ScopesUp { get; }
    //    public IReadOnlyList<IBox<MemberDefinition>> MemberDefinitions { get; }

    //    public IReturnable Returns(IElementBuilders builders)
    //    {
    //        return this;
    //    }

    //    public Member Child(IBox<MemberDefinition> member) {
    //        var list = MemberDefinitions.ToList();
    //        list.Add(member);
    //        return new Member(ScopesUp, list);
    //    }
    //}

    public class MemberMaker : IMaker<MemberDefinition>
    {
        public MemberMaker(MemberDefinition.Make make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private MemberDefinition.Make Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<MemberDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberPopulateScope( first.Item, Make)); ;
            }
            return ResultExtension.Bad<IPopulateScope<MemberDefinition>>();
        }
    }
    
    public class MemberPopulateScope : IPopulateScope<MemberDefinition>
    {
        private readonly string memberName;
        private readonly MemberDefinition.Make make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public MemberPopulateScope(string item, MemberDefinition.Make make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<MemberDefinition> Run(IPopulateScopeContext context)
        {
            var nameKey = new NameKey(memberName);
            IBox<MemberDefinition> memberDef = new Box<MemberDefinition>(new MemberDefinition(false, nameKey, new Box<IReturnable>(context.ElementBuilders.AnyType())));
            if (!context.TryGetMemberPath(nameKey, out var depth, out memberDef) && !context.TryAddMember(nameKey, memberDef)) {
                throw new Exception("uhh that is not right");
            }
            
            return new MemberResolveReferance(depth, memberDef, make, box);
        }

    }

    public class MemberResolveReferance : IResolveReference<MemberDefinition>
    {
        private readonly int depth;
        private readonly IBox<MemberDefinition> memberDef;
        private readonly MemberDefinition.Make make;
        private readonly Box<IReturnable> box;

        public MemberResolveReferance(
            int depth, 
            IBox<MemberDefinition> memberDef,
            MemberDefinition.Make make, 
            Box<IReturnable> box)
        {
            this.depth = depth;
            this.memberDef = memberDef ?? throw new ArgumentNullException(nameof(memberDef));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public MemberDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(depth, memberDef));
        }
    }



}