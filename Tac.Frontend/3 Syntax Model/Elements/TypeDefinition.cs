using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> StaticTypeDefinitionMaker = AddElementMakers(
            () => new TypeDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> TypeDefinitionMaker = StaticTypeDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}



namespace Tac.Semantic_Model
{

    internal class OverlayTypeDefinition : IWeakTypeDefinition
    {
        private readonly IWeakTypeDefinition backing;

        public OverlayTypeDefinition(IWeakTypeDefinition backing, Overlay overlay)
        {
            if (overlay == null)
            {
                throw new ArgumentNullException(nameof(overlay));
            }

            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            Scope = new OverlayedScope(backing.Scope,overlay);
        }
        public IIsPossibly<IKey> Key => backing.Key;
        public IResolvableScope Scope { get; }

        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is(this);
        }

        public IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = InterfaceType.Create();
            return new BuildIntention<IInterfaceType>(toBuild, () =>
            {
                maker.Build(Scope.Convert(context).Members);
            });
        }
    }

    internal interface IWeakTypeDefinition: IConvertableFrontendCodeElement<IInterfaceType>, IScoped, IConvertableFrontendType<IInterfaceType> {
        IIsPossibly<IKey> Key { get; }
    }

    internal class WeakTypeDefinition : IWeakTypeDefinition
    {
        public WeakTypeDefinition(IResolvableScope scope, IIsPossibly<IKey> key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IIsPossibly<IKey> Key { get; }
        // I am not sure I agree with this
        // it is an ordered set of types, names and acccessablity modifiers
        public IResolvableScope Scope { get; }
        
        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is(this);
        }

        public IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = InterfaceType.Create();
            return new BuildIntention<IInterfaceType>(toBuild, () =>
            {
                maker.Build(Scope.Convert(context).Members);
            });
        }
    }


    internal class TypeDefinitionMaker : IMaker<IPopulateScope<IWeakTypeReference, ISetUpType>>
    {
        public TypeDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<IWeakTypeReference, ISetUpType>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .OptionalHas(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);

            if (matching is IMatchedTokenMatching matched)
            {
               var elements = tokenMatching.Context.ParseBlock(body);
                
               return TokenMatching<IPopulateScope<IWeakTypeReference, ISetUpType>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TypeDefinitionPopulateScope(
                       elements, 
                       typeName != default ? new NameKey(typeName.Item).Cast<IKey>(): new ImplicitKey()));
            }

            return TokenMatching<IPopulateScope<WeakTypeReference, ISetUpType>>.MakeNotMatch(
                    matching.Context);
        }
        
        public static IPopulateScope<IWeakTypeReference,ISetUpType> PopulateScope(IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>,ISetUpSideNode>[] elements, IKey typeName)
        {
            return new TypeDefinitionPopulateScope(elements, typeName);
        }
        public static IPopulateBoxes<IWeakTypeReference> PopulateBoxes(IResolvableScope scope,
                Box<IIsPossibly<WeakTypeDefinition>> definitionBox,
                WeakTypeReference typeReferance,
                IKey key)
        {
            return new TypeDefinitionResolveReference(scope,
                definitionBox,
                typeReferance,
                key);
        }
        
        private class TypeDefinitionPopulateScope : IPopulateScope<IWeakTypeReference, ISetUpType>
        {
            private readonly IPopulateScope<IFrontendCodeElement,ISetUpSideNode>[] elements;
            private readonly IKey key;
            private readonly Box<IIsPossibly<WeakTypeDefinition>> definitionBox = new Box<IIsPossibly<WeakTypeDefinition>>();
            private readonly WeakTypeReference typeReferance;

            public TypeDefinitionPopulateScope(IPopulateScope<IFrontendCodeElement,ISetUpSideNode>[] elements, IKey typeName)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                key = typeName ?? throw new ArgumentNullException(nameof(typeName));
                typeReferance = new WeakTypeReference(Possibly.Is(definitionBox));
            }

            public IResolvelizeScope<IWeakTypeReference, ISetUpType> Run(IDefineMembers scope, IPopulateScopeContext context)
            {
                var type= context.TypeProblem.CreateType(scope, key);
                scope.Type(type);
                // what the heck? these are not passed anywhere?
                elements.Select(x => x.Run(type, context)).ToArray();
                return new TypeDefinitionFinalizeScope(
                    type,
                    definitionBox,
                    typeReferance,
                    key);
            }
        }

        private class TypeDefinitionFinalizeScope : IResolvelizeScope<IWeakTypeReference, ISetUpType>
        {
            private readonly IResolvelizableScope finalizableScope;
            private readonly Box<IIsPossibly<WeakTypeDefinition>> definitionBox;
            private readonly WeakTypeReference typeReferance;
            private readonly IKey key;

            public TypeDefinitionFinalizeScope(
                ISetUpType finalizableScope,
                Box<IIsPossibly<WeakTypeDefinition>> definitionBox,
                WeakTypeReference typeReferance,
                IKey key)
            {
                SetUpSideNode = finalizableScope ?? throw new ArgumentNullException(nameof(finalizableScope));
                this.definitionBox = definitionBox ?? throw new ArgumentNullException(nameof(definitionBox));
                this.typeReferance = typeReferance ?? throw new ArgumentNullException(nameof(typeReferance));
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public ISetUpType SetUpSideNode
            {
                get;
            }

            public IPopulateBoxes<IWeakTypeReference> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                var finalScope = finalizableScope.FinalizeScope(parent);
                return new TypeDefinitionResolveReference(finalScope, definitionBox, typeReferance, key);
            }
        }

        private class TypeDefinitionResolveReference : IPopulateBoxes<IWeakTypeReference>
        {
            private readonly IResolvableScope resolvableScope;
            private readonly Box<IIsPossibly<WeakTypeDefinition>> definitionBox;
            private readonly WeakTypeReference typeReferance;
            private readonly IKey key;

            public TypeDefinitionResolveReference(
                IResolvableScope resolvableScope,
                Box<IIsPossibly<WeakTypeDefinition>> definitionBox,
                WeakTypeReference typeReferance,
                IKey key)
            {
                this.resolvableScope = resolvableScope ?? throw new ArgumentNullException(nameof(resolvableScope));
                this.definitionBox = definitionBox ?? throw new ArgumentNullException(nameof(definitionBox));
                this.typeReferance = typeReferance ?? throw new ArgumentNullException(nameof(typeReferance));
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IIsPossibly<IWeakTypeReference> Run(IResolvableScope _, IResolveReferenceContext context)
            {
                definitionBox.Fill(Possibly.Is(new WeakTypeDefinition(resolvableScope, Possibly.Is(key))));
                return Possibly.Is(typeReferance);
            }
        }
    }

}
