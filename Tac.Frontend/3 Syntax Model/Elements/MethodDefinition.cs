using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox< IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticMethodDefinitionMaker = AddElementMakers(
            () => new MethodDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> MethodDefinitionMaker = StaticMethodDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}


namespace Tac.SemanticModel
{

    internal class WeakMethodDefinition :
        WeakAbstractBlockDefinition<IInternalMethodDefinition>, IReturn
    {
        private readonly IOrType<SyntaxModel.Elements.AtomicTypes.MethodType, IError> type;

        public WeakMethodDefinition(
            IOrType<IFrontendType<IVerifiableType>, IError> outputType,
            WeakMemberDefinition parameterDefinition,
            IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body,
            IOrType<WeakScope, IError> scope,
            IReadOnlyList<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            type = OrType.Make<SyntaxModel.Elements.AtomicTypes.MethodType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.MethodType(
                InputType,
                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OutputType)
                ));
        }

        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> InputType => ParameterDefinition.Type;
        public IOrType<IFrontendType<IVerifiableType>, IError> OutputType { get; }
        public WeakMemberDefinition ParameterDefinition { get; }


        public override IBuildIntention<IInternalMethodDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MethodDefinition.Create();
            return new BuildIntention<IInternalMethodDefinition>(toBuild, () =>
            {
                maker.Build(
                    OutputType.Is1OrThrow().Convert(context),
                    ParameterDefinition.Convert(context),
                    Scope.Is1OrThrow().Convert(context),
                    Body.GetValue().Select(x => x.Is1OrThrow().GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns()
        {
            return type;
            // TODO
            // are there really frontend types that arn't convertable?
            //return OrType.Make<IFrontendType<IVerifiableType>, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.MethodType(
            //    InputType,
            //    OutputType
            //    ));
        }
    }

    internal class MethodDefinitionMaker : IMaker<ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>>
    {
        public MethodDefinitionMaker()
        {
        }


        public ITokenMatching<ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> inputType = null, outputType = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            var matching = tokenMatching
                .Has(new KeyWordMaker("method"), out var _)
                .HasSquare(x => x
                    .HasLine(y => y
                        .Has(new TypeMaker(), out inputType)
                        .Has(new DoneMaker()))
                    .HasLine(y => y
                        .Has(new TypeMaker(), out outputType)
                        .Has(new DoneMaker()))
                    .Has(new DoneMaker()))
                .OptionalHas(new NameMaker(), out var parameterName)
                .Has(new BodyMaker(), out var body);

            if (matching
                 is IMatchedTokenMatching matched)
            {
                var elements = matching.Context.ParseBlock(body);

                return TokenMatching<ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>>.MakeMatch(
                    tokenMatching,
                    new MethodDefinitionPopulateScope(
                        inputType!,
                        elements,
                        outputType!,
                        parameterName!.Item),
                    matched.EndIndex
                    );
            }

            return TokenMatching<ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


    }


    internal class MethodDefinitionPopulateScope : ISetUp<IBox<WeakMethodDefinition>, Tpn.IValue>
    {
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition;
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output;
        private readonly string parameterName;

        public MethodDefinitionPopulateScope(
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition,
            IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements,
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output,
            string parameterName
            )
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }

        public ISetUpResult<IBox<WeakMethodDefinition>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            scope = scope.EnterInitizaionScopeIfNessisary();
            if (!(scope is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }


            var box = new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>();
            var converter = new WeakMethodDefinitionConverter(box);
            var (method, realizedInput, realizedOutput) = context.TypeProblem.CreateMethod(scope, 
                x=> parameterDefinition.Run(x, context.CreateChildContext(this)).SetUpSideNode, 
                x => output.Run(x, context.CreateChildContext(this)).SetUpSideNode, 
                parameterName, 
                converter);

            var nextElements = elements.Select(x => x.TransformInner(y => y.Run(method, context.CreateChildContext(this)).Resolve)).ToArray();

            var value = context.TypeProblem.CreateTransientMember(runtimeScope, new GenericNameKey(new NameKey("method"), new IOrType<IKey, IError>[] {
                    realizedInput.TransformInner(x=>x.Key()),
                    realizedOutput.TransformInner(x=>x.Key()),
                }), "result-of-" + method.DebugName);

            // when I allow inferred in methods 
            // there is going to be a problem with the type problem not following past the points of the method
            // for example
            // 
            // x > method [infer; infer] input { input return; } =: chicken c
            //
            // chicken need to flow through the method to x, 
            // maybe something like:
            //
            //var inputMember = context.TypeProblem.GetInput(value);
            //context.TypeProblem.IsAssignedTo(inputMember, method.Input.GetOrThrow()/*lazy GetOrThrow*/);
            //
            //var returnsMember = context.TypeProblem.GetInput(value);
            //context.TypeProblem.IsAssignedTo( method.Returns.GetOrThrow()/*lazy GetOrThrow*/, returnsMember);

            return new SetUpResult<IBox<WeakMethodDefinition>, Tpn.IValue>(new MethodDefinitionResolveReferance(method, nextElements,box), OrType.Make<Tpn.IValue, IError>(value));
        }
    }

    internal class MethodDefinitionResolveReferance : IResolve<IBox<WeakMethodDefinition>>
    {
        private readonly Tpn.TypeProblem2.Method method;
        private readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextElements;
        private readonly Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> box;

        public MethodDefinitionResolveReferance(Tpn.TypeProblem2.Method method, IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextElements, Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> box)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.nextElements = nextElements ?? throw new ArgumentNullException(nameof(nextElements));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IBox<WeakMethodDefinition> Run(Tpn.TypeSolution context)
        {
            box.Fill(nextElements.Select(x => x.TransformInner(y => y.Run(context))).ToArray());
            var res = method.Converter.Convert(context, method);
            if (res.Is1(out var v1))
            {
                return new Box<WeakMethodDefinition>(v1);
            }
            throw new Exception("wrong!");
        }
    }
}