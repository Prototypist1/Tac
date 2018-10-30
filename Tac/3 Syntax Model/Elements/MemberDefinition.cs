using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;
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
    internal class WeakMemberDefinition: ICodeElement, IMemberDefinition
    {
        public WeakMemberDefinition(bool readOnly, IKey key, IBox<IType> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<IType> Type { get; }
        public bool ReadOnly { get; }
        public IKey Key { get; }

        #region IMemberDefinition

        IType IMemberDefinition.Type => Type.GetValue();

        #endregion
        
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberDefinition(this);
        }
        
        public IType Returns()
        {
            return this;
        }
    }

    internal class MemberDefinitionMaker : IMaker<WeakMemberReferance>
    {
        public MemberDefinitionMaker()
        {
        }
        

        public IResult<IPopulateScope<WeakMemberReferance>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .OptionalHas(ElementMatcher.KeyWord("readonly"), out var readonlyToken)
                .Has(ElementMatcher.IsType, out NameKey typeToken)
                .Has(ElementMatcher.IsName, out AtomicToken nameToken)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new MemberDefinitionPopulateScope(nameToken.Item, readonlyToken != default, typeToken));
            }
            return ResultExtension.Bad<IPopulateScope<WeakMemberReferance>>();
        }
    }

    internal class MemberDefinitionPopulateScope : IPopulateScope< WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly bool isReadonly;
        private readonly NameKey typeName;
        private readonly Box<WeakMemberReferance> box = new Box<WeakMemberReferance>();
        private readonly Box<WeakMemberDefinition> memberDefinitionBox = new Box<WeakMemberDefinition>();

        public MemberDefinitionPopulateScope(string item, bool v, NameKey typeToken)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            typeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {
            var key = new NameKey(memberName);
            if (!context.Scope.TryAddMember(DefintionLifetime.Instance,key, memberDefinitionBox))
            {
                throw new Exception("bad bad bad!");
            }
            return new MemberDefinitionResolveReferance(memberName, box, isReadonly, typeName, context.GetResolvableScope(), memberDefinitionBox);
        }

        public IBox<IType> GetReturnType()
        {
            return box;
        }
    }

    internal class MemberDefinitionResolveReferance : IPopulateBoxes< WeakMemberReferance>
    {
        private readonly string memberName;
        private readonly Box<WeakMemberReferance> box;
        private readonly bool isReadonly;
        public readonly NameKey typeName;
        private readonly IResolvableScope scope;
        private readonly Box<WeakMemberDefinition> memberDefinitionBox;

        public MemberDefinitionResolveReferance(
            string memberName,
            Box<WeakMemberReferance> box,
            bool isReadonly,
            NameKey explicitTypeName,
            IResolvableScope scope,
            Box<WeakMemberDefinition> memberDefinitionBox)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.isReadonly = isReadonly;
            typeName = explicitTypeName ?? throw new ArgumentNullException(nameof(explicitTypeName));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.memberDefinitionBox = memberDefinitionBox ?? throw new ArgumentNullException(nameof(memberDefinitionBox));
        }

        public WeakMemberReferance Run(IResolveReferanceContext context)
        {
            memberDefinitionBox.Fill(
                new WeakMemberDefinition(
                    isReadonly,
                    new NameKey(memberName),
                    scope.GetTypeOrThrow(typeName)));

            return box.Fill(new WeakMemberReferance(memberDefinitionBox));
        }
    }
    
}