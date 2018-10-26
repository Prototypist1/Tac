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
        public delegate WeakMemberDefinition Make(bool readOnly, NameKey key, IBox<IWeakReturnable> type);

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

    public class MemberDefinitionMaker : IMaker<WeakMemberReferance>
    {
        public MemberDefinitionMaker(WeakMemberReferance.Make make,
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
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsType, out NameKey typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberDefinitionPopulateScope(nameToken.Item, readonlyToken != default, typeToken, Make));
            }
            return ResultExtension.Bad<IPopulateScope<WeakMemberReferance>>();
        }
    }

    public class MemberDefinitionPopulateScope : IPopulateScope<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly bool isReadonly;
        private readonly NameKey typeName;
        private readonly WeakMemberReferance.Make make;
        private readonly Box<WeakMemberReferance> box = new Box<WeakMemberReferance>();
        private readonly Box<WeakMemberDefinition> memberDefinitionBox = new Box<WeakMemberDefinition>();

        public MemberDefinitionPopulateScope(string item, bool v, NameKey typeToken, WeakMemberReferance.Make make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            typeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            var key = new NameKey(memberName);
            if (!context.Scope.TryAddMember(DefintionLifetime.Instance,key, memberDefinitionBox))
            {
                throw new Exception("bad bad bad!");
            }
            return new MemberDefinitionResolveReferance(memberName, box, isReadonly, typeName, make, context.GetResolvableScope(), memberDefinitionBox);
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }
    }

    public class MemberDefinitionResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly Box<WeakMemberReferance> box;
        private readonly bool isReadonly;
        public readonly NameKey typeName;
        private readonly WeakMemberReferance.Make make;
        private readonly IResolvableScope scope;
        private readonly Box<WeakMemberDefinition> memberDefinitionBox;

        public MemberDefinitionResolveReferance(
            string memberName,
            Box<WeakMemberReferance> box,
            bool isReadonly,
            NameKey explicitTypeName,
            WeakMemberReferance.Make make,
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

        public WeakMemberReferance Run(IResolveReferanceContext context)
        {
            memberDefinitionBox.Fill(
                context.ElementBuilders.MemberDefinition(
                    isReadonly,
                    new NameKey(memberName),
                    scope.GetTypeOrThrow(typeName)));

            return box.Fill(make(memberDefinitionBox));
        }
    }
}