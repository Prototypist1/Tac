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
            WeakMemberDefinition contextDefinition,
            WeakMemberDefinition parameterDefinition,
            IOrType<IFrontendType<IVerifiableType>, IError> outputType,
            IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> metohdBody,
            IBox<WeakScope> scope, 
            IEnumerable<IFrontendCodeElement> staticInitializers)
        {
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBody = metohdBody ?? throw new ArgumentNullException(nameof(metohdBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialzers = staticInitializers ?? throw new ArgumentNullException(nameof(staticInitializers));

            type = OrType.Make<IFrontendType<IVerifiableType>, IError>(SyntaxModel.Elements.AtomicTypes.MethodType.ImplementationType(
                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(ParameterDefinition.Type),
                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OutputType),
                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(ContextDefinition.Type)));
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> OutputType { get; }
        public WeakMemberDefinition ContextDefinition { get; }
        public WeakMemberDefinition ParameterDefinition { get; }
        public IBox<WeakScope> Scope { get; }
        public IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> MethodBody { get; }
        public IEnumerable<IFrontendCodeElement> StaticInitialzers { get; }

        private readonly OrType<IFrontendType<IVerifiableType>, IError> type;

        public IBuildIntention<IImplementationDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ImplementationDefinition.Create();

 

            return new BuildIntention<IImplementationDefinition>(toBuild, () =>
            {
                var contextMember = ContextDefinition.Convert(context);

                maker.Build(
                    OutputType.Is1OrThrow().Convert(context),
                    contextMember,
                    ParameterDefinition.Convert(context),
                    Scope.GetValue().Convert(context),
                    MethodBody.GetValue().Select(x => x.Is1OrThrow().GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitialzers.Select(x => x.ConvertElementOrThrow(context)).ToArray(),
                    Model.Instantiated.Scope.CreateAndBuild(new IsStatic[] {
                        new IsStatic(contextMember,false)
                    }));
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns()
        {
            // TODO
            // are there really frontend types that arnt convertable?!
            return type;
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in OutputType.SwitchReturns(x=>x.Validate(),x=> new[] { x}))
            {
                yield return error;
            }
            foreach (var error in ContextDefinition.Validate())
            {
                yield return error;
            }
            foreach (var error in ParameterDefinition.Validate())
            {
                yield return error;
            }
            foreach (var error in Scope.GetValue().Validate())
            {
                yield return error;
            }
            foreach (var line in MethodBody.GetValue().OfType<IIsDefinately<IBox<IFrontendCodeElement>>>().Select(x=>x.Value))
            {
                foreach (var error in line.GetValue().Validate())
                {
                    yield return error;
                }
            }
            foreach (var error in MethodBody.GetValue().OfType<IIsDefinately<IError>>().Select(x => x.Value))
            {
                yield return error;
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
            // this is not great
            // but the typing here is hard to get right 
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> context= null, input = null, output = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            var match = tokenMatching
                .Has(new KeyWordMaker("implementation"), out var _)
                .HasSquare(x => x
                    .HasLine(y=>y
                        .Has(new TypeMaker(), out context)
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .Has(new TypeMaker(), out input)
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .Has(new TypeMaker(), out output)
                        .Has(new DoneMaker()))
                    .Has(new DoneMaker()))
                .OptionalHas(new NameMaker(), out var contextName)
                .OptionalHas(new NameMaker(), out var parameterName)
                .Has(new BodyMaker(), out var body);
            if (match is IMatchedTokenMatching matched)
            {
                var elements = tokenMatching.Context.ParseBlock(body!);


                return TokenMatching<ISetUp<IBox<WeakImplementationDefinition>, IValue>>.MakeMatch(
                    tokenMatching,
                    new PopulateScopeImplementationDefinition(
                        context!,
                        input!,
                        elements,
                        output!,
                        contextName?.Item ?? "context",
                        parameterName?.Item ?? "input"),
                    matched.EndIndex);
            }


            return TokenMatching<ISetUp<IBox<WeakImplementationDefinition>, IValue>>.MakeNotMatch(match.Context);
        }
        
    }

    internal class PopulateScopeImplementationDefinition : ISetUp<IBox<WeakImplementationDefinition>, Tpn.IValue>
    {
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> contextDefinition;
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition;
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output;
        private readonly string contextName;
        private readonly string parameterName;

        public PopulateScopeImplementationDefinition(
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> contextDefinition,
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition,
            IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements,
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output,
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

        public ISetUpResult<IBox<WeakImplementationDefinition>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            scope = scope.EnterInitizaionScopeIfNessisary();

            if (!(scope is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            // TODO this is so painful, I think I need to look in to implementations having special treatment...
            // maybe they need to be a generic on the tpn
            // altho to the tpn they really are not special
            // but here they might maybe convert to an implementation not a method that returns a method
            // idk! 🤷‍😭

            var realizeContext = contextDefinition.Run(scope, context.CreateChildContext(this));
            var realizedInput = parameterDefinition.Run(scope, context.CreateChildContext(this));
            var realizedOutput = output.Run(scope, context.CreateChildContext(this));
            var outputTypeRef = context.TypeProblem.CreateTypeReference(scope, new GenericNameKey(new NameKey("method"), new[] {
                    realizedInput.SetUpSideNode.TransformInner(x=>x.Key()),
                    realizedOutput.SetUpSideNode.TransformInner(x=>x.Key()),
                }), new WeakTypeReferenceConverter());

            var innerBox = new Box<Tpn.TypeProblem2.Method>();
            var linesBox = new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>();
            var outer = context.TypeProblem.CreateMethod(
                scope, 
                realizeContext.SetUpSideNode, 
                OrType.Make<TypeProblem2.TypeReference, IError>(outputTypeRef), 
                contextName, 
                new WeakImplementationDefinitionConverter(
                    new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>(Array.Empty<IOrType<IBox<IFrontendCodeElement>,IError>>()), 
                    innerBox), 
                new WeakMemberDefinitionConverter(
                    Access.ReadWrite, 
                    new NameKey(parameterName)));

            var inner = context.TypeProblem.CreateMethod(
                outer, 
                realizedInput.SetUpSideNode, 
                realizedOutput.SetUpSideNode, 
                parameterName, 
                new WeakMethodDefinitionConverter(
                    linesBox), 
                new WeakMemberDefinitionConverter(
                    Access.ReadWrite, 
                    new NameKey(parameterName)));

            innerBox.Fill(inner);
            var nextElements = elements.Select(y => y.TransformInner(x => x.Run(inner, context.CreateChildContext(this)).Resolve)).ToArray();

            var innerValue = context.TypeProblem.CreateValue(outer,
                 new GenericNameKey(new NameKey("method"), new[] {
                         realizedInput.SetUpSideNode.TransformInner(x=>x.Key()),
                         realizedOutput.SetUpSideNode.TransformInner(x=>x.Key()),
                 }), new PlaceholderValueConverter());

            innerValue.AssignTo(outer.Returns());

            var value = context.TypeProblem.CreateValue(runtimeScope, new GenericNameKey(new NameKey("method"), new[] {
                    realizeContext.SetUpSideNode.TransformInner(x=>x.Key()),
                    OrType.Make<IKey,IError>(new GenericNameKey(new NameKey("method"), new[] {
                         realizedInput.SetUpSideNode.TransformInner(x=>x.Key()),
                         realizedOutput.SetUpSideNode.TransformInner(x=>x.Key()),
                    })),
                }), new PlaceholderValueConverter());

            return new SetUpResult<IBox<WeakImplementationDefinition>, Tpn.IValue>(new ImplementationDefinitionResolveReferance(
                outer, nextElements, linesBox), OrType.Make<Tpn.IValue, IError>(value));
        }
    }

    internal class ImplementationDefinitionResolveReferance : IResolve<IBox<WeakImplementationDefinition>>
    {
        private readonly Tpn.TypeProblem2.Method outer;
        private readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextElements;
        private readonly Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> linesBox;

        public ImplementationDefinitionResolveReferance(Tpn.TypeProblem2.Method outer, IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextElements, Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> linesBox)
        {
            this.outer = outer ?? throw new ArgumentNullException(nameof(outer));
            this.nextElements = nextElements ?? throw new ArgumentNullException(nameof(nextElements));
            this.linesBox = linesBox ?? throw new ArgumentNullException(nameof(linesBox));
        }

        public IBox<WeakImplementationDefinition> Run(Tpn.TypeSolution context)
        {
            linesBox.Fill(nextElements.Select(x => x.TransformInner(y => y.Run(context))).ToArray());
            var res = outer.Converter.Convert(context, outer);
            if (res.Is2(out var v2))
            {
                return new Box<WeakImplementationDefinition>(v2);
            }
            throw new Exception("wrong!");
        }
    }
}
