using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticEntryPointDefinitionMaker = AddElementMakers(
            () => new EntryPointDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> EntryPointDefinitionMaker = StaticEntryPointDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel
{

    internal class WeakEntryPointDefinition :
        WeakAbstractBlockDefinition<IEntryPointDefinition>
    {
        public WeakEntryPointDefinition(
            IOrType<IFrontendType<IVerifiableType>, IError> outputType,
            WeakMemberDefinition parameterDefinition,
            IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>,IError>>> body,
            IOrType<WeakScope, IError> scope,
            IReadOnlyList<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        //public IOrType<IFrontendType<IVerifiableType>, IError> InputType => ParameterDefinition.Type;
        public IOrType<IFrontendType<IVerifiableType>, IError> OutputType { get; }
        public WeakMemberDefinition ParameterDefinition { get; }

        public override IBuildIntention<IEntryPointDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = EntryPointDefinition.Create();
            return new BuildIntention<IEntryPointDefinition>(toBuild, () =>
            {
                maker.Build(
                    OutputType.Is1OrThrow().Convert(context),
                    ParameterDefinition.Convert(context),
                    Scope.Is1OrThrow().Convert(context),
                    Body.GetValue().Select(x => x.Is1OrThrow().GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class EntryPointDefinitionMaker : IMaker<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>>
    {
        public EntryPointDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>> TryMake(IMatchedTokenMatching tokenMatching)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> inputType = null, outputType = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            var matching = tokenMatching
                .Has(new KeyWordMaker("entry-point"), out var _)
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

                return TokenMatching<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>>.MakeMatch(
                    tokenMatching,
                    new EntryPointDefinitionPopulateScope(
                         inputType!,
                        elements,
                        outputType!,
                        parameterName!.Item),
                    matched.EndIndex
                    );
            }

            return TokenMatching<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>>.MakeNotMatch(
                    matching.Context);
        }
    }


    internal class EntryPointDefinitionPopulateScope : ISetUp<IBox<WeakEntryPointDefinition>, Tpn.TypeProblem2.Method>
    {

        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition;
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output;
        private readonly string parameterName;

        public EntryPointDefinitionPopulateScope(
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

        public ISetUpResult<IBox<WeakEntryPointDefinition>, Tpn.TypeProblem2.Method> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            //var box = new Box<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[]>();

            //var inputBox = new Box<IResolve<IBox<WeakMemberReference>>>();
            //var outputBox = new Box<IResolve<IBox<IFrontendType<IVerifiableType>>>>();

            //var innerScope = context.TypeProblem.CreateScope(scope, new WeakEntryPointConverter(box, inputBox, outputBox));
            //context.TypeProblem.HasEntryPoint(scope, innerScope);

            //inputBox.Fill(parameterDefinition.Run(innerScope, context.CreateChildContext(this)).Resolve);
            //outputBox.Fill(output.Run(innerScope, context.CreateChildContext(this)).Resolve);

            //if (!(scope is Tpn.IScope runtimeScope))
            //{
            //    throw new NotImplementedException("this should be an IError");
            //}


            var box = new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>();
            var converter = new WeakEntryPointConverter(box);
            var (method, realizedInput, realizedOutput) = context.TypeProblem.CreateMethod(
                scope,
                x=> parameterDefinition.Run(x, context.CreateChildContext(this)).SetUpSideNode, 
                x => output.Run(x, context.CreateChildContext(this)).SetUpSideNode, 
                parameterName, 
                converter);

            var converted = elements.Select(x => x.TransformInner(y => y.Run(method, context.CreateChildContext(this)).Resolve)).ToArray();

            return new SetUpResult<IBox<WeakEntryPointDefinition>, Tpn.TypeProblem2.Method > (new EntryPointDefinitionResolveReferance(method, box, converted), OrType.Make<Tpn.TypeProblem2.Method, IError>(method));
        }
    }

    internal class EntryPointDefinitionResolveReferance : IResolve<IBox<WeakEntryPointDefinition>>
    {
        private readonly Tpn.TypeProblem2.Method scope;
        private readonly Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> box;
        private readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] converted;

        public EntryPointDefinitionResolveReferance(Tpn.TypeProblem2.Method scope, Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> box, IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] converted)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.converted = converted ?? throw new ArgumentNullException(nameof(converted));
        }

        public IBox<WeakEntryPointDefinition> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {
            var finalForm=  converted.Select(x => x.TransformInner(y => y.Run(context, stack.Add(scope)))).ToArray();
            box.Fill(finalForm);
            var res = scope.Converter.Convert(context, scope);
            if (res.Is3(out var v3))
            {
                return new Box<WeakEntryPointDefinition>(v3);
            }
            throw new Exception("wrong!");
        }
    }
}


