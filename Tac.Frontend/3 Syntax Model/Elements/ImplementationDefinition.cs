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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticImplementationDefinitionMaker = AddElementMakers(
            () => new ImplementationDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> ImplementationDefinitionMaker = StaticImplementationDefinitionMaker;
#pragma warning disable IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model
{

    internal class WeakImplementationDefinition: IConvertableFrontendCodeElement<IImplementationDefinition>, IFrontendType
    {

        public WeakImplementationDefinition(
            IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> contextDefinition,
            IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> parameterDefinition,
            IIsPossibly<IFrontendType> outputType, 
            IEnumerable<IIsPossibly<IFrontendCodeElement>> metohdBody,
            IResolvableScope scope, 
            IEnumerable<IFrontendCodeElement> staticInitializers)
        {
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBody = metohdBody ?? throw new ArgumentNullException(nameof(metohdBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialzers = staticInitializers ?? throw new ArgumentNullException(nameof(staticInitializers));
        }

        // dang! these could also be inline definitions 
        public IIsPossibly<IFrontendType> ContextTypeBox
        {
            get
            {
                return ContextDefinition.IfIs(x=>x.GetValue()).IfIs(x=> x.Type);
            }
        }
        public IIsPossibly<IFrontendType> InputTypeBox
        {
            get
            {
                return ParameterDefinition.IfIs(x => x.GetValue()).IfIs(x => x.Type);
            }
        }
        public IIsPossibly<IFrontendType> OutputType { get; }
        // are these really boxes
        public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> ContextDefinition { get; }
        public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> ParameterDefinition { get; }
        public IResolvableScope Scope { get; }
        public IEnumerable<IIsPossibly<IFrontendCodeElement>> MethodBody { get; }
        public IEnumerable<IFrontendCodeElement> StaticInitialzers { get; }

        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is(this);
        }

        public IBuildIntention<IImplementationDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ImplementationDefinition.Create();
            return new BuildIntention<IImplementationDefinition>(toBuild, () =>
            {
                maker.Build(
                    TransformerExtensions.Convert<IVerifiableType>(OutputType.GetOrThrow(),context),
                    ContextDefinition.IfIs(x=>x.GetValue()).GetOrThrow().Convert(context),
                    ParameterDefinition.IfIs(x => x.GetValue()).GetOrThrow().Convert(context),
                    Scope.Convert(context),
                    MethodBody.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitialzers.Select(x => x.ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class ImplementationDefinitionMaker : IMaker<ISetUp<WeakImplementationDefinition, LocalTpn.IValue>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<WeakImplementationDefinition, LocalTpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> context= null, input = null, output = null;

            var match = tokenMatching
                .Has(new KeyWordMaker("implementation"), out var _)
                .HasSquare(x => x
                    .HasLine(y=>y
                        .HasElement(z=>z.Has(new TypeMaker(), out context))
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .HasElement(z => z.Has(new TypeMaker(), out input))
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .HasElement(z => z.Has(new TypeMaker(), out output))
                        .Has(new DoneMaker()))
                    .Has(new DoneMaker()))
                .OptionalHas(new NameMaker(), out var contextName)
                .OptionalHas(new NameMaker(), out var parameterName)
                .Has(new BodyMaker(), out var body);
            if (match is IMatchedTokenMatching matched)
            {
                var elements = tokenMatching.Context.ParseBlock(body);
                

                return TokenMatching<ISetUp<WeakImplementationDefinition, LocalTpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new PopulateScopeImplementationDefinition(
                        context,
                        input, 
                        elements,
                        output,
                        contextName?.Item ?? "context",
                        parameterName?.Item ?? "input"));
            }


            return TokenMatching<ISetUp<WeakImplementationDefinition, LocalTpn.IValue>>.MakeNotMatch(match.Context);
        }
        
        public static ISetUp<WeakImplementationDefinition, LocalTpn.IValue> PopulateScope(
                                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> contextDefinition,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> parameterDefinition,
                ISetUp<IConvertableFrontendCodeElement<ICodeElement>, LocalTpn.ITypeProblemNode>[] elements,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> output,
                string contextName,
                string parameterName)
        {
            return new PopulateScopeImplementationDefinition(
                                 contextDefinition,
                 parameterDefinition,
                 elements,
                 output,
                 contextName,
                 parameterName);
        }
        
        private class PopulateScopeImplementationDefinition : ISetUp<WeakImplementationDefinition, LocalTpn.IValue>
        {
            private readonly ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> contextDefinition;
            private readonly ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> parameterDefinition;
            private readonly ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements;
            private readonly ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> output;
            private readonly string contextName;
            private readonly string parameterName;

            public PopulateScopeImplementationDefinition(
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> contextDefinition,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> parameterDefinition,
                ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> output,
                string contextName,
                string parameterName)
            {
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.contextName = contextName ?? throw new ArgumentNullException(nameof(contextName));
                this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            }

            public ISetUpResult<WeakImplementationDefinition,LocalTpn.IValue> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                var realizeContext = contextDefinition.Run(scope, context);
                var realizedInput = parameterDefinition.Run(scope, context);
                var realizedOutput = output.Run(scope, context);
                var outputTypeRef = context.TypeProblem.CreateTypeReference(scope, new GenericNameKey(new NameKey("method"),new[] {
                    realizedInput.SetUpSideNode.Key(),
                    realizedOutput.SetUpSideNode.Key(),
                }));

                var outer = context.TypeProblem.CreateMethod(scope, realizeContext.SetUpSideNode, outputTypeRef, contextName);

                var inner = context.TypeProblem.CreateMethod(outer, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName);

                var innerValue = context.TypeProblem.CreateValue(outer, 
                    new GenericNameKey(new NameKey("method"), new[] {
                         realizedInput.SetUpSideNode.Key(),
                         realizedOutput.SetUpSideNode.Key(),
                    }));

                innerValue.AssignTo(outer.Returns());

                var value = context.TypeProblem.CreateValue(scope, new GenericNameKey(new NameKey("method"), new[] {
                    realizeContext.SetUpSideNode.Key(),
                    new GenericNameKey(new NameKey("method"), new[] {
                         realizedInput.SetUpSideNode.Key(),
                         realizedOutput.SetUpSideNode.Key(),
                    }),
                }));

                return new SetUpResult<WeakImplementationDefinition, LocalTpn.IValue>(new ImplementationDefinitionResolveReferance(
                    realizeContext.Resolve,
                    realizedInput.Resolve,
                    elements.Select(y => y.Run(inner, context).Resolve).ToArray(),
                    realizedOutput.Resolve
                    ), value);
            }
        }


        private class ImplementationDefinitionResolveReferance : IResolve<WeakImplementationDefinition>
        {
            private readonly IResolve<IFrontendType> contextDefinition;
            private readonly IResolve<IFrontendType> parameterDefinition;
            private readonly IResolve<IFrontendCodeElement>[] elements;
            private readonly IResolve<IFrontendType> output;

            public ImplementationDefinitionResolveReferance(
                IResolve<IFrontendType> contextDefinition,
                IResolve<IFrontendType> parameterDefinition,
                IResolve<IFrontendCodeElement>[] elements,
                IResolve<IFrontendType> output)
            {
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public IIsPossibly<WeakImplementationDefinition> Run(IResolveContext context)
            {
                var innerRes = new WeakImplementationDefinition(
                        contextDefinition.Run(context).IfIs(x => x.MemberDefinition),
                        parameterDefinition.Run(context).IfIs(x => x.MemberDefinition),
                        output.Run(context),
                        elements.Select(x => x.Run(context)).ToArray(),
                        methodScope,
                        new IConvertableFrontendCodeElement<ICodeElement>[0]);

                var res = Possibly.Is(innerRes);

                return res;
            }
        }
    }
}
