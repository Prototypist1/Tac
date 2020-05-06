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
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>> StaticTypeDefinitionMaker = AddTypeMaker(
            () => new TypeDefinitionMaker(),
            MustBeBefore<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>> TypeDefinitionMaker = StaticTypeDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}



namespace Tac.SemanticModel
{

    internal interface IWeakTypeDefinition: IConvertableFrontendCodeElement<IInterfaceType>, IScoped {
        //IIsPossibly<IKey> Key { get; }
    }


    internal interface IIsType {
        public IFrontendType FrontendType();
    }


    internal class WeakTypeDefinition : IWeakTypeDefinition, IIsType
    {
        public WeakTypeDefinition(IBox<WeakScope> scope)//, IIsPossibly<IKey> key
        {
            //Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            lazy = new Lazy<IFrontendType>(() => new HasMembersType(Scope.GetValue()));
        }

        //public IIsPossibly<IKey> Key { get; }
        // I am not sure I agree with this
        // it is an ordered set of types, names and acccessablity modifiers
        public IBox<WeakScope> Scope { get; }

        private readonly  Lazy<IFrontendType> lazy;
        

        public IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = InterfaceType.Create();
            return new BuildIntention<IInterfaceType>(toBuild, () =>
            {
                maker.Build(Scope.GetValue().Convert(context).Members.Values.Select(x=>x.Value).ToArray());
            });
        }

        public IFrontendType FrontendType()
        {
            return lazy.Value;
        }

        public IEnumerable<IError> Validate() => Scope.GetValue().Validate();

    }


    internal class TypeDefinitionMaker : IMaker<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>>
    {
        public TypeDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .OptionalHas(new NameMaker(), out var typeName)
                .Has(new BodyMaker()).ConvertIfMatched(body=> new TypeDefinitionPopulateScope(
                       tokenMatching.Context.ParseBlock(body),
                       typeName != default ? OrType.Make<NameKey, ImplicitKey>(new NameKey(typeName.Item)) : OrType.Make<NameKey, ImplicitKey>(new ImplicitKey(Guid.NewGuid()))));
        }
        
        private class TypeDefinitionPopulateScope : ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>
        {
            private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
            private readonly IOrType< NameKey, ImplicitKey> key;

            public TypeDefinitionPopulateScope(IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError>> elements, IOrType<NameKey, ImplicitKey> typeName)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            }

            public ISetUpResult<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var type= context.TypeProblem.CreateType(scope, key,new WeakTypeDefinitionConverter());
                var typeReference = context.TypeProblem.CreateTypeReference(scope, key.SwitchReturns<IKey>(x=>x,x=>x), new WeakTypeReferenceConverter());
                elements.Select(x => x.TransformInner(y=>y.Run(type, context))).ToArray();
                return new SetUpResult<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>( new TypeReferanceMaker.TypeReferanceResolveReference(typeReference), OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(typeReference));
            }
        }
    }
}
