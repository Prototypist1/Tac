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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> StaticImplementationDefinitionMaker = AddElementMakers(
            () => new ImplementationDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> ImplementationDefinitionMaker = StaticImplementationDefinitionMaker;
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

    internal class ImplementationDefinitionMaker : IMaker<IPopulateScope<WeakImplementationDefinition,Tpn.IMethod>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakImplementationDefinition,Tpn.IMethod>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            IPopulateScope<IWeakTypeReference, ISetUpTypeReference> context= null, input = null, output = null;

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
                
                var contextNameString = contextName?.Item ?? "context";
                var contextDefinition = MemberDefinitionMaker.PopulateScope(
                        new NameKey( contextNameString),
                        false,
                        context
                        );
                
                var parameterNameString = parameterName?.Item ?? "input";
                var parameterDefinition = MemberDefinitionMaker.PopulateScope(
                        new NameKey( parameterNameString),
                        false,
                        input
                        );


                var resultDefinition = MemberDefinitionMaker.PopulateScope(
                        new ImplicitKey(),
                        false,
                        output
                        );

                return TokenMatching<IPopulateScope<WeakImplementationDefinition,Tpn.IMethod>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new PopulateScopeImplementationDefinition(
                        contextDefinition, 
                        parameterDefinition, 
                        elements,
                        resultDefinition));
            }


            return TokenMatching<IPopulateScope<WeakImplementationDefinition,Tpn.IMethod>>.MakeNotMatch(match.Context);
        }
        
        public static IPopulateScope<WeakImplementationDefinition,Tpn.IMethod> PopulateScope(
                                IPopulateScope<WeakMemberReference,Tpn.IMember> contextDefinition,
                IPopulateScope<WeakMemberReference,Tpn.IMember> parameterDefinition,
                IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>, ITypeProblemNode>[] elements,
                IPopulateScope<WeakMemberReference, Tpn.IMember> output)
        {
            return new PopulateScopeImplementationDefinition(
                                 contextDefinition,
                 parameterDefinition,
                 elements,
                 output);
        }
        public static IPopulateBoxes<WeakImplementationDefinition> PopulateBoxes(
                IPopulateBoxes<WeakMemberReference> contextDefinition,
                IPopulateBoxes<WeakMemberReference> parameterDefinition,
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
        
        private class PopulateScopeImplementationDefinition : IPopulateScope<WeakImplementationDefinition,Tpn.IMethod>
        {
            private readonly IPopulateScope<WeakMemberReference,Tpn.IMember> contextDefinition;
            private readonly IPopulateScope<WeakMemberReference, Tpn.IMember> parameterDefinition;
            private readonly IPopulateScope<IFrontendCodeElement, ITypeProblemNode>[] elements;
            private readonly IPopulateScope<WeakMemberReference, Tpn.IMember> output;

            public PopulateScopeImplementationDefinition(
                IPopulateScope<WeakMemberReference, Tpn.IMember> contextDefinition,
                IPopulateScope<WeakMemberReference, Tpn.IMember> parameterDefinition,
                IPopulateScope<IFrontendCodeElement, ITypeProblemNode>[] elements,
                IPopulateScope<WeakMemberReference, Tpn.IMember> output)
            {
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public IResolvelizeScope<WeakImplementationDefinition,Tpn.IMethod> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {
                var outer = context.TypeProblem.CreateMethod(scope);

                var inner = context.TypeProblem.CreateMethod(outer);

                var realizeContext = contextDefinition.Run(outer, context);
                var outerResultMember = context.TypeProblem.CreateMember(new ImplicitKey());
                var outerMethod = outer.SetInputOutput(realizeContext.SetUpSideNode, outerResultMember);

                var parameter = parameterDefinition.Run(scope, context);
                var outputResolves = output.Run(scope, context);

                var innerMethod = inner.SetInputOutput(parameter.SetUpSideNode, outputResolves.SetUpSideNode);

                outerMethod.AssignToReturns(innerMethod);

                return new ImplementationDefinitionFinalizeScope(
                    outerMethod,
                    realizeContext,
                    parameter,
                    elements.Select(y => y.Run(innerMethod, context)).ToArray(),
                    outputResolves
                    );
            }
        }


        private class ImplementationDefinitionFinalizeScope: IResolvelizeScope<WeakImplementationDefinition,Tpn.IMethod>
        {
            private readonly IResolvelizeScope<WeakMemberReference, Tpn.IMember> contextDefinition;
            private readonly IResolvelizeScope<WeakMemberReference, Tpn.IMember> parameterDefinition;
            private readonly IResolvelizeScope<IFrontendCodeElement, ITypeProblemNode>[] elements;
            private readonly IResolvelizeScope<WeakMemberReference, Tpn.IMember> output;

            public ImplementationDefinitionFinalizeScope(
                Tpn.IMethod finalizableScope,
                IResolvelizeScope<WeakMemberReference, Tpn.IMember> contextDefinition,
                IResolvelizeScope<WeakMemberReference, Tpn.IMember> parameterDefinition,
                IResolvelizeScope<IFrontendCodeElement,ITypeProblemNode>[] elements,
                IResolvelizeScope<WeakMemberReference, Tpn.IMember> output)
            {
                SetUpSideNode = finalizableScope ?? throw new ArgumentNullException(nameof(finalizableScope));
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public Tpn.IMethod SetUpSideNode
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
            private readonly IPopulateBoxes<WeakMemberReference> contextDefinition;
            private readonly IPopulateBoxes<WeakMemberReference> parameterDefinition;
            private readonly IPopulateBoxes<IFrontendCodeElement>[] elements;
            private readonly IPopulateBoxes<IWeakTypeReference> output;

            public ImplementationDefinitionResolveReferance(
                IResolvableScope methodScope,
                IPopulateBoxes<WeakMemberReference> contextDefinition,
                IPopulateBoxes<WeakMemberReference> parameterDefinition,
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
