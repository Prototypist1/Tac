
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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> StaticMethodDefinitionMaker = AddElementMakers(
            () => new MethodDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> MethodDefinitionMaker = StaticMethodDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model
{

    internal class WeakMethodDefinition :
        WeakAbstractBlockDefinition<IInternalMethodDefinition>,
        IMethodDefinition
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
    
    internal class MethodDefinitionMaker : IMaker<IPopulateScope<WeakMethodDefinition,Tpn.IMethod>>
    {
        public MethodDefinitionMaker()
        {
        }


        public ITokenMatching<IPopulateScope<WeakMethodDefinition, Tpn.IMethod>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            
            {
                IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> inputType = null, outputType = null;
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

                    var parameterDefinition = MemberDefinitionMaker.PopulateScope(
                            new NameKey(parameterName?.Item ?? "input"),
                            false,
                            inputType
                            );

                    var resultDefinition = MemberDefinitionMaker.PopulateScope(
                            new ImplicitKey(),
                            false,
                            outputType
                            );

                    return TokenMatching<IPopulateScope<WeakMethodDefinition,Tpn.IMethod>>.MakeMatch(
                        matched.Tokens,
                        matched.Context,
                        new MethodDefinitionPopulateScope(
                            parameterDefinition,
                            elements,
                            resultDefinition,
                            false)
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

                    var parameterDefinition = MemberDefinitionMaker.PopulateScope(
                            new NameKey("input"),
                            false,
                            TypeReferanceMaker.PopulateScope(new NameKey("empty"))
                            );


                    var resultDefinition = MemberDefinitionMaker.PopulateScope(
                            new ImplicitKey(),
                            false,
                            TypeReferanceMaker.PopulateScope(new NameKey("empty"))
                            );

                    return TokenMatching<IPopulateScope<WeakMethodDefinition,Tpn.IMethod>>.MakeMatch(
                        matched.Tokens,
                        matched.Context,
                        new MethodDefinitionPopulateScope(
                            parameterDefinition,
                            elements,
                            resultDefinition,
                            true)
                        );
                }

                return TokenMatching<IPopulateScope<WeakMethodDefinition,Tpn.IMethod>>.MakeNotMatch(
                        matching.Context);
            }

        }

        public static IPopulateScope<WeakMethodDefinition,Tpn.IMethod> PopulateScope(
                IPopulateScope<WeakMemberReference,Tpn.IMember> parameterDefinition,
                IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>,ITypeProblemNode>[] elements,
                IPopulateScope<WeakMemberReference, Tpn.IMember> output,
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
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] resolveReferance2,
                IPopulateBoxes<IWeakTypeReference> output,
                bool isEntryPoint)
        {
            return new MethodDefinitionResolveReferance(
                methodScope,
                parameter,
                resolveReferance2,
                output,
                isEntryPoint);
        }


        private class MethodDefinitionPopulateScope : IPopulateScope<WeakMethodDefinition,Tpn.IMethod>
        {
            private readonly IPopulateScope<WeakMemberReference,Tpn.IMember> parameterDefinition;
            private readonly IPopulateScope<IFrontendCodeElement,ITypeProblemNode>[] elements;
            private readonly IPopulateScope<WeakMemberReference, Tpn.IMember> output;
            private readonly bool isEntryPoint;

            public MethodDefinitionPopulateScope(
                IPopulateScope<WeakMemberReference,Tpn.IMember> parameterDefinition,
                IPopulateScope<IFrontendCodeElement,ITypeProblemNode>[] elements,
                IPopulateScope<WeakMemberReference, Tpn.IMember> output,
                bool isEntryPoint
                )
            {
                this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.isEntryPoint = isEntryPoint;
            }

            public IResolvelizeScope<WeakMethodDefinition,Tpn.IMethod> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {

                var methodBuilder= context.TypeProblem.CreateMethod(scope);

                var parameterResolve = parameterDefinition.Run(methodBuilder, context);
                methodBuilder.Member(parameterResolve.SetUpSideNode);
                var outputResolve = parameterDefinition.Run(methodBuilder, context);
                methodBuilder.Member(outputResolve.SetUpSideNode);
                var method = methodBuilder.SetInputOutput(parameterResolve.SetUpSideNode, outputResolve.SetUpSideNode);

                return new MethodDefinitionFinalizeScope(
                    method,
                    parameterResolve,
                    elements.Select(x => x.Run(method, context)).ToArray(),
                    outputResolve,
                    isEntryPoint);
            }
        }

        private class MethodDefinitionFinalizeScope : IResolvelizeScope<WeakMethodDefinition, Tpn.IMethod>
        {
            private readonly IResolvelizeScope<WeakMemberReference,Tpn.IMember> parameter;
            private readonly IResolvelizeScope<IFrontendCodeElement,ITypeProblemNode>[] lines;
            private readonly IResolvelizeScope<WeakMemberReference, Tpn.IMember> output;
            private readonly bool isEntryPoint;

            public MethodDefinitionFinalizeScope(
                Tpn.IMethod methodScope,
                IResolvelizeScope<WeakMemberReference,Tpn.IMember> parameter,
                IResolvelizeScope<IFrontendCodeElement,ITypeProblemNode>[] resolveReferance2,
                IResolvelizeScope<WeakMemberReference, Tpn.IMember> output,
                bool isEntryPoint)
            {
                SetUpSideNode = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
                this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
                lines = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.isEntryPoint = isEntryPoint;
            }

            public Tpn.IMethod SetUpSideNode
            {
                get;
            }

            public IPopulateBoxes<WeakMethodDefinition> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                var scope = methodScope.FinalizeScope(parent);

                return new MethodDefinitionResolveReferance(
                    scope,
                    parameter.Run(scope,context),
                    lines.Select(x => x.Run(scope,context)).ToArray(),
                    output.Run(scope,context),
                    isEntryPoint);
            }
        }

        private class MethodDefinitionResolveReferance : IPopulateBoxes<WeakMethodDefinition>
        {
            private readonly IResolvableScope methodScope;
            private readonly IPopulateBoxes<WeakMemberReference> parameter;
            private readonly IPopulateBoxes<IFrontendCodeElement>[] lines;
            private readonly IPopulateBoxes<IWeakTypeReference> output;
            private readonly bool isEntryPoint;

            public MethodDefinitionResolveReferance(
                IResolvableScope methodScope,
                IPopulateBoxes<WeakMemberReference> parameter,
                IPopulateBoxes<IFrontendCodeElement>[] resolveReferance2,
                IPopulateBoxes<IWeakTypeReference> output,
                bool isEntryPoint)
            {
                this.methodScope = methodScope ?? throw new ArgumentNullException(nameof(methodScope));
                this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
                lines = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.isEntryPoint = isEntryPoint;
            }

            public IIsPossibly<WeakMethodDefinition> Run(IResolvableScope _, IResolveReferenceContext context)
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