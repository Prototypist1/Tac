
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
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    internal class WeakMethodDefinition : WeakAbstractBlockDefinition<IMethodDefinition>, IFrontendType<IVarifiableType>
    {
        public WeakMethodDefinition(
            IIsPossibly<WeakTypeReferance> outputType, 
            IIsPossibly<IBox<IIsPossibly<WeakMemberDefinition>>> parameterDefinition,
            IIsPossibly<IFrontendCodeElement<ICodeElement>>[] body,
            IResolvableScope scope,
            IEnumerable<IIsPossibly<IFrontendCodeElement<ICodeElement>>> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }
        
        public IIsPossibly<WeakTypeReferance> InputType => ParameterDefinition.IfIs(x=> x.GetValue()).IfIs(x=>x.Type);
        public IIsPossibly<WeakTypeReferance> OutputType { get; }
        public IIsPossibly<IBox<IIsPossibly<WeakMemberDefinition>>> ParameterDefinition { get; }


        public override IBuildIntention<IMethodDefinition> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = MethodDefinition.Create();
            return new BuildIntention<IMethodDefinition>(toBuild, () =>
            {
                maker.Build(
                    TransformerExtensions.Convert<ITypeReferance>(InputType.GetOrThrow(),context),
                    TransformerExtensions.Convert<ITypeReferance>(OutputType.GetOrThrow(),context),
                    ParameterDefinition.GetOrThrow().GetValue().GetOrThrow().Convert(context),
                    Scope.Convert(context),
                    Body.Select(x=>x.GetOrThrow().Convert(context)).ToArray(),
                    StaticInitailizers.Select(x=>x.GetOrThrow().Convert(context)).ToArray());
            });
        }

        public override IIsPossibly<IFrontendType<IVarifiableType>> Returns() => Possibly.Is(this);

        IBuildIntention<IVarifiableType> IConvertable<IVarifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);
    }


    internal class MethodDefinitionMaker : IMaker<IPopulateScope<WeakMethodDefinition>>
    {
        public MethodDefinitionMaker()
        {
        }
        

        public ITokenMatching<IPopulateScope<WeakMethodDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            IPopulateScope<WeakTypeReferance> input = null, output = null;
            var matching = tokenMatching
                .Has(new KeyWordMaker("method"), out var _)
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
                var elements = matching.Context.ParseBlock(body);
                
                var parameterDefinition = new MemberDefinitionPopulateScope(
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
                        output));
            }

            return TokenMatching<IPopulateScope<WeakMethodDefinition>>.MakeNotMatch(
                    matching.Context);
        }
    }

    internal class MethodDefinitionPopulateScope : IPopulateScope<WeakMethodDefinition>
    {
        private readonly IPopulateScope<WeakMemberReference> parameterDefinition;
        private readonly IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements;
        private readonly IPopulateScope<WeakTypeReferance> output;
        private readonly Box<IIsPossibly<IFrontendType<IVarifiableType>>> box = new Box<IIsPossibly<IFrontendType<IVarifiableType>>>();

        public MethodDefinitionPopulateScope(
            IPopulateScope<WeakMemberReference> parameterDefinition,
            IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements,
            IPopulateScope<WeakTypeReferance> output
            )
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.output = output ?? throw new ArgumentNullException(nameof(output));

        }

        public IBox<IIsPossibly<IFrontendType<IVarifiableType>>> GetReturnType()
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
                box);
        }
    }

    internal class MethodDefinitionResolveReferance : IPopulateBoxes<WeakMethodDefinition>
    {
        private readonly IPopulateBoxes<WeakMemberReference> parameter;
        private readonly IResolvableScope methodScope;
        private readonly IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] lines;
        private readonly IPopulateBoxes<WeakTypeReferance> output;
        private readonly Box<IIsPossibly<IFrontendType<IVarifiableType>>> box;

        public MethodDefinitionResolveReferance(
            IPopulateBoxes<WeakMemberReference> parameter, 
            IResolvableScope methodScope, 
            IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] resolveReferance2,
            IPopulateBoxes<WeakTypeReferance> output,
            Box<IIsPossibly<IFrontendType<IVarifiableType>>> box)
        {
            this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
            lines = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IIsPossibly<WeakMethodDefinition> Run(IResolveReferenceContext context)
        {
            return box.Fill(
                Possibly.Is(
                    new WeakMethodDefinition(
                        output.Run(context),
                        parameter.Run(context).IfIs(x=> x.MemberDefinition), 
                        lines.Select(x => x.Run(context)).ToArray(),
                        methodScope,
                        new IIsPossibly<IFrontendCodeElement<ICodeElement>>[0])));
        }
    }
    
}