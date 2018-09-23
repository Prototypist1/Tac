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

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<Member> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsType, out ExplicitTypeName typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {

                
                result = PopulateScope(matchingContext.ScopeStack.TopScope, nameToken.Item, readonlyToken != default, typeToken);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<Member> PopulateScope(IScope scope, string memberName,bool isReadonly, ExplicitTypeName explicitTypeName)
        {
            return (tree) =>
            {
                var memberDef = new Box<MemberDefinition>();
                var scopeStack = new ScopeStack(tree, scope);
                if (scope.Cast<LocalStaticScope>().TryAddLocal(new Names.ExplicitMemberName(memberName).Key, memberDef))
                {
                    throw new Exception("bad bad bad!");
                }
                return DetermineInferedTypes(scope, memberName,memberDef, isReadonly,explicitTypeName);
            };
        }

        private Steps.DetermineInferedTypes<Member> DetermineInferedTypes(IScope scope, string memberName, Box<MemberDefinition> box, bool isReadonly, ExplicitTypeName explicitTypeName)
        {
            return () =>
            {
                return ResolveReferance(scope, memberName, box, isReadonly, explicitTypeName);
            };
        }

        private Steps.ResolveReferance<Member> ResolveReferance(IScope scope, string memberName, Box<MemberDefinition> box, bool isReadonly, ExplicitTypeName explicitTypeName)
        {
            return (tree) =>
            {
                box.Fill(new MemberDefinition(isReadonly, new ExplicitMemberName(memberName), new ScopeStack(tree, scope).GetType(explicitTypeName)));
                return Make(0, box);
            };
        }
    }

}