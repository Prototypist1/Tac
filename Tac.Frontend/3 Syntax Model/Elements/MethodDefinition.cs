
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
using Tac.New;
using Tac.Parser;
using Tac.SemanticModel;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticMethodDefinitionMaker = AddElementMakers(
            () => new MethodDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> MethodDefinitionMaker = StaticMethodDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}


namespace Tac.SemanticModel
{

    internal class WeakMethodDefinition :
        WeakAbstractBlockDefinition<IInternalMethodDefinition>
    {
        public WeakMethodDefinition(
            IBox<IFrontendType> outputType,
            IBox<IWeakMemberDefinition> parameterDefinition,
            IBox<IFrontendCodeElement>[] body,
            IBox<WeakScope> scope,
            IEnumerable<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public IBox<IFrontendType> InputType => ParameterDefinition.GetValue().Type;
        public IBox<IFrontendType> OutputType { get; }
        public IBox<IWeakMemberDefinition> ParameterDefinition { get; }

        public override IBuildIntention<IInternalMethodDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MethodDefinition.Create();
            return new BuildIntention<IInternalMethodDefinition>(toBuild, () =>
            {
                maker.Build(
                    InputType.GetValue().ConvertTypeOrThrow(context),
                    OutputType.GetValue().ConvertTypeOrThrow(context),
                    ParameterDefinition.GetValue().Convert(context),
                    Scope.GetValue().Convert(context),
                    Body.Select(x => x.GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class MethodDefinitionMaker : IMaker<ISetUp<WeakMethodDefinition, Tpn.IValue>>
    {
        public MethodDefinitionMaker()
        {
        }


        public ITokenMatching<ISetUp<WeakMethodDefinition, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>? inputType = null, outputType = null;
            var matching = tokenMatching
                .Has(new KeyWordMaker("method"), out var _)
                .HasSquare(x => x
                    .HasLine(y => y
                        .HasElement(z => z.Has(new TypeMaker(), out inputType))
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .HasElement(z => z.Has(new TypeMaker(), out outputType))
                        .Has(new DoneMaker()))
                    .Has(new DoneMaker()))
                .OptionalHas(new NameMaker(), out var parameterName)
                .Has(new BodyMaker(), out var body);

            if (matching
                 is IMatchedTokenMatching matched)
            {
                var elements = matching.Context.ParseBlock(body!);

                return TokenMatching<ISetUp<WeakMethodDefinition, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new MethodDefinitionPopulateScope(
                        inputType!,
                        elements,
                        outputType!,
                        false,
                        parameterName!.Item)
                    );
            }

            return TokenMatching<ISetUp<WeakMethodDefinition, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


        private class MethodDefinitionPopulateScope : ISetUp<WeakMethodDefinition, Tpn.IValue>
        {
            private readonly ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> parameterDefinition;
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;
            private readonly ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> output;
            private readonly bool isEntryPoint;
            private readonly string parameterName;

            public MethodDefinitionPopulateScope(
                ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> parameterDefinition,
                ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements,
                ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> output,
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

            public ISetUpResult<WeakMethodDefinition, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var realizedInput = parameterDefinition.Run(scope, context);
                var realizedOutput = output.Run(scope, context);

                var box = new Box<IResolve<IFrontendCodeElement>[]>();
                var converter = new WeakMethodDefinitionConverter(box, isEntryPoint);
                var method = context.TypeProblem.CreateMethod(scope, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName, converter, new WeakMemberDefinitionConverter(false, new NameKey(parameterName)));

                box.Fill(elements.Select(x => x.Run(method, context).Resolve).ToArray());

                var value = context.TypeProblem.CreateValue(scope, new GenericNameKey(new NameKey("method"), new IKey[] {
                    realizedInput.SetUpSideNode.Key(),
                    realizedOutput.SetUpSideNode.Key(),
                }), new PlaceholderValueConverter());

                return new SetUpResult<WeakMethodDefinition, Tpn.IValue>(new MethodDefinitionResolveReferance(method), value);
            }
        }

        private class MethodDefinitionResolveReferance : IResolve<WeakMethodDefinition>
        {
            private readonly Tpn.TypeProblem2.Method method;

            public MethodDefinitionResolveReferance(Tpn.TypeProblem2.Method method)
            {
                this.method = method ?? throw new ArgumentNullException(nameof(method));
            }

            public IBox<WeakMethodDefinition> Run(Tpn.ITypeSolution context)
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
}