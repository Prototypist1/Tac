﻿using Prototypist.Toolbox.Object;
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
        private static readonly WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>> StaticTypeDefinitionMaker = AddTypeMaker(
            () => new TypeDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>> TypeDefinitionMaker = StaticTypeDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}



namespace Tac.SemanticModel
{

    internal interface IWeakTypeDefinition: IConvertableFrontendCodeElement<IInterfaceType>, IScoped {
    }


    internal class WeakTypeDefinition : IWeakTypeDefinition//, IIsType
    {
        public WeakTypeDefinition(IOrType< IBox<WeakScope>,IError> scope)//, IIsPossibly<IKey> key
        {
            //Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            lazy = new Lazy<IOrType<IFrontendType<IInterfaceType>, IError>>(() => 
                Scope.SwitchReturns(
                    x => OrType.Make<IFrontendType<IInterfaceType>, IError>( new HasMembersType(x.GetValue())),
                    x => OrType.Make<IFrontendType<IInterfaceType>, IError>(x)));
        }

        //public IIsPossibly<IKey> Key { get; }
        // I am not sure I agree with this
        // it is an ordered set of types, names and acccessablity modifiers
        public IOrType<IBox<WeakScope>, IError> Scope { get; }

        private readonly Lazy<IOrType<IFrontendType<IInterfaceType>, IError>> lazy;

        private class DummyBuildIntention : IBuildIntention<IInterfaceType>
        {
            public DummyBuildIntention(IInterfaceType tobuild)
            {
                Tobuild = tobuild ?? throw new ArgumentNullException(nameof(tobuild));
            }

            public Action Build => () => { };

            public IInterfaceType Tobuild { get; init; }
        }
        public IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context)
        {
            // this and the HasMembersType type it holds both convert to the same IInterfaceType
            return new DummyBuildIntention(lazy.Value.Is1OrThrow().Convert(context));
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> FrontendType()
        {
            return lazy.Value;
        }

        public IEnumerable<IError> Validate() => Scope.SwitchReturns(x=>x.GetValue().Validate(),x=>new IError[] { x});

    }

    internal class TypeDefinitionMaker : IMaker<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>>
    {
        public TypeDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .OptionalHas(new NameMaker(), out var typeName)
                .Has(new BodyMaker()).ConvertIfMatched(body=> new TypeDefinitionPopulateScope(
                       tokenMatching.Context.ParseType(body),
                       typeName != default ? OrType.Make<NameKey, ImplicitKey>(new NameKey(typeName.Item)) : OrType.Make<NameKey, ImplicitKey>(new ImplicitKey(Guid.NewGuid()))), tokenMatching);
        }
    }

    internal class TypeDefinitionPopulateScope : ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>
    {
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>> elements;
        private readonly IOrType<NameKey, ImplicitKey> key;

        public TypeDefinitionPopulateScope(IReadOnlyList<IOrType<ISetUp<IBox<WeakMemberReference>, Tpn.ITypeProblemNode>, IError>> elements, IOrType<NameKey, ImplicitKey> typeName)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            key = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public ISetUpResult<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            var type = context.TypeProblem.CreateType(scope, key, new WeakTypeDefinitionConverter());
            var typeReference = context.TypeProblem.CreateTypeReference(scope, key.SwitchReturns<IKey>(x => x, x => x), new WeakTypeReferenceConverter());
            elements.Select(x => x.TransformInner(y => y.Run(type, context.CreateChildContext(this)))).ToArray();
            return new SetUpResult<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>(new TypeReferanceResolveReference(typeReference), OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(typeReference));
        }
    }
}
