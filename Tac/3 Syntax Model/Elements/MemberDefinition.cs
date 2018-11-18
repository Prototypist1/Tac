using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend._2_Parser;
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
    internal class WeakMemberDefinition: ICodeElement, IMemberDefinition, IVarifiableType
    {
        public WeakMemberDefinition(bool readOnly, IKey key, IBox<IVarifiableType> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<IVarifiableType> Type { get; }
        public bool ReadOnly { get; }
        public IKey Key { get; }

        #region IMemberDefinition

        IVarifiableType IMemberDefinition.Type => Type.GetValue();

        #endregion
        
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberDefinition(this);
        }
        
        public IVarifiableType Returns()
        {
            return this;
        }
    }

    internal class MemberDefinitionMaker : IMaker<IPopulateScope<WeakMemberReferance>>
    {
        public MemberDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakMemberReferance>> TryMake(ITokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
                .Has(new TypeMaker(), out NameKey typeToken)
                .Has(new NameMaker(), out AtomicToken nameToken);
            if (matching.IsMatch)
            {
                return TokenMatching<IPopulateScope<WeakMemberReferance>>.Match(
                    matching.Tokens,
                    matching.Context,
                    new MemberDefinitionPopulateScope(nameToken.Item, readonlyToken != default, typeToken));
            }
            return TokenMatching<IPopulateScope<WeakMemberReferance>>.NotMatch(
                               matching.Tokens,
                               matching.Context);
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

        public IBox<IVarifiableType> GetReturnType()
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