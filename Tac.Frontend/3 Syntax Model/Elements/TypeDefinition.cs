using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticTypeDefinitionMaker = AddElementMakers(
            () => new TypeDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> TypeDefinitionMaker = StaticTypeDefinitionMaker;
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
        //IIsPossibly<IKey> Key { get; }
    }

    internal class WeakTypeDefinition : IWeakTypeDefinition
    {
        public WeakTypeDefinition(WeakScope scope)//, IIsPossibly<IKey> key
        {
            //Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        //public IIsPossibly<IKey> Key { get; }
        // I am not sure I agree with this
        // it is an ordered set of types, names and acccessablity modifiers
        public WeakScope Scope { get; }
        
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


    internal class TypeDefinitionMaker : IMaker<ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference>>
    {
        public TypeDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .OptionalHas(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);

            if (matching is IMatchedTokenMatching matched)
            {
               var elements = tokenMatching.Context.ParseBlock(body);
                
               return TokenMatching<ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TypeDefinitionPopulateScope(
                       elements, 
                       typeName != default ? new NameKey(typeName.Item).Cast<IKey>(): new ImplicitKey()));
            }

            return TokenMatching<ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    matching.Context);
        }
        
        public static ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> PopulateScope(ISetUp<IConvertableFrontendCodeElement<ICodeElement>, LocalTpn.ITypeProblemNode>[] elements, IKey typeName)
        {
            return new TypeDefinitionPopulateScope(elements, typeName);
        }
        
        private class TypeDefinitionPopulateScope : ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference>
        {
            private readonly ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements;
            private readonly IKey key;
            private readonly Box<IIsPossibly<WeakTypeDefinition>> definitionBox = new Box<IIsPossibly<WeakTypeDefinition>>();

            public TypeDefinitionPopulateScope(ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements, IKey typeName)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            }

            public ISetUpResult<IFrontendType, LocalTpn.TypeProblem2.TypeReference> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                var type= context.TypeProblem.CreateType(scope, key);
                var typeReference = context.TypeProblem.CreateTypeReference(scope, key);
                elements.Select(x => x.Run(type, context)).ToArray();
                return new SetUpResult<IFrontendType, LocalTpn.TypeProblem2.TypeReference>( new TypeDefinitionResolveReference(
                    definitionBox,
                    typeReference,
                    key), typeReference);
            }
        }

        private class TypeDefinitionResolveReference : IResolve<IFrontendType>
        {
            private readonly Box<IIsPossibly<WeakTypeDefinition>> definitionBox;
            private readonly WeakTypeReference typeReferance;
            private readonly IKey key;

            public TypeDefinitionResolveReference(
                Box<IIsPossibly<WeakTypeDefinition>> definitionBox,
                WeakTypeReference typeReferance,
                IKey key)
            {
                this.definitionBox = definitionBox ?? throw new ArgumentNullException(nameof(definitionBox));
                this.typeReferance = typeReferance ?? throw new ArgumentNullException(nameof(typeReferance));
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IIsPossibly<IFrontendType> Run(LocalTpn.ITypeSolution context)
            {
                definitionBox.Fill(Possibly.Is(new WeakTypeDefinition(resolvableScope, Possibly.Is(key))));
                return Possibly.Is(typeReferance);
            }
        }
    }

}
