using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement>> StaticTypeDefinitionMaker = AddElementMakers(
            () => new TypeDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement>>(typeof(MemberMaker)));
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement>> TypeDefinitionMaker = StaticTypeDefinitionMaker;
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

        public IBuildIntention<IInterfaceType> GetBuildIntention(TransformerExtensions.ConversionContext context)
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

        public IBuildIntention<IInterfaceType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = InterfaceType.Create();
            return new BuildIntention<IInterfaceType>(toBuild, () =>
            {
                maker.Build(Scope.Convert(context).Members);
            });
        }
    }


    internal class TypeDefinitionMaker : IMaker<IPopulateScope<IWeakTypeReference>>
    {
        public TypeDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<IWeakTypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .OptionalHas(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);

            if (matching is IMatchedTokenMatching matched)
            {
               var elements = tokenMatching.Context.ParseBlock(body);
                
               return TokenMatching<IPopulateScope<IWeakTypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TypeDefinitionPopulateScope(
                       elements, 
                       typeName != default ? new NameKey(typeName.Item).Cast<IKey>(): new ImplicitKey()));
            }

            return TokenMatching<IPopulateScope<WeakTypeReference>>.MakeNotMatch(
                    matching.Context);
        }
        
        public static IPopulateScope<IWeakTypeReference> PopulateScope(IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements, IKey typeName)
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
        
        private class TypeDefinitionPopulateScope : IPopulateScope<IWeakTypeReference>
        {
            private readonly IPopulateScope<IFrontendCodeElement>[] elements;
            private readonly IKey key;
            private readonly Box<IIsPossibly<WeakTypeDefinition>> definitionBox = new Box<IIsPossibly<WeakTypeDefinition>>();
            private readonly WeakTypeReference typeReferance;
            private readonly Box<IIsPossibly<WeakTypeReference>> box;

            public TypeDefinitionPopulateScope(IPopulateScope<IFrontendCodeElement>[] elements, IKey typeName)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                key = typeName ?? throw new ArgumentNullException(nameof(typeName));
                typeReferance = new WeakTypeReference(Possibly.Is(definitionBox));
                box = new Box<IIsPossibly<WeakTypeReference>>(Possibly.Is(typeReferance));
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return box;
            }

            public IPopulateBoxes<IWeakTypeReference> Run(IPopulateScopeContext context)
            {
                var encolsing = context.Scope.TryAddType(key, box);
                var nextContext = context.Child();
                elements.Select(x => x.Run(nextContext)).ToArray();
                return new TypeDefinitionResolveReference(
                    nextContext.GetResolvableScope(),
                    definitionBox,
                    typeReferance,
                    key);
            }
        }

        private class TypeDefinitionResolveReference : IPopulateBoxes<IWeakTypeReference>
        {
            private readonly IResolvableScope scope;
            private readonly Box<IIsPossibly<WeakTypeDefinition>> definitionBox;
            private readonly WeakTypeReference typeReferance;
            private readonly IKey key;

            public TypeDefinitionResolveReference(
                IResolvableScope scope,
                Box<IIsPossibly<WeakTypeDefinition>> definitionBox,
                WeakTypeReference typeReferance,
                IKey key)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.definitionBox = definitionBox ?? throw new ArgumentNullException(nameof(definitionBox));
                this.typeReferance = typeReferance ?? throw new ArgumentNullException(nameof(typeReferance));
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IIsPossibly<IWeakTypeReference> Run(IResolveReferenceContext context)
            {
                definitionBox.Fill(Possibly.Is(new WeakTypeDefinition(scope, Possibly.Is(key))));
                return Possibly.Is(typeReferance);
            }
        }
    }

}
