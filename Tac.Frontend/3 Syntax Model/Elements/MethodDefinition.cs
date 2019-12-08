
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend._3_Syntax_Model.Elements;
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticMethodDefinitionMaker = AddElementMakers(
            () => new MethodDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> MethodDefinitionMaker = StaticMethodDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model
{

    internal class WeakMethodDefinition :
        WeakAbstractBlockDefinition<IInternalMethodDefinition>,
        IValueDefinition
    {
        public WeakMethodDefinition(
            IBox<IFrontendType> outputType, 
            IBox<IWeakMemberDefinition> parameterDefinition,
            IIsPossibly<IFrontendCodeElement>[] body,
            WeakScope scope,
            IEnumerable<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers,
            bool isEntryPoint) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            IsEntryPoint = isEntryPoint;
        }
        
        public IBox<IFrontendType> InputType => ParameterDefinition.GetValue().Type;
        public IBox<IFrontendType> OutputType { get; }
        public IBox<IWeakMemberDefinition> ParameterDefinition { get; }
        public bool IsEntryPoint { get; }

        public override IBuildIntention<IInternalMethodDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MethodDefinition.Create();
            return new BuildIntention<IInternalMethodDefinition>(toBuild, () =>
            {
                maker.Build(
                    InputType.GetValue().ConvertTypeOrThrow(context),
                    OutputType.GetValue().ConvertTypeOrThrow(context),
                    ParameterDefinition.GetValue().Convert(context),
                    Scope.Convert(context),
                    Body.Select(x=>x.GetOrThrow().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x=>x.GetOrThrow().ConvertElementOrThrow(context)).ToArray(),
                    IsEntryPoint);
            });
        }

        public override IIsPossibly<IFrontendType> Returns() => Possibly.Is(this);
    }
    
    internal class MethodDefinitionMaker : IMaker<ISetUp<WeakMethodDefinition, LocalTpn.IValue>>
    {
        public MethodDefinitionMaker()
        {
        }


        public ITokenMatching<ISetUp<WeakMethodDefinition, LocalTpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            
            {
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> inputType = null, outputType = null;
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
                    var elements = matching.Context.ParseBlock(body);
                    
                    return TokenMatching<ISetUp<WeakMethodDefinition, LocalTpn.IValue>>.MakeMatch(
                        matched.Tokens,
                        matched.Context,
                        new MethodDefinitionPopulateScope(
                            inputType,
                            elements,
                            outputType,
                            false,
                            parameterName)
                        );
                }
            }
            {
                var matching = tokenMatching
                    .Has(new KeyWordMaker("entry-point"), out var _)
                    .Has(new BodyMaker(), out var body);
                if (matching
                     is IMatchedTokenMatching matched)
                {
                    var elements = matching.Context.ParseBlock(body);


                    return TokenMatching<ISetUp<WeakMethodDefinition, LocalTpn.IValue>>.MakeMatch(
                        matched.Tokens,
                        matched.Context,
                        new MethodDefinitionPopulateScope(
                            TypeReferanceMaker.PopulateScope(new NameKey("empty")),
                            elements,
                            TypeReferanceMaker.PopulateScope(new NameKey("empty")),
                            true)
                        );
                }

                return TokenMatching<ISetUp<WeakMethodDefinition, LocalTpn.IValue>>.MakeNotMatch(
                        matching.Context);
            }

        }


        private class MethodDefinitionPopulateScope : ISetUp<WeakMethodDefinition, LocalTpn.IValue>
        {
            private readonly ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> parameterDefinition;
            private readonly ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements;
            private readonly ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> output;
            private readonly bool isEntryPoint;
            private readonly string parameterName;

            public MethodDefinitionPopulateScope(
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> parameterDefinition,
                ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> output,
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

            public ISetUpResult<WeakMethodDefinition, LocalTpn.IValue> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                var realizedInput = parameterDefinition.Run(scope, context);
                var realizedOutput = output.Run(scope, context);


                var method= context.TypeProblem.CreateMethod(scope, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName);

                var value = context.TypeProblem.CreateValue(scope, new GenericNameKey(new NameKey("method"), new IKey[] {
                    realizedInput.SetUpSideNode.Key(),
                    realizedOutput.SetUpSideNode.Key(),
                }), new PlaceholderValueConverter());

                return new SetUpResult<WeakMethodDefinition, LocalTpn.IValue>( new MethodDefinitionResolveReferance(
                    realizedInput.Resolve,
                    elements.Select(x => x.Run(method, context).Resolve).ToArray(),
                    realizedOutput.Resolve,
                    isEntryPoint),value);
            }
        }

        private class MethodDefinitionResolveReferance : IResolve<WeakMethodDefinition>
        {
            private readonly IResolve<WeakMemberReference> parameter;
            private readonly IResolve<IFrontendCodeElement>[] lines;
            private readonly IResolve<IFrontendType> output;
            private readonly bool isEntryPoint;

            public MethodDefinitionResolveReferance(
                IResolve<WeakMemberReference> parameter,
                IResolve<IFrontendCodeElement>[] resolveReferance2,
                IResolve<IFrontendType> output,
                bool isEntryPoint)
            {
                this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
                lines = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.isEntryPoint = isEntryPoint;
            }

            public IIsPossibly<WeakMethodDefinition> Run(LocalTpn.ITypeSolution context)
            {
                return 
                    Possibly.Is(
                        new WeakMethodDefinition(
                            output.Run(context),
                            parameter.Run(context).IfIs(x => x.MemberDefinition),
                            lines.Select(x => x.Run(context)).ToArray(),
                            methodScope,
                            new IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>[0], isEntryPoint));
            }
        }
    }

    
}