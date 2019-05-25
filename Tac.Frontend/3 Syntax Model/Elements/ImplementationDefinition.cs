using System;
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
using Tac.Semantic_Model.CodeStuff;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model
{

    internal class WeakImplementationDefinition: IConvertableFrontendCodeElement<IImplementationDefinition>, IConvertableFrontendType<IVerifiableType>
    {

        public WeakImplementationDefinition(
            IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> contextDefinition,
            IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> parameterDefinition,
            IIsPossibly<WeakTypeReference> outputType, 
            IEnumerable<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> metohdBody,
            IResolvableScope scope, 
            IEnumerable<IConvertableFrontendCodeElement<ICodeElement>> staticInitializers)
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
        public IIsPossibly<WeakTypeReference> OutputType { get; }
        // are these really boxes
        public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> ContextDefinition { get; }
        public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> ParameterDefinition { get; }
        public IResolvableScope Scope { get; }
        public IEnumerable<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> MethodBody { get; }
        public IEnumerable<IConvertableFrontendCodeElement<ICodeElement>> StaticInitialzers { get; }

        IIsPossibly<IConvertableFrontendType<IVerifiableType>> IConvertableFrontendCodeElement<IImplementationDefinition>.Returns()
        {
            return Possibly.Is(this);
        }

        public IBuildIntention<IImplementationDefinition> GetBuildIntention(ConversionContext context)
        {
            var (toBuild, maker) = ImplementationDefinition.Create();
            return new BuildIntention<IImplementationDefinition>(toBuild, () =>
            {
                maker.Build(
                    TransformerExtensions.Convert<ITypeReferance>(OutputType.GetOrThrow(),context),
                    ContextDefinition.IfIs(x=>x.GetValue()).GetOrThrow().Convert(context),
                    ParameterDefinition.IfIs(x => x.GetValue()).GetOrThrow().Convert(context),
                    Scope.Convert(context),
                    MethodBody.Select(x => x.GetOrThrow().Convert(context)).ToArray(),
                    StaticInitialzers.Select(x => x.Convert(context)).ToArray());
            });
        }

        IBuildIntention<IVerifiableType> IConvertable<IVerifiableType>.GetBuildIntention(ConversionContext context) => GetBuildIntention(context);
    }

    internal class ImplementationDefinitionMaker : IMaker<IPopulateScope<WeakImplementationDefinition>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakImplementationDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            IPopulateScope<WeakTypeReference> context= null, input = null, output = null;

            var match = tokenMatching
                .Has(new KeyWordMaker("implementation"), out var _)
                .HasSquare(x => x
                    .HasLine(y=>y
                        .HasElement(z=>z
                            .HasOne(
                                w => w.Has(new TypeReferanceMaker(), out var _)
                                    .Has(new DoneMaker()),
                                w => w.Has(new TypeDefinitionMaker(), out var _)
                                    .Has(new DoneMaker()),
                                out context))
                         .Has(new DoneMaker()))
                    .HasLine(y => y
                        .HasElement(z => z
                            .HasOne(
                                w => w.Has(new TypeReferanceMaker(), out var _)
                                    .Has(new DoneMaker()),
                                w => w.Has(new TypeDefinitionMaker(), out var _)
                                    .Has(new DoneMaker()),
                                out input))
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .HasElement(z => z
                           .HasOne(
                                w => w.Has(new TypeReferanceMaker(), out var _)
                                    .Has(new DoneMaker()),
                                w => w.Has(new TypeDefinitionMaker(), out var _)
                                    .Has(new DoneMaker()),
                                out output))
                        .Has(new DoneMaker()))
                    .Has(new DoneMaker()))
                .OptionalHas(new NameMaker(), out AtomicToken contextName)
                .OptionalHas(new NameMaker(), out AtomicToken parameterName)
                .Has(new BodyMaker(), out CurleyBracketToken body);
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
                IPopulateBoxes<WeakTypeReference> output,
                Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> box)
        {
            return new ImplementationDefinitionResolveReferance(
                 contextDefinition,
                 parameterDefinition,
                 methodScope,
                 elements,
                 output,
                 box);
        }
        
        private class PopulateScopeImplementationDefinition : IPopulateScope<WeakImplementationDefinition>
        {
            private readonly IPopulateScope<WeakMemberReference> contextDefinition;
            private readonly IPopulateScope<WeakMemberReference> parameterDefinition;
            private readonly IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements;
            private readonly IPopulateScope<WeakTypeReference> output;
            private readonly Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> box = new Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>();

            public PopulateScopeImplementationDefinition(
                IPopulateScope<WeakMemberReference> contextDefinition,
                IPopulateScope<WeakMemberReference> parameterDefinition,
                IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements,
                IPopulateScope<WeakTypeReference> output)
            {
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public IPopulateBoxes<WeakImplementationDefinition> Run(IPopulateScopeContext context)
            {

                var nextContext = context.Child();
                return new ImplementationDefinitionResolveReferance(
                    contextDefinition.Run(nextContext),
                    parameterDefinition.Run(nextContext),
                    nextContext.GetResolvableScope(),
                    elements.Select(x => x.Run(nextContext)).ToArray(),
                    output.Run(context),
                    box);
            }

            public IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> GetReturnType()
            {
                return box;
            }

        }

        private class ImplementationDefinitionResolveReferance : IPopulateBoxes<WeakImplementationDefinition>
        {
            private readonly IPopulateBoxes<WeakMemberReference> contextDefinition;
            private readonly IPopulateBoxes<WeakMemberReference> parameterDefinition;
            private readonly IResolvableScope methodScope;
            private readonly IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] elements;
            private readonly IPopulateBoxes<WeakTypeReference> output;
            private readonly Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> box;

            public ImplementationDefinitionResolveReferance(
                IPopulateBoxes<WeakMemberReference> contextDefinition,
                IPopulateBoxes<WeakMemberReference> parameterDefinition,
                IResolvableScope methodScope,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] elements,
                IPopulateBoxes<WeakTypeReference> output,
                Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> box)
            {
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
            }

            public IIsPossibly<WeakImplementationDefinition> Run(IResolveReferenceContext context)
            {
                return box.Fill(
                    Possibly.Is(
                        new WeakImplementationDefinition(
                        contextDefinition.Run(context).IfIs(x => x.MemberDefinition),
                        parameterDefinition.Run(context).IfIs(x => x.MemberDefinition),
                        output.Run(context),
                        elements.Select(x => x.Run(context)).ToArray(),
                        methodScope,
                        new IConvertableFrontendCodeElement<ICodeElement>[0])));
            }
        }
    }



}
