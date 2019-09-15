﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement>> StaticImplementationDefinitionMaker = AddElementMakers(
            () => new ImplementationDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement>> ImplementationDefinitionMaker = StaticImplementationDefinitionMaker;
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

    internal class ImplementationDefinitionMaker : IMaker<IPopulateScope<WeakImplementationDefinition>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakImplementationDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            IPopulateScope<IWeakTypeReference> context= null, input = null, output = null;

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
                        contextNameString,
                        false,
                        context
                        );
                
                var parameterNameString = parameterName?.Item ?? "input";
                var parameterDefinition = MemberDefinitionMaker.PopulateScope(
                        parameterNameString,
                        false,
                        input
                        );
                
                return TokenMatching<IPopulateScope<WeakImplementationDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new PopulateScopeImplementationDefinition(
                        contextDefinition, 
                        parameterDefinition, 
                        elements,
                        output));
            }


            return TokenMatching<IPopulateScope<WeakImplementationDefinition>>.MakeNotMatch(match.Context);
        }
        
        public static IPopulateScope<WeakImplementationDefinition> PopulateScope(
                                IPopulateScope<WeakMemberReference> contextDefinition,
                IPopulateScope<WeakMemberReference> parameterDefinition,
                IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements,
                IPopulateScope<WeakTypeReference> output)
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
        
        private class PopulateScopeImplementationDefinition : IPopulateScope<WeakImplementationDefinition>
        {
            private readonly IPopulateScope<WeakMemberReference> contextDefinition;
            private readonly IPopulateScope<WeakMemberReference> parameterDefinition;
            private readonly IPopulateScope<IFrontendCodeElement>[] elements;
            private readonly IPopulateScope<IWeakTypeReference> output;

            public PopulateScopeImplementationDefinition(
                IPopulateScope<WeakMemberReference> contextDefinition,
                IPopulateScope<WeakMemberReference> parameterDefinition,
                IPopulateScope<IFrontendCodeElement>[] elements,
                IPopulateScope<IWeakTypeReference> output)
            {
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public IResolvelizeScope<WeakImplementationDefinition> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {

                var myScope = scope.AddChild();
                return new ImplementationDefinitionFinalizeScope(
                    myScope.GetResolvelizableScope(),
                    contextDefinition.Run(myScope, context),
                    parameterDefinition.Run(myScope, context),
                    elements.Select(x => x.Run(myScope, context)).ToArray(),
                    output.Run(myScope, context)
                    );
            }
        }


        private class ImplementationDefinitionFinalizeScope: IResolvelizeScope<WeakImplementationDefinition>
        {
            private readonly IResolvelizableScope finalizableScope;
            private readonly IResolvelizeScope<WeakMemberReference> contextDefinition;
            private readonly IResolvelizeScope<WeakMemberReference> parameterDefinition;
            private readonly IResolvelizeScope<IFrontendCodeElement>[] elements;
            private readonly IResolvelizeScope<IWeakTypeReference> output;

            public ImplementationDefinitionFinalizeScope(
                IResolvelizableScope finalizableScope,
                IResolvelizeScope<WeakMemberReference> contextDefinition,
                IResolvelizeScope<WeakMemberReference> parameterDefinition,
                IResolvelizeScope<IFrontendCodeElement>[] elements,
                IResolvelizeScope<IWeakTypeReference> output)
            {
                this.finalizableScope = finalizableScope ?? throw new ArgumentNullException(nameof(finalizableScope));
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
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
