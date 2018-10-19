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

    public class MemberMaker : IMaker<PathPart>
    {
        public MemberMaker(PathPart.Make make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private PathPart.Make Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<PathPart>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberPopulateScope( first.Item, Make)); ;
            }
            return ResultExtension.Bad<IPopulateScope<PathPart>>();
        }
    }
    
    public class MemberPopulateScope : IPopulateScope<PathPart>
    {
        private readonly string memberName;
        private readonly PathPart.Make make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public MemberPopulateScope(string item, PathPart.Make make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<PathPart> Run(IPopulateScopeContext context)
        {
            var nameKey = new NameKey(memberName);
            if (!context.TryGetMemberPath(nameKey, out var depth, out var memberDef) && 
                !context.TryAddMember(nameKey, new Box<MemberDefinition>(context.ElementBuilders.MemberDefinition(false,nameKey,new Box<AnyType>(context.ElementBuilders.AnyType())))))
            {
                throw new Exception("uhh that is not right");
            }
            
            return new MemberResolveReferance(nameKey, make, box);
        }

    }

    public class MemberResolveReferance : IResolveReference<PathPart>
    {

        private readonly NameKey key;
        private readonly PathPart.Make make;
        private readonly Box<IReturnable> box;

        public MemberResolveReferance(
            NameKey key, 
            PathPart.Make make, 
            Box<IReturnable> box)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public PathPart Run(IResolveReferanceContext context)
        {
            return box.Fill(make(context.GetMemberDefinition(key)));
        }
    }



}