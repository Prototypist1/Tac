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
using Tac.SyntaxModel.Elements.AtomicTypes;
using Prototypist.Toolbox.Object;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticGenericMethodDefinitionMaker = AddElementMakers(
            () => new GenericMethodDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> GenericMethodDefinitionMaker = StaticGenericMethodDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}

namespace Tac.SemanticModel
{

    internal class WeakGenericMethodDefinition :
        WeakAbstractBlockDefinition<IGenericMethodDefinition>, IReturn
    {
        private readonly IOrType<SyntaxModel.Elements.AtomicTypes.GenericMethodType, IError> type;

        public WeakGenericMethodDefinition(
            IOrType<IFrontendType<IVerifiableType>, IError> outputType,
            WeakMemberDefinition parameterDefinition,
            IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body,
            IOrType<WeakScope, IError> scope,
            IReadOnlyList<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers,
            IOrType<IGenericTypeParameterPlacholder, IError>[] TypeParameterDefinitions) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            type = OrType.Make<SyntaxModel.Elements.AtomicTypes.GenericMethodType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.GenericMethodType(
                InputType,
                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OutputType),
                TypeParameterDefinitions));
            this.TypeParameterDefinitions = TypeParameterDefinitions ?? throw new ArgumentNullException(nameof(TypeParameterDefinitions));
        }

        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> InputType => ParameterDefinition.Type;
        public IOrType<IFrontendType<IVerifiableType>, IError> OutputType { get; }
        public WeakMemberDefinition ParameterDefinition { get; }
        public IOrType<IGenericTypeParameterPlacholder, IError>[] TypeParameterDefinitions { get; }

        public override IBuildIntention<IGenericMethodDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = GenericMethodDefinition.Create();
            return new BuildIntention<IGenericMethodDefinition>(toBuild, () =>
            {
                maker.Build(
                    OutputType.Is1OrThrow().Convert(context),
                    ParameterDefinition.Convert(context),
                    Scope.Is1OrThrow().Convert(context),
                    Body.GetValue().Select(x => x.Is1OrThrow().GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray(),
                    TypeParameterDefinitions.Select(x=> x.Is1OrThrow().Convert(context).SafeCastTo(out IGenericTypeParameter _)).ToArray());
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

    internal class GenericMethodDefinitionMaker : IMaker<ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.TypeProblem2.Method>>
    {

        public ITokenMatching<ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.TypeProblem2.Method>> TryMake(IMatchedTokenMatching tokenMatching)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> inputType = null, outputType = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            var matching = tokenMatching
                .Has(new KeyWordMaker("method"), out var _)
                .Has(new DefineGenericNMaker(), out var generics)
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

                return TokenMatching<ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.TypeProblem2.Method>>.MakeMatch(
                    tokenMatching,
                    new GenericMethodDefinitionPopulateScope(
                        inputType!,
                        elements,
                        outputType!,
                        parameterName!.Item,
                        generics.Select(x =>new NameKey(x)).ToArray()),
                    matched.EndIndex
                    );
            }

            return TokenMatching<ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.TypeProblem2.Method>>.MakeNotMatch(
                    matching.Context);
        }
    }


    internal class GenericMethodDefinitionPopulateScope : ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.TypeProblem2.Method>
    {
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition;
        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output;
        private readonly string parameterName;
        private readonly NameKey[] genericParameters;

        public GenericMethodDefinitionPopulateScope(
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition,
            IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements,
            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output,
            string parameterName,
            NameKey[] genericParameters
            )
        {
            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
        }

        public ISetUpResult<IBox<WeakGenericMethodDefinition>, Tpn.TypeProblem2.Method> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            scope = scope.EnterInitizaionScopeIfNessisary();
            if (!(scope is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            var box = new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>();
            var (method, realizedInput, realizedOutput) = context.TypeProblem.CreateGenericMethod(
                scope, 
                x=> parameterDefinition.Run(x, context.CreateChildContext(this)).SetUpSideNode, 
                x => output.Run(x, context.CreateChildContext(this)).SetUpSideNode,
                parameterName,
                new WeakMethodDefinitionConverter(box),
                genericParameters.Select(x => new Tpn.TypeAndConverter(x, new WeakTypeDefinitionConverter())).ToArray());

            var nextElements = elements.Select(x => x.TransformInner(y => y.Run(method, context.CreateChildContext(this)).Resolve)).ToArray();

            // I think for the type I return a method type
            // they already have generic parameters 

            // the question is how does look up work?
            // we aren't looking up something with a fix number of generic parameters...
            // I can't throw a bunch of stuff in the primitive scope
            // it's not just about how many parameters they have but also how those parameters turn into output
            // so we have to end up with a factory behind a cache

            // there is also a problem with how you name our type parameters
            // generic-method [T] [T,T] is the same as generic-method [T1] [T1,T1]
            // stituations like: generic-method [T1] [T1,generic-method [T] [T1,T]] make this makes this more complex
            // type combining is common is tac
            // generic types can do it type [T1] X {T1 t} and type [T] X{T t} are the same
            // types can do it too type X {int a} and type Y {int a}
            // for types, they just end up being assignable to each other
            // I don't think generic types really work... in the example above those would both be registered under NameKey("X") and the second one you try to add would throw

            // if I can count on them to combine later maybe I should just make the type here
            // there will still be look up tho, members


            // {8E138F8D-53AA-4D6A-B337-64CAFED23391}
            //var value = context.TypeProblem.CreateValue(method, new DoubleGenericNameKey(
            //    new NameKey("method"),
            //    genericParameters,
            //    new IOrType<IKey, IError>[] {
            //        realizedInput.TransformInner(x=>x.Key()),
            //        realizedOutput.TransformInner(x=>x.Key()),
            //    })); ;


            //// {C28BDF52-A848-4D0A-824A-7F2943BCFE53}
            //var inputMember = context.TypeProblem.GetInput(value);
            //context.TypeProblem.IsAssignedTo(inputMember, method.Input.GetOrThrow()/*lazy GetOrThrow*/);

            //var returnsMember = context.TypeProblem.GetReturns(value);
            //context.TypeProblem.IsAssignedTo(method.Returns.GetOrThrow()/*lazy GetOrThrow*/, returnsMember);

            //var dict =context.TypeProblem.HasGenerics(value, genericParameters);
            //foreach (var key in genericParameters)
            //{
            //    context.TypeProblem.AssertIs(dict[key], method.Generics[key]/*lazy GetOrThrow*/);
            //}


            //var returnsMember = context.TypeProblem.GetReturns(value);
            //context.TypeProblem.IsAssignedTo(method.Returns.GetOrThrow()/*lazy GetOrThrow*/, returnsMember);

            return new SetUpResult<IBox<WeakGenericMethodDefinition>, Tpn.TypeProblem2.Method>(new GenericMethodDefinitionResolveReferance(method, nextElements, box), OrType.Make<Tpn.TypeProblem2.Method, IError>(method));
        }
    }

    internal class GenericMethodDefinitionResolveReferance : IResolve<IBox<WeakGenericMethodDefinition>>
    {
        private readonly Tpn.TypeProblem2.Method method;
        private readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextElements;
        private readonly Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> box;

        public GenericMethodDefinitionResolveReferance(Tpn.TypeProblem2.Method method, IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextElements, Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> box)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.nextElements = nextElements ?? throw new ArgumentNullException(nameof(nextElements));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IBox<WeakGenericMethodDefinition> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {
            box.Fill(nextElements.Select(x => x.TransformInner(y => y.Run(context, stack.Add(method)))).ToArray());
            var res = method.Converter.Convert(context, method, stack);
            if (res.Is4(out var v4))
            {
                return new Box<WeakGenericMethodDefinition>(v4);
            }
            throw new Exception("wrong!");
        }
    }

}
