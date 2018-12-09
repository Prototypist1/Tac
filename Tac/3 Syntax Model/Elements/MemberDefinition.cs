using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
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
    internal class WeakMemberDefinition: IFrontendCodeElement, IMemberDefinition, IVarifiableType
    {
        public WeakMemberDefinition(bool readOnly, IKey key, IIsPossibly<WeakTypeReferance> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IIsPossibly<WeakTypeReferance> Type { get; }
        public bool ReadOnly { get; }
        public IKey Key { get; }

        #region IMemberDefinition

        ITypeReferance IMemberDefinition.Type => Type.GetOrThrow();

        #endregion
        
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberDefinition(this);
        }
        
        public IVarifiableType Returns()
        {
            return this;
        }

        IIsPossibly<IVarifiableType> IFrontendCodeElement.Returns()
        {
            return Possibly.Is(this);
        }
    }

    internal class MemberDefinitionMaker : IMaker<IPopulateScope<WeakMemberReference>>
    {
        public MemberDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakMemberReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
                .HasOne(w => w.Has(new TypeReferanceMaker(), out var _),
                        w => w.Has(new TypeDefinitionMaker(), out var _),
                        out var type)
                .Has(new NameMaker(), out var nameToken);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new MemberDefinitionPopulateScope(nameToken.Item, readonlyToken != default, type));
            }
            return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeNotMatch(
                               matching.Context);
        }
    }

    internal class MemberDefinitionPopulateScope : IPopulateScope<WeakMemberReference>
    {
        private readonly string memberName;
        private readonly bool isReadonly;
        private readonly IPopulateScope<WeakTypeReferance> typeName;
        private readonly Box<IIsPossibly<WeakMemberReference>> box = new Box<IIsPossibly<WeakMemberReference>>();
        private readonly Box<IIsPossibly<WeakMemberDefinition>> memberDefinitionBox = new Box<IIsPossibly<WeakMemberDefinition>>();

        public MemberDefinitionPopulateScope(string item, bool v, IPopulateScope<WeakTypeReferance> typeToken)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            typeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
        }

        public IPopulateBoxes<WeakMemberReference> Run(IPopulateScopeContext context)
        {
            var key = new NameKey(memberName);
            if (!context.Scope.TryAddMember(DefintionLifetime.Instance,key, memberDefinitionBox))
            {
                throw new Exception("bad bad bad!");
            }
            return new MemberDefinitionResolveReferance(memberName, box, isReadonly, typeName.Run(context), context.GetResolvableScope(), memberDefinitionBox);
        }

        public IBox<IIsPossibly<IVarifiableType>> GetReturnType()
        {
            return box;
        }
    }

    internal class MemberDefinitionResolveReferance : IPopulateBoxes< WeakMemberReference>
    {
        private readonly string memberName;
        private readonly Box<IIsPossibly<WeakMemberReference>> box;
        private readonly bool isReadonly;
        public readonly IPopulateBoxes<WeakTypeReferance> type;
        private readonly IResolvableScope scope;
        private readonly Box<IIsPossibly<WeakMemberDefinition>> memberDefinitionBox;

        public MemberDefinitionResolveReferance(
            string memberName,
            Box<IIsPossibly<WeakMemberReference>> box,
            bool isReadonly,
            IPopulateBoxes<WeakTypeReferance> type,
            IResolvableScope scope,
            Box<IIsPossibly<WeakMemberDefinition>> memberDefinitionBox)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.isReadonly = isReadonly;
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.memberDefinitionBox = memberDefinitionBox ?? throw new ArgumentNullException(nameof(memberDefinitionBox));
        }

        public IIsPossibly<WeakMemberReference> Run(IResolveReferenceContext context)
        {
            memberDefinitionBox.Fill(
                Possibly.Is(
                new WeakMemberDefinition(
                    isReadonly,
                    new NameKey(memberName),
                    type.Run(context))));

            return box.Fill(Possibly.Is(new WeakMemberReference(Possibly.Is(memberDefinitionBox))));
        }
    }
}