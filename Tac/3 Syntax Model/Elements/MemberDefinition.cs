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
    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    public class MemberDefinition: IReturnable, ICodeElement
    {
        public delegate MemberDefinition Make(bool readOnly, NameKey key, IBox<IReturnable> type);

        public MemberDefinition(bool readOnly, NameKey key, IBox<IReturnable> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<IReturnable> Type { get; }
        public bool ReadOnly { get; }
        public NameKey Key { get; }

        public IReturnable Returns(IElementBuilders elementBuilders)
        {
            return this;
        }
    }

    public class MemberDefinitionMaker : IMaker<MemberReferance>
    {
        public MemberDefinitionMaker(MemberReferance.Make make,
            IElementBuilders elementBuilders)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        private MemberReferance.Make Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public IResult<IPopulateScope<MemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
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
            return ResultExtension.Bad<IPopulateScope<MemberReferance>>();
        }
    }

    public class MemberDefinitionPopulateScope : IPopulateScope<MemberReferance>
    {
        private readonly string memberName;
        private readonly bool isReadonly;
        private readonly NameKey typeName;
        private readonly MemberReferance.Make make;
        private readonly Box<MemberReferance> box = new Box<MemberReferance>();
        private readonly Box<MemberDefinition> memberDefinitionBox = new Box<MemberDefinition>();

        public MemberDefinitionPopulateScope(string item, bool v, NameKey typeToken, MemberReferance.Make make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            typeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<MemberReferance> Run(IPopulateScopeContext context)
        {
            var key = new NameKey(memberName);
            if (context.Scope.TryAddMember(DefintionLifetime.Instance,key, memberDefinitionBox))
            {
                throw new Exception("bad bad bad!");
            }
            return new MemberDefinitionResolveReferance(memberName, box, isReadonly, typeName, make, context.GetResolvableScope(), memberDefinitionBox);
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }
    }

    public class MemberDefinitionResolveReferance : IResolveReference<MemberReferance>
    {
        private readonly string memberName;
        private readonly Box<MemberReferance> box;
        private readonly bool isReadonly;
        public readonly NameKey typeName;
        private readonly MemberReferance.Make make;
        private readonly IResolvableScope scope;
        private readonly Box<MemberDefinition> memberDefinitionBox;

        public MemberDefinitionResolveReferance(
            string memberName,
            Box<MemberReferance> box,
            bool isReadonly,
            NameKey explicitTypeName,
            MemberReferance.Make make,
            IResolvableScope scope,
            Box<MemberDefinition> memberDefinitionBox)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.isReadonly = isReadonly;
            typeName = explicitTypeName ?? throw new ArgumentNullException(nameof(explicitTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.memberDefinitionBox = memberDefinitionBox ?? throw new ArgumentNullException(nameof(memberDefinitionBox));
        }

        public MemberReferance Run(IResolveReferanceContext context)
        {
            memberDefinitionBox.Fill(
                context.ElementBuilders.MemberDefinition(
                    isReadonly,
                    new NameKey(memberName),
                    scope.GetType(typeName)));

            return box.Fill(make(memberDefinitionBox));
        }
    }
}