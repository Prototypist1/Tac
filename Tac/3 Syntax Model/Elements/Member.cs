using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model.Elements;
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
        public MemberMaker()
        {
        }
        
        public IResult<IPopulateScope<WeakMemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberPopulateScope(first.Item)); ;
            }
            return ResultExtension.Bad<IPopulateScope<WeakMemberReferance>>();
        }
    }
    
    public class MemberPopulateScope : IPopulateScope<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly Box<IType> box = new Box<IType>();

        public MemberPopulateScope(string item)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
        }

        public IBox<IType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            var nameKey = new NameKey(memberName);
            if (!context.Scope.TryGetMember(nameKey, false, out var memberDef) && 
                !context.Scope.TryAddMember(DefintionLifetime.Instance,nameKey, new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,nameKey,new Box<IType>(new AnyType())))))
            {
                throw new Exception("uhh that is not right");
            }
            
            return new MemberResolveReferance(context.GetResolvableScope(), nameKey, box);
        }

    }

    public class MemberResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {
        private readonly IResolvableScope resolvableScope;
        private readonly NameKey key;
        private readonly Box<IType> box;

        public MemberResolveReferance(
            IResolvableScope resolvableScope,
            NameKey key, 
            Box<IType> box)
        {
            this.resolvableScope = resolvableScope ?? throw new ArgumentNullException(nameof(resolvableScope));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IOpenBoxes<WeakMemberReferance> Run(IResolveReferanceContext context)
        {
            var item =  box.Fill(new WeakMemberReferance(resolvableScope.GetMemberOrThrow(key, false)));
            return new MemberOpenBoxes(item);
        }
    }

    internal class MemberOpenBoxes : IOpenBoxes<WeakMemberReferance>
    {
        public WeakMemberReferance CodeElement { get; }

        public MemberOpenBoxes(WeakMemberReferance item)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberReferance(CodeElement);
        }
    }
}