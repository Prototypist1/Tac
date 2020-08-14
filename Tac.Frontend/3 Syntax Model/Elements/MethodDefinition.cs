
using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox< IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticMethodDefinitionMaker = AddElementMakers(
            () => new MethodDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> MethodDefinitionMaker = StaticMethodDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}


namespace Tac.SemanticModel
{

    internal class WeakMethodDefinition :
        WeakAbstractBlockDefinition<IInternalMethodDefinition>, IReturn
    {
        public WeakMethodDefinition(
            IBox<IOrType<IFrontendType, IError>> outputType,
            IBox<WeakMemberDefinition> parameterDefinition,
            IReadOnlyList<IOrType< IBox<IFrontendCodeElement>,IError>> body,
            IOrType<IBox<WeakScope>, IError> scope,
            IReadOnlyList<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public IBox<IOrType<IFrontendType,IError>> InputType => ParameterDefinition.GetValue().Type;
        public IBox<IOrType<IFrontendType, IError>> OutputType { get; }
        public IBox<WeakMemberDefinition> ParameterDefinition { get; }

        public override IBuildIntention<IInternalMethodDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MethodDefinition.Create();
            return new BuildIntention<IInternalMethodDefinition>(toBuild, () =>
            {
                maker.Build(
                    InputType.GetValue().Is1OrThrow().ConvertTypeOrThrow(context),
                    OutputType.GetValue().Is1OrThrow().ConvertTypeOrThrow(context),
                    ParameterDefinition.GetValue().Convert(context),
                    Scope.Is1OrThrow().GetValue().Convert(context),
                    Body.Select(x => x.Is1OrThrow().GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }

        public IOrType<IFrontendType, IError> Returns()
        {
            // TODO
            // are there really frontend types that arn't convertable?
            return OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.MethodType(
                InputType.GetValue().TransformInner(x=>x),
                OutputType.GetValue().TransformInner(x=>x)
                ));
        }
    }

    internal class MethodDefinitionMaker : IMaker<ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>>
    {
        public MethodDefinitionMaker()
        {
        }


        public ITokenMatching<ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> inputType = null, outputType = null;

            var matching = tokenMatching
                .Has(new KeyWordMaker("method"), out var _)
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

                return TokenMatching<ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>>.MakeMatch(
                    matched.AllTokens,
                    matched.Context,
                    new MethodDefinitionPopulateScope(
                        inputType,
                        elements,
                        outputType,
                        false,
                        parameterName!.Item),
                    matched.StartIndex,
                    matched.EndIndex
                    );
            }

            return TokenMatching<ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


    }


    internal class MethodDefinitionPopulateScope : ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>
    {
        private readonly ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> parameterDefinition;
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
        private readonly ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> output;
        private readonly bool isEntryPoint;
        private readonly string parameterName;

        public MethodDefinitionPopulateScope(
            ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> parameterDefinition,
            IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements,
            ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> output,
            bool isEntryPoint,
            string parameterName
            )
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.isEntryPoint = isEntryPoint;
            this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }

        public ISetUpResult<IBox<WeakMethodDefinition>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            scope = scope.EnterInitizaionScopeIfNessisary();
            if (!(scope is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            var realizedInput = parameterDefinition.Run(scope, context.CreateChild(this));
            var realizedOutput = output.Run(scope, context.CreateChild(this));

            var box = new Box<IReadOnlyList<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>>>();
            var converter = new WeakMethodDefinitionConverter(box, isEntryPoint);
            var method = context.TypeProblem.CreateMethod(scope, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName, converter, new WeakMemberDefinitionConverter(false, new NameKey(parameterName)));

            box.Fill(elements.Select(x => x.TransformInner(y => y.Run(method, context.CreateChild(this)).Resolve)).ToArray());

            var value = context.TypeProblem.CreateValue(runtimeScope, new GenericNameKey(new NameKey("method"), new IOrType<IKey, IError>[] {
                    realizedInput.SetUpSideNode.TransformInner(x=>x.Key()),
                    realizedOutput.SetUpSideNode.TransformInner(x=>x.Key()),
                }), new PlaceholderValueConverter());

            return new SetUpResult<IBox<WeakMethodDefinition>, Tpn.IValue>(new MethodDefinitionResolveReferance(method), OrType.Make<Tpn.IValue, IError>(value));
        }
    }

    internal class MethodDefinitionResolveReferance : IResolve<IBox<WeakMethodDefinition>>
    {
        private readonly Tpn.TypeProblem2.Method method;

        public MethodDefinitionResolveReferance(Tpn.TypeProblem2.Method method)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public IBox<WeakMethodDefinition> Run(Tpn.TypeSolution context)
        {
            var res = context.GetMethod(method);
            if (res.GetValue().Is1(out var v1))
            {
                return new Box<WeakMethodDefinition>(v1);
            }
            throw new Exception("wrong!");
        }
    }
}