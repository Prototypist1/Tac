
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticMethodDefinitionMaker = AddElementMakers(
            () => new MethodDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> MethodDefinitionMaker = StaticMethodDefinitionMaker;
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
            IIsPossibly<IWeakTypeReference> outputType, 
            IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> parameterDefinition,
            IIsPossibly<IFrontendCodeElement>[] body,
            IResolvableScope scope,
            IEnumerable<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers,
            bool isEntryPoint) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            IsEntryPoint = isEntryPoint;
        }
        
        public IIsPossibly<IWeakTypeReference> InputType => ParameterDefinition.IfIs(x=> x.GetValue()).IfIs(x=>x.Type);
        public IIsPossibly<IWeakTypeReference> OutputType { get; }
        public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> ParameterDefinition { get; }
        public bool IsEntryPoint { get; }

        public override IBuildIntention<IInternalMethodDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MethodDefinition.Create();
            return new BuildIntention<IInternalMethodDefinition>(toBuild, () =>
            {
                maker.Build(
                    InputType.GetOrThrow().TypeDefinition.GetOrThrow().GetValue().GetOrThrow().ConvertTypeOrThrow(context),
                    OutputType.GetOrThrow().TypeDefinition.GetOrThrow().GetValue().GetOrThrow().ConvertTypeOrThrow(context),
                    ParameterDefinition.GetOrThrow().GetValue().GetOrThrow().Convert(context),
                    Scope.Convert(context),
                    Body.Select(x=>x.GetOrThrow().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x=>x.GetOrThrow().ConvertElementOrThrow(context)).ToArray(),
                    IsEntryPoint);
            });
        }

        public override IIsPossibly<IFrontendType> Returns() => Possibly.Is(this);
    }
    
    internal class MethodDefinitionMaker : IMaker<ISetUp<WeakMethodDefinition,Tpn.IValue>>
    {
        public MethodDefinitionMaker()
        {
        }


        public ITokenMatching<ISetUp<WeakMethodDefinition, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            
            {
                ISetUp<IWeakTypeReference, Tpn.ITypeReference> inputType = null, outputType = null;
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
                    
                    return TokenMatching<ISetUp<WeakMethodDefinition,Tpn.IValue>>.MakeMatch(
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


                    return TokenMatching<ISetUp<WeakMethodDefinition,Tpn.IValue>>.MakeMatch(
                        matched.Tokens,
                        matched.Context,
                        new MethodDefinitionPopulateScope(
                            TypeReferanceMaker.PopulateScope(new NameKey("empty")),
                            elements,
                            TypeReferanceMaker.PopulateScope(new NameKey("empty")),
                            true)
                        );
                }

                return TokenMatching<ISetUp<WeakMethodDefinition,Tpn.IValue>>.MakeNotMatch(
                        matching.Context);
            }

        }


        private class MethodDefinitionPopulateScope : ISetUp<WeakMethodDefinition,Tpn.IValue>
        {
            private readonly ISetUp<IWeakTypeReference, Tpn.ITypeReference> parameterDefinition;
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;
            private readonly ISetUp<IWeakTypeReference, Tpn.ITypeReference> output;
            private readonly bool isEntryPoint;
            private readonly string parameterName;

            public MethodDefinitionPopulateScope(
                ISetUp<IWeakTypeReference,Tpn.ITypeReference> parameterDefinition,
                ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements,
                ISetUp<IWeakTypeReference, Tpn.ITypeReference> output,
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

            public ISetUpResult<WeakMethodDefinition,Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var realizedInput = parameterDefinition.Run(scope, context);
                var realizedOutput = output.Run(scope, context);


                var method= context.TypeProblem.CreateMethod(scope, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName);

                var value = context.TypeProblem.CreateValue(scope, new GenericNameKey(new NameKey("method"), new IKey[] {
                    realizedInput.SetUpSideNode.Key(),
                    realizedOutput.SetUpSideNode.Key(),
                }));

                return new SetUpResult<WeakMethodDefinition, Tpn.IValue>( new MethodDefinitionResolveReferance(
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
            private readonly IResolve<IWeakTypeReference> output;
            private readonly bool isEntryPoint;

            public MethodDefinitionResolveReferance(
                IResolve<WeakMemberReference> parameter,
                IResolve<IFrontendCodeElement>[] resolveReferance2,
                IResolve<IWeakTypeReference> output,
                bool isEntryPoint)
            {
                this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
                lines = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.isEntryPoint = isEntryPoint;
            }

            public IIsPossibly<WeakMethodDefinition> Run(IResolvableScope _, IResolveContext context)
            {
                return 
                    Possibly.Is(
                        new WeakMethodDefinition(
                            output.Run(methodScope,context),
                            parameter.Run(methodScope,context).IfIs(x => x.MemberDefinition),
                            lines.Select(x => x.Run(methodScope,context)).ToArray(),
                            methodScope,
                            new IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>[0], isEntryPoint));
            }
        }
    }

    
}