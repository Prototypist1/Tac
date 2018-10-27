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

    public interface IMemberDefinition : ICodeElement, IReturnable
    {
        IWeakReturnable Type { get; }
        bool ReadOnly { get; }
        // why does this know it's own key??
        NameKey Key { get; }
    }

    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    public class WeakMemberDefinition: IWeakReturnable, IWeakCodeElement
    {
        public WeakMemberDefinition(bool readOnly, NameKey key, IBox<IWeakReturnable> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<IWeakReturnable> Type { get; }
        public bool ReadOnly { get; }
        public NameKey Key { get; }

        public IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return this;
        }
    }

    public class MemberDefinitionMaker<T> : IMaker<T, WeakMemberReferance>
    {
        public MemberDefinitionMaker(Func<WeakMemberReferance,T> make,
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
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsType, out NameKey typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberDefinitionPopulateScope<T>(nameToken.Item, readonlyToken != default, typeToken, Make));
            }
            return ResultExtension.Bad<IPopulateScope<T, WeakMemberReferance>>();
        }
    }

    public class MemberDefinitionPopulateScope<T> : IPopulateScope<T, WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly bool isReadonly;
        private readonly NameKey typeName;
        private readonly Func<WeakMemberReferance,T> make;
        private readonly Box<WeakMemberReferance> box = new Box<WeakMemberReferance>();
        private readonly Box<WeakMemberDefinition> memberDefinitionBox = new Box<WeakMemberDefinition>();

        public MemberDefinitionPopulateScope(string item, bool v, NameKey typeToken, Func<WeakMemberReferance,T> make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            typeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IPopulateBoxes<T, WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            var key = new NameKey(memberName);
            if (!context.Scope.TryAddMember(DefintionLifetime.Instance,key, memberDefinitionBox))
            {
                throw new Exception("bad bad bad!");
            }
            return new MemberDefinitionResolveReferance<T>(memberName, box, isReadonly, typeName, make, context.GetResolvableScope(), memberDefinitionBox);
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }
    }

    public class MemberDefinitionResolveReferance<T> : IPopulateBoxes<T, WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly Box<WeakMemberReferance> box;
        private readonly bool isReadonly;
        public readonly NameKey typeName;
        private readonly Func<WeakMemberReferance,T> make;
        private readonly IResolvableScope scope;
        private readonly Box<WeakMemberDefinition> memberDefinitionBox;

        public MemberDefinitionResolveReferance(
            string memberName,
            Box<WeakMemberReferance> box,
            bool isReadonly,
            NameKey explicitTypeName,
            Func<WeakMemberReferance,T> make,
            IResolvableScope scope,
            Box<WeakMemberDefinition> memberDefinitionBox)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.isReadonly = isReadonly;
            typeName = explicitTypeName ?? throw new ArgumentNullException(nameof(explicitTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.memberDefinitionBox = memberDefinitionBox ?? throw new ArgumentNullException(nameof(memberDefinitionBox));
        }

        public IOpenBoxes<T, WeakMemberReferance> Run(IResolveReferanceContext context)
        {
            memberDefinitionBox.Fill(
                new WeakMemberDefinition(
                    isReadonly,
                    new NameKey(memberName),
                    scope.GetTypeOrThrow(typeName)));

            var item=  box.Fill(new WeakMemberReferance(memberDefinitionBox));
            return new MemberDefinitionOpenBoxes<T>(item, make);
        }
    }

    internal class MemberDefinitionOpenBoxes<T> : IOpenBoxes<T, WeakMemberReferance>
    {
        public WeakMemberReferance CodeElement { get; }
        private readonly Func<WeakMemberReferance, T> make;

        public MemberDefinitionOpenBoxes(WeakMemberReferance item, Func<WeakMemberReferance, T> make)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }
    }
}