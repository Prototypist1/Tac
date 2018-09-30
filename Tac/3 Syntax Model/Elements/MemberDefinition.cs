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

        public MemberDefinition GetMemberDefinition(ScopeStack scopeStack)
        {
            return this;
        }
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
                .Has(ElementMatcher.IsType, out ExplicitTypeName typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberDefinitionPopulateScope(matchingContext.ScopeStack.TopScope, nameToken.Item, readonlyToken != default, typeToken, Make));
            }
            return ResultExtension.Bad<IPopulateScope<Member>>();
        }

    }


    public class MemberDefinitionPopulateScope : IPopulateScope<Member>
    {
        private readonly IScope scope;
        private readonly string memberName;
        private readonly bool isReadonly;
        private readonly ExplicitTypeName explicitTypeName;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;

        public MemberDefinitionPopulateScope(IScope topScope, string item, bool v, ExplicitTypeName typeToken, Func<int, IBox<MemberDefinition>, Member> make)
        {
            scope = topScope ?? throw new ArgumentNullException(nameof(topScope));
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            explicitTypeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReferance<Member> Run(IPopulateScopeContext context)
        {
            var memberDef = new Box<MemberDefinition>();
            var scopeStack = new ScopeStack(context.Tree, scope);
            if (scope.Cast<LocalStaticScope>().TryAddLocal(new Names.ExplicitMemberName(memberName).Key, memberDef))
            {
                throw new Exception("bad bad bad!");
            }
            return new MemberDefinitionResolveReferance(scope, memberName, memberDef, isReadonly, explicitTypeName, make);
        }

    }

    public class MemberDefinitionResolveReferance : IResolveReferance<Member>
    {
        private readonly IScope scope;
        private readonly string memberName;
        private readonly Box<MemberDefinition> memberDef;
        private readonly bool isReadonly;
        public readonly ExplicitTypeName explicitTypeName;
        private readonly Func<int, IBox<MemberDefinition>, Member> make;

        public MemberDefinitionResolveReferance(IScope scope, string memberName, Box<MemberDefinition> memberDef, bool isReadonly, ExplicitTypeName explicitTypeName, Func<int, IBox<MemberDefinition>, Member> make)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.memberDef = memberDef ?? throw new ArgumentNullException(nameof(memberDef));
            this.isReadonly = isReadonly;
            this.explicitTypeName = explicitTypeName ?? throw new ArgumentNullException(nameof(explicitTypeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public Member Run(IResolveReferanceContext context)
        {
            memberDef.Fill(new MemberDefinition(isReadonly, new ExplicitMemberName(memberName), new ScopeStack(context.Tree, scope).GetType(explicitTypeName)));
            return make(0, memberDef);
        }


        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return context.Tree.Root.GetTypeOrThrow(RootScope.MemberType.Key);
        }

    }
}