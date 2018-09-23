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
        public MemberDefinition(bool readOnly, ExplicitMemberName key, IBox<ITypeDefinition> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<ITypeDefinition> Type { get; }
        public bool ReadOnly { get; }
        public ExplicitMemberName Key { get; }
        
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

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<Member> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsType, out ExplicitTypeName typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                
                result = new MemberPopulateScope(matchingContext.ScopeStack.TopScope, nameToken.Item, readonlyToken != default, typeToken,Make);
                return true;
            }

            result = default;
            return false;
        }

        private class MemberPopulateScope : IPopulateScope<Member>
        {
            private readonly IScope scope;
            private readonly string memberName;
            private readonly bool isReadonly;
            private readonly ExplicitTypeName explicitTypeName;
            private readonly Func<int, IBox<MemberDefinition>, Member> make;

            public MemberPopulateScope(IScope topScope, string item, bool v, ExplicitTypeName typeToken, Func<int, IBox<MemberDefinition>, Member> make)
            {
                this.scope = topScope ?? throw new ArgumentNullException(nameof(topScope));
                this.memberName = item ?? throw new ArgumentNullException(nameof(item));
                this.isReadonly = v;
                this.explicitTypeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IResolveReferance<Member> Run(ScopeTree tree)
            {
                var memberDef = new Box<MemberDefinition>();
                var scopeStack = new ScopeStack(tree, scope);
                if (scope.Cast<LocalStaticScope>().TryAddLocal(new Names.ExplicitMemberName(memberName).Key, memberDef))
                {
                    throw new Exception("bad bad bad!");
                }
                return new MemberResolveReferance(scope, memberName, memberDef, isReadonly, explicitTypeName,make);
            }
        }

        private class MemberResolveReferance : IResolveReferance<Member>
        {
            private readonly IScope scope;
            private readonly string memberName;
            private readonly Box<MemberDefinition> memberDef;
            private readonly bool isReadonly;
            private readonly ExplicitTypeName explicitTypeName;
            private readonly Func<int, IBox<MemberDefinition>, Member> make;

            public MemberResolveReferance(IScope scope, string memberName, Box<MemberDefinition> memberDef, bool isReadonly, ExplicitTypeName explicitTypeName, Func<int, IBox<MemberDefinition>, Member> make)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
                this.memberDef = memberDef ?? throw new ArgumentNullException(nameof(memberDef));
                this.isReadonly = isReadonly;
                this.explicitTypeName = explicitTypeName ?? throw new ArgumentNullException(nameof(explicitTypeName));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public Member Run(ScopeTree tree)
            {
                memberDef.Fill(new MemberDefinition(isReadonly, new ExplicitMemberName(memberName), new ScopeStack(tree, scope).GetType(explicitTypeName)));
                return make(0, memberDef);
            }
        }
    }

}