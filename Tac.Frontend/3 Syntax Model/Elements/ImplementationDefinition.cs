using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prototypist.Toolbox;
using Tac.Frontend;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;
using static Tac.Frontend.New.CrzayNamespace.Tpn;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, ITypeProblemNode>> StaticImplementationDefinitionMaker = AddElementMakers(
            () => new ImplementationDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, ITypeProblemNode>> ImplementationDefinitionMaker = StaticImplementationDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}

namespace Tac.SemanticModel
{

    internal class WeakImplementationDefinition: IConvertableFrontendCodeElement<IImplementationDefinition>, IReturn
    {

        public WeakImplementationDefinition(
            IBox<WeakMemberDefinition> contextDefinition,
            IBox<WeakMemberDefinition> parameterDefinition,
            IOrType< IBox<IFrontendType>,IError> outputType,
            IReadOnlyList<IBox<IFrontendCodeElement>> metohdBody,
            IBox<WeakScope> scope, 
            IEnumerable<IFrontendCodeElement> staticInitializers)
        {
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBody = metohdBody ?? throw new ArgumentNullException(nameof(metohdBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialzers = staticInitializers ?? throw new ArgumentNullException(nameof(staticInitializers));
        }

        public IOrType<IBox<IFrontendType>, IError> OutputType { get; }
        public IBox<WeakMemberDefinition> ContextDefinition { get; }
        public IBox<WeakMemberDefinition> ParameterDefinition { get; }
        public IBox<WeakScope> Scope { get; }
        public IReadOnlyList<IBox<IFrontendCodeElement>> MethodBody { get; }
        public IEnumerable<IFrontendCodeElement> StaticInitialzers { get; }

        public IBuildIntention<IImplementationDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ImplementationDefinition.Create();
            return new BuildIntention<IImplementationDefinition>(toBuild, () =>
            {
                maker.Build(
                    OutputType.Is1OrThrow().GetValue().ConvertTypeOrThrow(context),
                    ContextDefinition.GetValue().Convert(context),
                    ParameterDefinition.GetValue().Convert(context),
                    Scope.GetValue().Convert(context),
                    MethodBody.Select(x => x.GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitialzers.Select(x => x.ConvertElementOrThrow(context)).ToArray());
            });
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in OutputType.SwitchReturns(x=>x.GetValue().Validate(),x=> new[] { x}))
            {
                yield return error;
            }
            foreach (var error in ContextDefinition.GetValue().Validate())
            {
                yield return error;
            }
            foreach (var error in ParameterDefinition.GetValue().Validate())
            {
                yield return error;
            }
            foreach (var error in Scope.GetValue().Validate())
            {
                yield return error;
            }
            foreach (var line in MethodBody)
            {
                foreach (var error in line.GetValue().Validate())
                {
                    yield return error;
                }
            }
            foreach (var line in StaticInitialzers)
            {
                foreach (var error in line.Validate())
                {
                    yield return error;
                }
            }
        }
    }

    internal class ImplementationDefinitionMaker : IMaker<ISetUp<IBox<WeakImplementationDefinition>, Tpn.IValue>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<IBox<WeakImplementationDefinition>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference>? context= null, input = null, output = null;

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
                var elements = tokenMatching.Context.ParseBlock(body!);
                

