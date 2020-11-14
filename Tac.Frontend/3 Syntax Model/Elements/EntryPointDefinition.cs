using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticEntryPointDefinitionMaker = AddElementMakers(
            () => new EntryPointDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> EntryPointDefinitionMaker = StaticEntryPointDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel
{

    internal class WeakEntryPointDefinition :
        WeakAbstractBlockDefinition<IEntryPointDefinition>
    {
        public WeakEntryPointDefinition(
            IBox<IOrType<IFrontendType, IError>> outputType,
            IBox<WeakMemberDefinition> parameterDefinition,
            IOrType<IBox<IFrontendCodeElement>,IError>[] body,
            IOrType<IBox<WeakScope>, IError> scope,
            IReadOnlyList<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
                        OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public IBox<IOrType<IFrontendType, IError>> InputType => ParameterDefinition.GetValue().Type;
        public IBox<IOrType<IFrontendType, IError>> OutputType { get; }
        public IBox<WeakMemberDefinition> ParameterDefinition { get; }

        public override IBuildIntention<IEntryPointDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = EntryPointDefinition.Create();
            return new BuildIntention<IEntryPointDefinition>(toBuild, () =>
            {
                maker.Build(
                    OutputType.GetValue().Is1OrThrow().ConvertTypeOrThrow(context),
                    ParameterDefinition.GetValue().Convert(context),
                    Scope.Is1OrThrow().GetValue().Convert(context),
                    Body.Select(x => x.Is1OrThrow().GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class EntryPointDefinitionMaker : IMaker<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>>
    {
        public EntryPointDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>> TryMake(IMatchedTokenMatching tokenMatching)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> inputType = null, outputType = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            var matching = tokenMatching
                .Has(new KeyWordMaker("entry-point"), out var _)
                .HasSquare(x => x
                    .HasLine(y => y
                        .Has(new TypeMaker(), out inputType)
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .Has(new TypeMaker(), out outputType)
                        .Has(new DoneMaker()))
                    .Has(new DoneMaker()))
                .OptionalHas(new NameMaker(), out var parameterName)
                .Has(new BodyMaker(), out var body);

            if (matching
                 is IMatchedTokenMatching matched)
            {
                var elements = matching.Context.ParseBlock(body);

                return TokenMatching<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>>.MakeMatch(
                    tokenMatching,
                    new EntryPointDefinitionPopulateScope(
                        new MemberDefinitionPopulateScope(new NameKey(parameterName!.Item),false, inputType!),
                        elements,
                        outputType!
                        ),
                    matched.EndIndex
                    );
            }

            return TokenMatching<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>>.MakeNotMatch(
                    matching.Context);
        }
    }


    internal class EntryPointDefinitionPopulateScope : ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>
    {
        private readonly MemberDefinitionPopulateScope parameterDefinition;
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
        private readonly ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> output;

        public EntryPointDefinitionPopulateScope(
            MemberDefinitionPopulateScope parameterDefinition,
            IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements,
            ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> output
            )
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public ISetUpResult<IBox<WeakEntryPointDefinition>, Tpn.IScope> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            var box = new Box<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[]>();

            var inputBox = new Box<IResolve<IBox<WeakMemberReference>>>();
            var outputBox = new Box<IResolve<IBox<IFrontendType>>>();

            var innerScope = context.TypeProblem.CreateScope(scope, new WeakEntryPointConverter(box, inputBox, outputBox));
            context.TypeProblem.HasEntryPoint(scope, innerScope);

            inputBox.Fill(parameterDefinition.Run(innerScope, context.CreateChildContext(this)).Resolve);
            outputBox.Fill(output.Run(innerScope, context.CreateChildContext(this)).Resolve);

            box.Fill(elements.Select(x =>
                x.SwitchReturns(
                    y => OrType.Make<IResolve<IBox<IFrontendCodeElement>>, IError>(y.Run(innerScope, context.CreateChildContext(this)).Resolve),
                    y => OrType.Make<IResolve<IBox<IFrontendCodeElement>>, IError>(y)))
            .ToArray());

            return new SetUpResult<IBox<WeakEntryPointDefinition>, Tpn.IScope>(new EntryPointDefinitionResolveReferance(innerScope), OrType.Make<Tpn.IScope, IError>(innerScope));
        }
    }

    internal class EntryPointDefinitionResolveReferance : IResolve<IBox<WeakEntryPointDefinition>>
    {
        private readonly Tpn.TypeProblem2.Scope scope;

        public EntryPointDefinitionResolveReferance(Tpn.TypeProblem2.Scope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IBox<WeakEntryPointDefinition> Run(Tpn.TypeSolution context)
        {
            var res = context.GetScope(scope);
            if (res.GetValue().Is3(out var v3))
            {
                return new Box<WeakEntryPointDefinition>(v3);
            }
            throw new Exception("wrong!");
        }
    }
}


