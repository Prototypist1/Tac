using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prototypist.Fluent;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend._3_Syntax_Model.Operations;
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticImplementationDefinitionMaker = AddElementMakers(
            () => new ImplementationDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> ImplementationDefinitionMaker = StaticImplementationDefinitionMaker;
#pragma warning disable IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model
{

    internal class WeakImplementationDefinition: IConvertableFrontendCodeElement<IImplementationDefinition>, IFrontendType
    {

        public WeakImplementationDefinition(
            IBox<IWeakMemberDefinition> contextDefinition,
            IBox<IWeakMemberDefinition> parameterDefinition,
            IBox<IFrontendType> outputType,
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

        // dang! these could also be inline definitions 
        public IIsPossibly<IFrontendType> ContextTypeBox
        {
            get
            {
                return ContextDefinition.IfIs(x=>x.GetValue()).IfIs(x=> x.Type);
            }
        }
        public IIsPossibly<IFrontendType> InputTypeBox
        {
            get
            {
                return ParameterDefinition.IfIs(x => x.GetValue()).IfIs(x => x.Type);
            }
        }
        public IBox<IFrontendType> OutputType { get; }
        public IBox<IWeakMemberDefinition> ContextDefinition { get; }
        public IBox<IWeakMemberDefinition> ParameterDefinition { get; }
        public IBox<WeakScope> Scope { get; }
        public IReadOnlyList<IBox<IFrontendCodeElement>> MethodBody { get; }
        public IEnumerable<IFrontendCodeElement> StaticInitialzers { get; }

        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is(this);
        }

        public IBuildIntention<IImplementationDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ImplementationDefinition.Create();
            return new BuildIntention<IImplementationDefinition>(toBuild, () =>
            {
                maker.Build(
                    TransformerExtensions.Convert<IVerifiableType>(OutputType.GetOrThrow(),context),
                    ContextDefinition.IfIs(x=>x.GetValue()).GetOrThrow().Convert(context),
                    ParameterDefinition.IfIs(x => x.GetValue()).GetOrThrow().Convert(context),
                    Scope.GetValue().Convert(context),
                    MethodBody.Select(x => x.GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitialzers.Select(x => x.ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class ImplementationDefinitionMaker : IMaker<ISetUp<WeakImplementationDefinition, LocalTpn.IValue>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<WeakImplementationDefinition, LocalTpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> context= null, input = null, output = null;

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
                var elements = tokenMatching.Context.ParseBlock(body);
                

                return TokenMatching<ISetUp<WeakImplementationDefinition, LocalTpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new PopulateScopeImplementationDefinition(
                        context,
                        input, 
                        elements,
                        output,
                        contextName?.Item ?? "context",
                        parameterName?.Item ?? "input"));
            }


            return TokenMatching<ISetUp<WeakImplementationDefinition, LocalTpn.IValue>>.MakeNotMatch(match.Context);
        }
        
        public static ISetUp<WeakImplementationDefinition, LocalTpn.IValue> PopulateScope(
                                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> contextDefinition,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> parameterDefinition,
                ISetUp<IConvertableFrontendCodeElement<ICodeElement>, LocalTpn.ITypeProblemNode>[] elements,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> output,
                string contextName,
                string parameterName)
        {
            return new PopulateScopeImplementationDefinition(
                                 contextDefinition,
                 parameterDefinition,
                 elements,
                 output,
                 contextName,
                 parameterName);
        }
        
        private class PopulateScopeImplementationDefinition : ISetUp<WeakImplementationDefinition, LocalTpn.IValue>
        {
            private readonly ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> contextDefinition;
            private readonly ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> parameterDefinition;
            private readonly ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements;
            private readonly ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> output;
            private readonly string contextName;
            private readonly string parameterName;

            public PopulateScopeImplementationDefinition(
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> contextDefinition,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> parameterDefinition,
                ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements,
                ISetUp<IFrontendType, LocalTpn.TypeProblem2.TypeReference> output,
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

            public ISetUpResult<WeakImplementationDefinition,LocalTpn.IValue> Run(LocalTpn.IScope scope, ISetUpContext context)
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
                    realizedInput.SetUpSideNode.Key(),
                    realizedOutput.SetUpSideNode.Key(),
                }),new WeakTypeReferenceConverter());

                var innerBox = new Box<LocalTpn.TypeProblem2.Method>();
                var linesBox = new Box<IResolve<IFrontendCodeElement>[]>();
                var outer = context.TypeProblem.CreateMethod(scope, realizeContext.SetUpSideNode, outputTypeRef, contextName, new WeakImplementationDefinitionConverter(new Box<IResolve<IFrontendCodeElement>[]>(Array.Empty<IResolve<IFrontendCodeElement>>()), innerBox), new WeakMemberDefinitionConverter(false, new NameKey(parameterName)), new WeakMemberDefinitionConverter(false, new NameKey("result")));

                var inner = context.TypeProblem.CreateMethod(outer, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName, new WeakMethodDefinitionConverter(linesBox,false), new WeakMemberDefinitionConverter(false, new NameKey(parameterName)), new WeakMemberDefinitionConverter(false, new NameKey("result")));
                innerBox.Fill(inner);
                linesBox.Fill(elements.Select(y => y.Run(inner, context).Resolve).ToArray());

               var innerValue = context.TypeProblem.CreateValue(outer, 
                    new GenericNameKey(new NameKey("method"), new[] {
                         realizedInput.SetUpSideNode.Key(),
                         realizedOutput.SetUpSideNode.Key(),
                    }), new PlaceholderValueConverter());

                innerValue.AssignTo(outer.Returns());

                var value = context.TypeProblem.CreateValue(scope, new GenericNameKey(new NameKey("method"), new[] {
                    realizeContext.SetUpSideNode.Key(),
                    new GenericNameKey(new NameKey("method"), new[] {
                         realizedInput.SetUpSideNode.Key(),
                         realizedOutput.SetUpSideNode.Key(),
                    }),
                }),new PlaceholderValueConverter());

                return new SetUpResult<WeakImplementationDefinition, LocalTpn.IValue>(new ImplementationDefinitionResolveReferance(
                    outer), value);
            }
        }


        private class ImplementationDefinitionResolveReferance : IResolve<WeakImplementationDefinition>
        {
            private Tpn<WeakBlockDefinition, OrType<WeakTypeDefinition, WeakGenericTypeDefinition>, WeakObjectDefinition, WeakTypeOrOperation, OrType<WeakMethodDefinition, WeakImplementationDefinition>, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Method outer;

            public ImplementationDefinitionResolveReferance(Tpn<WeakBlockDefinition, OrType<WeakTypeDefinition, WeakGenericTypeDefinition>, WeakObjectDefinition, WeakTypeOrOperation, OrType<WeakMethodDefinition, WeakImplementationDefinition>, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Method outer)
            {
                this.outer = outer;
            }

            public IBox<WeakImplementationDefinition> Run(LocalTpn.ITypeSolution context)
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
