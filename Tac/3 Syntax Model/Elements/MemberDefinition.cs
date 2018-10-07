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
    public class MemberDefinition
    {
        public MemberDefinition(bool readOnly, NameKey key, IBox<ITypeDefinition> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<ITypeDefinition> Type { get; }
        public bool ReadOnly { get; }
        public NameKey Key { get; }
    }

    public class MemberDefinitionMaker : IMaker<Member>
    {
        public MemberDefinitionMaker(Func<int, IBox<MemberDefinition>, Member> make,
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
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsType, out NameKey typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberDefinitionPopulateScope(nameToken.Item, readonlyToken != default, typeToken, Make));
            }
            return ResultExtension.Bad<IPopulateScope<Member>>();
        }

    }


    public class MemberDefinitionPopulateScope : IPopulateScope<Member>
    {
        private readonly string memberName;
        private readonly bool isReadonly;
        private readonly NameKey typeName;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;

        public MemberDefinitionPopulateScope(string item, bool v, NameKey typeToken, Func<int, IBox<MemberDefinition>, Member> make)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            typeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<Member> Run(IPopulateScopeContext context)
        {
            var memberDef = new Box<MemberDefinition>();
            var key = new NameKey(memberName);
            if (context.TryAddMember(key, memberDef))
            {
                throw new Exception("bad bad bad!");
            }
            return new MemberDefinitionResolveReferance( memberName, memberDef, isReadonly, typeName, make);
        }

    }

    public class MemberDefinitionResolveReferance : IResolveReference<Member>
    {
        private readonly string memberName;
        private readonly Box<MemberDefinition> memberDef;
        private readonly bool isReadonly;
        public readonly NameKey typeName;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;

        public MemberDefinitionResolveReferance(string memberName, Box<MemberDefinition> memberDef, bool isReadonly, NameKey explicitTypeName, Func<int, IBox<MemberDefinition>, Member> make)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.memberDef = memberDef ?? throw new ArgumentNullException(nameof(memberDef));
            this.isReadonly = isReadonly;
            this.typeName = explicitTypeName ?? throw new ArgumentNullException(nameof(explicitTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public Member Run(IResolveReferanceContext context)
        {
            memberDef.Fill(new MemberDefinition(isReadonly, new NameKey(memberName), context.GetTypeDefintion(typeName)));
            return make(0, memberDef);
        }


        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return context.RootScope.MemberType(typeName);
        }
    }
}