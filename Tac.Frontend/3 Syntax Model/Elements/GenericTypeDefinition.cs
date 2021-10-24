
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.Parser;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticGenericTypeDefinitionMaker = AddElementMakers(
            () => new GenericTypeDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> GenericTypeDefinitionMaker = StaticGenericTypeDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}



namespace Tac.SemanticModel
{

    // I am a little thrown off by the fact that this doesn't convert
    // why doesn't this convert?
    internal class WeakGenericTypeDefinition : IFrontendCodeElement//, IIsType //: //IWeakGenericTypeDefinition
    {
        public WeakGenericTypeDefinition(
            IIsPossibly<IOrType<NameKey, ImplicitKey>> key,
            IOrType<HasMembersType, IError> type,
            IOrType<IGenericTypeParameterPlacholder, IError>[] TypeParameterDefinitions)
        {
            this.TypeParameterDefinitions = TypeParameterDefinitions ?? throw new ArgumentNullException(nameof(TypeParameterDefinitions));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            Scope = type.TransformInner(x => x.weakScope);
        }

        public IOrType<IGenericTypeParameterPlacholder, IError>[] TypeParameterDefinitions { get; }
        public IIsPossibly<IOrType<NameKey, ImplicitKey>> Key { get; }
        public IOrType<WeakScope, IError> Scope { get; }


        private readonly IOrType<HasMembersType, IError> type;

        public IOrType<IFrontendType<IVerifiableType>, IError> FrontendType()
        {
            return type;
        }

        public IEnumerable<IError> Validate() => Scope.SwitchReturns(x => x.Validate(), x => new IError[] { x });
    }

    internal class GenericTypeDefinitionMaker : IMaker<ISetUp<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType>>
    {

        public GenericTypeDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .Has(new DefineGenericNMaker())
                .Has(new NameMaker())
                .Has(new BodyMaker())
                .ConvertIfMatched((_,generics, name, lines) => {
                    return new GenericTypeDefinitionPopulateScope(
                        new NameKey(name.Item),
                        tokenMatching.Context.ParseBlock(lines),
                        generics.Select(x => new NameKey(x)).ToArray());
                }, tokenMatching);
        }

        //public static ISetUp<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType> PopulateScope(
        //        NameKey nameKey,
        //        IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> lines,
        //        IGenericTypeParameterPlacholder[] genericParameters)
        //{
        //    return new GenericTypeDefinitionPopulateScope(
        //        nameKey,
        //        lines,
        //        genericParameters);
        //}

        
    }

    internal class GenericTypeDefinitionPopulateScope : ISetUp<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType>
    {
        private readonly NameKey nameKey;
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> lines;
        private readonly NameKey[] genericParameters;

        public GenericTypeDefinitionPopulateScope(
            NameKey nameKey,
            IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> lines,
            NameKey[] genericParameters)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
        }

        public ISetUpResult<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            // oh geez here is a mountain.
            // I generic types are erased 
            // what on earth does this return?
            var myScope = context.TypeProblem.CreateGenericType(
                scope,
                OrType.Make<NameKey, ImplicitKey>(nameKey),
                genericParameters.Select(x => new Tpn.TypeAndConverter(x, new WeakTypeDefinitionConverter())).ToArray(),
                new WeakTypeDefinitionConverter());
            var nextLines = lines.Select(x => x.TransformInner(y => y.Run(myScope, context.CreateChildContext(this)).Resolve)).ToArray();
            return new SetUpResult<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType>(new GenericTypeDefinitionResolveReferance(myScope, nextLines), OrType.Make<Tpn.IExplicitType, IError>(myScope));
        }
    }

    internal class GenericTypeDefinitionResolveReferance : IResolve<IBox<WeakGenericTypeDefinition>>
    {
        private readonly Tpn.TypeProblem2.Type myScope;
        private readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] lines;

        public GenericTypeDefinitionResolveReferance(Tpn.TypeProblem2.Type myScope, IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] lines)
        {
            this.myScope = myScope;
            this.lines = lines;
        }


        public IBox<WeakGenericTypeDefinition> Run(Tpn.TypeSolution context)
        {
            // uhhh it is werid that I have to do this
            lines.Select(x => x.TransformInner(y => y.Run(context))).ToArray();

            return new Box<WeakGenericTypeDefinition>(myScope.Converter.Convert(context, myScope).Is2OrThrow());
        }
    }
}
