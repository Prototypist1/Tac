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

    public class MemberMaker<T> : IMaker<T, WeakMemberReferance>
    {
        public MemberMaker(Func<WeakMemberReferance,T> make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private Func<WeakMemberReferance,T> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<T, WeakMemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsName, out AtomicToken first)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberPopulateScope<T>( first.Item, Make)); ;
            }
            return ResultExtension.Bad<IPopulateScope<T, WeakMemberReferance>>();
        }
    }
    
    public class MemberPopulateScope<T> : IPopulateScope<T, WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly Func<WeakMemberReferance,T> make;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public MemberPopulateScope(string item, Func<WeakMemberReferance,T> make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<T, WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            var nameKey = new NameKey(memberName);
            if (!context.Scope.TryGetMember(nameKey, false, out var memberDef) && 
                !context.Scope.TryAddMember(DefintionLifetime.Instance,nameKey, new Box<WeakMemberDefinition>(new WeakMemberDefinition(false,nameKey,new Box<IWeakReturnable>(context.ElementBuilders.AnyType())))))
            {
                throw new Exception("uhh that is not right");
            }
            
            return new MemberResolveReferance<T>(context.GetResolvableScope(), nameKey, make, box);
        }

    }

    public class MemberResolveReferance<T> : IPopulateBoxes<T, WeakMemberReferance>
    {
        private readonly IResolvableScope resolvableScope;
        private readonly NameKey key;
        private readonly Func<WeakMemberReferance,T> make;
        private readonly Box<IWeakReturnable> box;

        public MemberResolveReferance(
            IResolvableScope resolvableScope,
            NameKey key, 
            Func<WeakMemberReferance,T> make, 
            Box<IWeakReturnable> box)
        {
            this.resolvableScope = resolvableScope ?? throw new ArgumentNullException(nameof(resolvableScope));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IOpenBoxes<T, WeakMemberReferance> Run(IResolveReferanceContext context)
        {
            var item =  box.Fill(new WeakMemberReferance(resolvableScope.GetMemberOrThrow(key, false)));
            return new MemberOpenBoxes<T>(item, make);
        }
    }

    internal class MemberOpenBoxes<T> : IOpenBoxes<T, WeakMemberReferance>
    {
        public WeakMemberReferance CodeElement { get; }
        private readonly Func<WeakMemberReferance, T> make;

        public MemberOpenBoxes(WeakMemberReferance item, Func<WeakMemberReferance, T> make)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public T Run(IOpenBoxesContext context)
        {
            return make(CodeElement);
        }
    }
}