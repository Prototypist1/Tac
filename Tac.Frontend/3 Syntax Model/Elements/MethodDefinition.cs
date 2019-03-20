
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    internal class WeakMethodDefinition :
        WeakAbstractBlockDefinition<IInternalMethodDefinition>,
        Frontend._3_Syntax_Model.Elements.IMethodDefinition,
        IFrontendType<IVerifiableType>
    {
        public WeakMethodDefinition(
            IIsPossibly<IWeakTypeReferance> outputType, 
            IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> parameterDefinition,
            IIsPossibly<IFrontendCodeElement<ICodeElement>>[] body,
            IResolvableScope scope,
            IEnumerable<IIsPossibly<IFrontendCodeElement<ICodeElement>>> staticInitializers,
            bool isEntryPoint) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            IsEntryPoint = isEntryPoint;
        }
        
        public IIsPossibly<IWeakTypeReferance> InputType => ParameterDefinition.IfIs(x=> x.GetValue()).IfIs(x=>x.Type);
        public IIsPossibly<IWeakTypeReferance> OutputType { get; }
        public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> ParameterDefinition { get; }
        public bool IsEntryPoint { get; }

        public override IBuildIntention<IInternalMethodDefinition> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = MethodDefinition.Create();
            return new BuildIntention<IInternalMethodDefinition>(toBuild, () =>
            {
                maker.Build(
                    TransformerExtensions.Convert<ITypeReferance>(InputType.GetOrThrow(),context),
                    TransformerExtensions.Convert<ITypeReferance>(OutputType.GetOrThrow(),context),
                    ParameterDefinition.GetOrThrow().GetValue().GetOrThrow().Convert(context),
                    Scope.Convert(context),
                    Body.Select(x=>x.GetOrThrow().Convert(context)).ToArray(),
                    StaticInitailizers.Select(x=>x.GetOrThrow().Convert(context)).ToArray(),
                    IsEntryPoint);
            });
        }

        public override IIsPossibly<IFrontendType<IVerifiableType>> Returns() => Possibly.Is(this);

        IBuildIntention<IVerifiableType> IConvertable<IVerifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);
    }
    
    internal class MethodDefinitionMaker : IMaker<IPopulateScope<WeakMethodDefinition>>
    {
        public MethodDefinitionMaker()
        {
        }
        

        public ITokenMatching<IPopulateScope<WeakMethodDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            IPopulateScope<WeakTypeReference> input = null, output = null;
            var matching = tokenMatching
                .HasOne(
                    x=>x.Has(new KeyWordMaker("method"),out var _),
                    x=>x.Has( new KeyWordMaker("entry-point"),out var _), out var entryPoint)
                .HasSquare(x => x
                    .HasLine(y => y
                        .HasElement(z=>z
                            .HasOne( 
                                w=>w.Has(new TypeReferanceMaker(),out var _)
                                    .Has(new DoneMaker()),
                                w=>w.Has(new TypeDefinitionMaker(), out var _)
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
                .OptionalHas(new NameMaker(), out var parameterName)
                .Has(new BodyMaker(), out var body);
            if (matching
                 is IMatchedTokenMatching matched)
            {
                var isEntryPoint = entryPoint.Item == "entryPoint";

                var elements = matching.Context.ParseBlock(body);
                
                var parameterDefinition = MemberDefinitionMaker.PopulateScope(
                        parameterName?.Item ?? "input",
                        false,
                        input
                        );
                
                return TokenMatching<IPopulateScope<WeakMethodDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new MethodDefinitionPopulateScope(
                        parameterDefinition,
                        elements, 
                        output,
                        isEntryPoint)
                    );
            }

            return TokenMatching<IPopulateScope<WeakMethodDefinition>>.MakeNotMatch(
                    matching.Context);
        }

        public static IPopulateScope<WeakMethodDefinition> PopulateScope(IPopulateScope<WeakMemberReference> parameterDefinition,
                IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements,
                IPopulateScope<WeakTypeReference> output,
                bool isEntryPoint)
        {
            return new MethodDefinitionPopulateScope( parameterDefinition,
                 elements,
                 output,
                 isEntryPoint);
        }
        public static IPopulateBoxes<WeakMethodDefinition> PopulateBoxes(
                IPopulateBoxes<WeakMemberReference> parameter,
                IResolvableScope methodScope,
                IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] resolveReferance2,
                IPopulateBoxes<WeakTypeReference> output,
                Box<IIsPossibly<IFrontendType<IVerifiableType>>> box,
                bool isEntryPoint)
        {
            return new MethodDefinitionResolveReferance(
                parameter,
                methodScope,
                resolveReferance2,
                output,
                box,
                isEntryPoint);
        }


        private class MethodDefinitionPopulateScope : IPopulateScope<WeakMethodDefinition>
        {
            private readonly IPopulateScope<WeakMemberReference> parameterDefinition;
            private readonly IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements;
            private readonly IPopulateScope<WeakTypeReference> output;
            private readonly bool isEntryPoint;
            private readonly Box<IIsPossibly<IFrontendType<IVerifiableType>>> box = new Box<IIsPossibly<IFrontendType<IVerifiableType>>>();

            public MethodDefinitionPopulateScope(
                IPopulateScope<WeakMemberReference> parameterDefinition,
                IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements,
                IPopulateScope<WeakTypeReference> output,
                bool isEntryPoint
                )
            {
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.isEntryPoint = isEntryPoint;
            }

            public IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetReturnType()
            {
                return box;
            }

            public IPopulateBoxes<WeakMethodDefinition> Run(IPopulateScopeContext context)
            {

                var nextContext = context.Child();
                return new MethodDefinitionResolveReferance(
                    parameterDefinition.Run(nextContext),
                    nextContext.GetResolvableScope(),
                    elements.Select(x => x.Run(nextContext)).ToArray(),
                    output.Run(context),
                    box,
                    isEntryPoint);
            }
        }

        private class MethodDefinitionResolveReferance : IPopulateBoxes<WeakMethodDefinition>
        {
            private readonly IPopulateBoxes<WeakMemberReference> parameter;
            private readonly IResolvableScope methodScope;
            private readonly IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] lines;
            private readonly IPopulateBoxes<WeakTypeReference> output;
            private readonly Box<IIsPossibly<IFrontendType<IVerifiableType>>> box;
            private readonly bool isEntryPoint;

            public MethodDefinitionResolveReferance(
                IPopulateBoxes<WeakMemberReference> parameter,
                IResolvableScope methodScope,
                IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] resolveReferance2,
                IPopulateBoxes<WeakTypeReference> output,
                Box<IIsPossibly<IFrontendType<IVerifiableType>>> box,
                bool isEntryPoint)
            {
                this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
                this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
                lines = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.isEntryPoint = isEntryPoint;
            }

            public IIsPossibly<WeakMethodDefinition> Run(IResolveReferenceContext context)
            {
                return box.Fill(
                    Possibly.Is(
                        new WeakMethodDefinition(
                            output.Run(context),
                            parameter.Run(context).IfIs(x => x.MemberDefinition),
                            lines.Select(x => x.Run(context)).ToArray(),
                            methodScope,
                            new IIsPossibly<IFrontendCodeElement<ICodeElement>>[0], isEntryPoint)));
            }
        }
    }

    
}