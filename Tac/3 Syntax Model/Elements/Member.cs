using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
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

    public class MemberMaker : IMaker<WeakMemberReferance>
    {
        public MemberMaker(WeakMemberReferance.Make make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private WeakMemberReferance.Make Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<WeakMemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberPopulateScope( first.Item, Make)); ;
            }
            return ResultExtension.Bad<IPopulateScope<WeakMemberReferance>>();
        }
    }
    
    public class MemberPopulateScope : IPopulateScope<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly WeakMemberReferance.Make make;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public MemberPopulateScope(string item, WeakMemberReferance.Make make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            var nameKey = new NameKey(memberName);
            if (!context.Scope.TryGetMember(nameKey, false, out var memberDef) && 
                !context.Scope.TryAddMember(DefintionLifetime.Instance,nameKey, new Box<WeakMemberDefinition>(context.ElementBuilders.MemberDefinition(false,nameKey,new Box<IWeakReturnable>(context.ElementBuilders.AnyType())))))
            {
                throw new Exception("uhh that is not right");
            }
            
            return new MemberResolveReferance(context.GetResolvableScope(), nameKey, make, box);
        }

    }

    public class MemberResolveReferance : IResolveReference<WeakMemberReferance>
    {
        private readonly IResolvableScope resolvableScope;
        private readonly NameKey key;
        private readonly WeakMemberReferance.Make make;
        private readonly Box<IWeakReturnable> box;

        public MemberResolveReferance(
            IResolvableScope resolvableScope,
            NameKey key, 
            WeakMemberReferance.Make make, 
            Box<IWeakReturnable> box)
        {
            this.resolvableScope = resolvableScope ?? throw new ArgumentNullException(nameof(resolvableScope));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public WeakMemberReferance Run(IResolveReferanceContext context)
        {
            return box.Fill(make(resolvableScope.GetMemberOrThrow(key, false)));
        }
    }



}