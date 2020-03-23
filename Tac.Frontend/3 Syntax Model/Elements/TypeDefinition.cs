using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;
using Prototypist.Toolbox;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>> StaticTypeDefinitionMaker = AddTypeMaker(
            () => new TypeDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendType, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>> TypeDefinitionMaker = StaticTypeDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}



namespace Tac.SemanticModel
{

    internal interface IWeakTypeDefinition: IConvertableFrontendCodeElement<IInterfaceType>, IScoped, IConvertableFrontendType<IInterfaceType> {
        //IIsPossibly<IKey> Key { get; }
    }

    internal class WeakTypeDefinition : IWeakTypeDefinition
    {
        public WeakTypeDefinition(IBox<WeakScope> scope)//, IIsPossibly<IKey> key
        {
            //Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        //public IIsPossibly<IKey> Key { get; }
        // I am not sure I agree with this
        // it is an ordered set of types, names and acccessablity modifiers
        public IBox<WeakScope> Scope { get; }
        

        public IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = InterfaceType.Create();
            return new BuildIntention<IInterfaceType>(toBuild, () =>
            {
                maker.Build(Scope.GetValue().Convert(context).Members.Values.Select(x=>x.Value).ToArray());
            });
        }
    }


    internal class TypeDefinitionMaker : IMaker<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>
    {
        public TypeDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .OptionalHas(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);

            if (matching is IMatchedTokenMatching matched)
            {
               var elements = tokenMatching.Context.ParseBlock(body!);
                
               return TokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TypeDefinitionPopulateScope(
                       elements, 
                       typeName != default ? new NameKey(typeName.Item).CastTo<IKey>(): new ImplicitKey(Guid.NewGuid())));
            }

            return TokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(
                    matching.Context);
        }
        
        private class TypeDefinitionPopulateScope : ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>
        {
            private readonly IReadOnlyList<OrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError>> elements;
            private readonly IKey key;

            public TypeDefinitionPopulateScope(IReadOnlyList<OrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>,IError>> elements, IKey typeName)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            }

            public ISetUpResult<IFrontendType, Tpn.TypeProblem2.TypeReference> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var type= context.TypeProblem.CreateType(scope, key,new WeakTypeDefinitionConverter());
                var typeReference = context.TypeProblem.CreateTypeReference(scope, key, new WeakTypeReferenceConverter());
                elements.Select(x => x.Convert(y=>y.Run(type, context))).ToArray();
                return new SetUpResult<IFrontendType, Tpn.TypeProblem2.TypeReference>( new TypeReferanceMaker.TypeReferanceResolveReference(typeReference), new OrType<Tpn.TypeProblem2.TypeReference, IError>(typeReference));
            }
        }
    }
}
