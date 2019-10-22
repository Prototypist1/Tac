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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticImplementationDefinitionMaker = AddElementMakers(
            () => new ImplementationDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> ImplementationDefinitionMaker = StaticImplementationDefinitionMaker;
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
            IIsPossibly<IWeakTypeReference> outputType, 
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
        public IIsPossibly<IWeakTypeReference> ContextTypeBox
        {
            get
            {
                return ContextDefinition.IfIs(x=>x.GetValue()).IfIs(x=> x.Type);
            }
        }
        public IIsPossibly<IWeakTypeReference> InputTypeBox
        {
            get
            {
                return ParameterDefinition.IfIs(x => x.GetValue()).IfIs(x => x.Type);
            }
        }
        public IIsPossibly<IWeakTypeReference> OutputType { get; }
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

    internal class ImplementationDefinitionMaker : IMaker<IPopulateScope<WeakImplementationDefinition,Tpn.IValue>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakImplementationDefinition,Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> context= null, input = null, output = null;

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
                

                return TokenMatching<IPopulateScope<WeakImplementationDefinition,Tpn.IValue>>.MakeMatch(
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


            return TokenMatching<IPopulateScope<WeakImplementationDefinition,Tpn.IValue>>.MakeNotMatch(match.Context);
        }
        
        public static IPopulateScope<WeakImplementationDefinition,Tpn.IValue> PopulateScope(
                                IPopulateScope<IWeakTypeReference,Tpn.ITypeReference> contextDefinition,
                IPopulateScope<IWeakTypeReference,Tpn.ITypeReference> parameterDefinition,
                IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>, Tpn.ITypeProblemNode>[] elements,
                IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> output,
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
        public static IPopulateBoxes<WeakImplementationDefinition> PopulateBoxes(
                IPopulateBoxes<IWeakTypeReference> contextDefinition,
                IPopulateBoxes<IWeakTypeReference> parameterDefinition,
                IResolvableScope methodScope,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] elements,
                IPopulateBoxes<WeakTypeReference> output)
        {
            return new ImplementationDefinitionResolveReferance(
                 methodScope,
                 contextDefinition,
                 parameterDefinition,
                 elements,
                 output);
        }
        
        private class PopulateScopeImplementationDefinition : IPopulateScope<WeakImplementationDefinition,Tpn.IValue>
        {
            private readonly IPopulateScope<IWeakTypeReference,Tpn.ITypeReference> contextDefinition;
            private readonly IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> parameterDefinition;
            private readonly IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;
            private readonly IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> output;
            private readonly string contextName;
            private readonly string parameterName;

            public PopulateScopeImplementationDefinition(
                IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> contextDefinition,
                IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> parameterDefinition,
                IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements,
                IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> output,
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

            public IResolvelizeScope<WeakImplementationDefinition,Tpn.IValue> Run(Tpn.IScope scope, IPopulateScopeContext context)
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

                return new ImplementationDefinitionFinalizeScope(
                    value,
                    realizeContext,
                    realizedInput,
                    elements.Select(y => y.Run(inner, context)).ToArray(),
                    realizedOutput
                    );
            }
        }


        private class ImplementationDefinitionFinalizeScope: IResolvelizeScope<WeakImplementationDefinition,Tpn.IValue>
        {
            private readonly IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> contextDefinition;
            private readonly IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> parameterDefinition;
            private readonly IResolvelizeScope<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;
            private readonly IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> output;

            public ImplementationDefinitionFinalizeScope(
                Tpn.IValue finalizableScope,
                IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> contextDefinition,
                IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> parameterDefinition,
                IResolvelizeScope<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements,
                IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> output)
            {
                SetUpSideNode = finalizableScope ?? throw new ArgumentNullException(nameof(finalizableScope));
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public Tpn.IValue SetUpSideNode
            {
                get;
            }

            public IPopulateBoxes<WeakImplementationDefinition> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                var scope = finalizableScope.FinalizeScope(parent);
                return new ImplementationDefinitionResolveReferance(
                    scope,
                    contextDefinition.Run(scope,context),
                    parameterDefinition.Run(scope,context),
                    elements.Select(x=>x.Run(scope,context)).ToArray(),
                    output.Run(scope,context));
            }
        }

        private class ImplementationDefinitionResolveReferance : IPopulateBoxes<WeakImplementationDefinition>
        {
            private readonly IResolvableScope methodScope;
            private readonly IPopulateBoxes<IWeakTypeReference> contextDefinition;
            private readonly IPopulateBoxes<IWeakTypeReference> parameterDefinition;
            private readonly IPopulateBoxes<IFrontendCodeElement>[] elements;
            private readonly IPopulateBoxes<IWeakTypeReference> output;

            public ImplementationDefinitionResolveReferance(
                IResolvableScope methodScope,
                IPopulateBoxes<IWeakTypeReference> contextDefinition,
                IPopulateBoxes<IWeakTypeReference> parameterDefinition,
                IPopulateBoxes<IFrontendCodeElement>[] elements,
                IPopulateBoxes<IWeakTypeReference> output)
            {
                this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public IIsPossibly<WeakImplementationDefinition> Run(IResolvableScope _, IResolveReferenceContext context)
            {
                var innerRes = new WeakImplementationDefinition(
                        contextDefinition.Run(methodScope,context).IfIs(x => x.MemberDefinition),
                        parameterDefinition.Run(methodScope,context).IfIs(x => x.MemberDefinition),
                        output.Run(methodScope,context),
                        elements.Select(x => x.Run(methodScope,context)).ToArray(),
                        methodScope,
                        new IConvertableFrontendCodeElement<ICodeElement>[0]);

                var res = Possibly.Is(innerRes);

                return res;
            }
        }
    }
}
