//using Prototypist.Toolbox;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Tac.Frontend.New.CrzayNamespace;
//using Tac.Frontend.Parser;
//using Tac.Infastructure;
//using Tac.Model;
//using Tac.Model.Elements;
//using Tac.Parser;
//using Tac.SemanticModel;
//using Tac.SemanticModel.CodeStuff;
//using Tac.SyntaxModel.Elements.AtomicTypes;

//namespace Tac.Frontend._3_Syntax_Model.Elements
//{

//    internal class WeakGenericMethodDefinition { 
    
//    }

//    internal class GenericMethodDefinitionMaker
//    {

//        public ITokenMatching<ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
//        {
//#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
//            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> inputType = null, outputType = null;
//#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

//            var matching = tokenMatching
//                .Has(new KeyWordMaker("method"), out var _)
//                .Has(new DefineGenericNMaker(), out var generics)
//                .HasSquare(x => x
//                    .HasLine(y => y
//                        .Has(new TypeMaker(), out inputType)
//                        .Has(new DoneMaker()))
//                    .HasLine(y => y
//                        .Has(new TypeMaker(), out outputType)
//                        .Has(new DoneMaker()))
//                    .Has(new DoneMaker()))
//                .OptionalHas(new NameMaker(), out var parameterName)
//                .Has(new BodyMaker(), out var body);

//            if (matching
//                 is IMatchedTokenMatching matched)
//            {
//                var elements = matching.Context.ParseBlock(body);

//                return TokenMatching<ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.IValue>>.MakeMatch(
//                    tokenMatching,
//                    new GenericMethodDefinitionPopulateScope(
//                        inputType!,
//                        elements,
//                        outputType!,
//                        parameterName!.Item,
//                        generics.Select(x =>
//                            new GenericTypeParameterPlacholder(OrType.Make<NameKey, ImplicitKey>(new NameKey(x))) as IGenericTypeParameterPlacholder).ToArray()),
//                    matched.EndIndex
//                    );
//            }

//            return TokenMatching<ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.IValue>>.MakeNotMatch(
//                    matching.Context);
//        }
//    }


//    internal class GenericMethodDefinitionPopulateScope : ISetUp<IBox<WeakGenericMethodDefinition>, Tpn.IValue>
//    {
//        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition;
//        private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
//        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output;
//        private readonly string parameterName;
//        private readonly IGenericTypeParameterPlacholder[] genericParameters;

//        public GenericMethodDefinitionPopulateScope(
//            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> parameterDefinition,
//            IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements,
//            ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> output,
//            string parameterName,
//            IGenericTypeParameterPlacholder[] genericParameters
//            )
//        {
//            this.parameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
//            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
//            this.output = output ?? throw new ArgumentNullException(nameof(output));
//            this.parameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
//            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
//        }

//        public ISetUpResult<IBox<WeakGenericMethodDefinition>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
//        {

//            scope = scope.EnterInitizaionScopeIfNessisary();
//            if (!(scope is Tpn.IScope runtimeScope))
//            {
//                throw new NotImplementedException("this should be an IError");
//            }

//            var realizedInput = parameterDefinition.Run(scope, context.CreateChildContext(this));
//            var realizedOutput = output.Run(scope, context.CreateChildContext(this));

//            var box = new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>();
//            var converter = new WeakMethodDefinitionConverter(box);
//            var method = context.TypeProblem.CreateMethod(scope, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName, converter);

//            var nextElements = elements.Select(x => x.TransformInner(y => y.Run(method, context.CreateChildContext(this)).Resolve)).ToArray();

//            var value = context.TypeProblem.CreateValue(runtimeScope, new GenericNameKey(new NameKey("method"), new IOrType<IKey, IError>[] {
//                    realizedInput.SetUpSideNode.TransformInner(x=>x.Key()),
//                    realizedOutput.SetUpSideNode.TransformInner(x=>x.Key()),
//                }), new PlaceholderValueConverter());

//            return new SetUpResult<IBox<WeakMethodDefinition>, Tpn.IValue>(new MethodDefinitionResolveReferance(method, nextElements, box), OrType.Make<Tpn.IValue, IError>(value));
//        }
//    }

//}