                return TokenMatching<ISetUp<IBox<WeakImplementationDefinition>, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new PopulateScopeImplementationDefinition(
                        context!,
                        input!, 
                        elements,
                        output!,
                        contextName?.Item ?? "context",
                        parameterName?.Item ?? "input"));
            }


            return TokenMatching<ISetUp<IBox<WeakImplementationDefinition>, Tpn.IValue>>.MakeNotMatch(match.Context);
        }
        
        private class PopulateScopeImplementationDefinition : ISetUp<IBox<WeakImplementationDefinition>, Tpn.IValue>
        {
            private readonly ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> contextDefinition;
            private readonly ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> parameterDefinition;
            private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
            private readonly ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> output;
            private readonly string contextName;
            private readonly string parameterName;

            public PopulateScopeImplementationDefinition(
                ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> contextDefinition,
                ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> parameterDefinition,
                IReadOnlyList<IOrType< ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError>> elements,
                ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> output,
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

            public ISetUpResult<IBox<WeakImplementationDefinition>, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {

                // TODO this is so painful, I think I need to look in to implementations having special treatment...
                // maybe they need to be a generic on the tpn
                // altho to the tpn they really are not special
                // but here they might maybe convert to an implementation not a method that returns a method
                // idk! 🤷‍😭

                var realizeContext = contextDefinition.Run(scope, context);
                var realizedInput = parameterDefinition.Run(scope, context);
                var realizedOutput = output.Run(scope, context);
                var outputTypeRef = context.TypeProblem.CreateTypeReference(scope, new GenericNameKey(new NameKey("method"),new[] {
                    realizedInput.SetUpSideNode.TransformInner(x=>x.Key()),
                    realizedOutput.SetUpSideNode.TransformInner(x=>x.Key()),
                }),new WeakTypeReferenceConverter());

                var innerBox = new Box<Tpn.TypeProblem2.Method>();
                var linesBox = new Box<IOrType< IResolve<IBox<IFrontendCodeElement>>,IError>[]>();
                var outer = context.TypeProblem.CreateMethod(scope, realizeContext.SetUpSideNode, OrType.Make<TypeProblem2.TypeReference, IError>(outputTypeRef), contextName, new WeakImplementationDefinitionConverter(new Box<IResolve<IBox<IFrontendCodeElement>>[]>(Array.Empty<IResolve<IBox<IFrontendCodeElement>>>()), innerBox), new WeakMemberDefinitionConverter(false, new NameKey(parameterName)));

                var inner = context.TypeProblem.CreateMethod(outer, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName, new WeakMethodDefinitionConverter(linesBox,false), new WeakMemberDefinitionConverter(false, new NameKey(parameterName)));
                innerBox.Fill(inner);
                linesBox.Fill(elements.Select(y => y.TransformInner(x=>x.Run(inner, context).Resolve)).ToArray());

               var innerValue = context.TypeProblem.CreateValue(outer, 
                    new GenericNameKey(new NameKey("method"), new[] {
                         realizedInput.SetUpSideNode.TransformInner(x=>x.Key()),
                         realizedOutput.SetUpSideNode.TransformInner(x=>x.Key()),
                    }), new PlaceholderValueConverter());

                innerValue.AssignTo(outer.Returns());

                var value = context.TypeProblem.CreateValue(scope, new GenericNameKey(new NameKey("method"), new[] {
                    realizeContext.SetUpSideNode.TransformInner(x=>x.Key()),
                    OrType.Make<IKey,IError>(new GenericNameKey(new NameKey("method"), new[] {
                         realizedInput.SetUpSideNode.TransformInner(x=>x.Key()),
                         realizedOutput.SetUpSideNode.TransformInner(x=>x.Key()),
                    })),
                }),new PlaceholderValueConverter());

                return new SetUpResult<IBox<WeakImplementationDefinition>, Tpn.IValue>(new ImplementationDefinitionResolveReferance(
                    outer), OrType.Make<Tpn.IValue, IError>(value));
            }
        }

        private class ImplementationDefinitionResolveReferance : IResolve<IBox< WeakImplementationDefinition>>
        {
            private readonly Tpn.TypeProblem2.Method outer;

            public ImplementationDefinitionResolveReferance(Tpn.TypeProblem2.Method outer)
            {
                this.outer = outer;
            }

            public IBox<WeakImplementationDefinition> Run(Tpn.ITypeSolution context)
            {
                var res = context.GetMethod(outer);
                if (res.GetValue().Is2(out var v2))
                {
                    return new Box<WeakImplementationDefinition>(v2);
                }
                throw new Exception("wrong!");
            }
        }
    }
}
