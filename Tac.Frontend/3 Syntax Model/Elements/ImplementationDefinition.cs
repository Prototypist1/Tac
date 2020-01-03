using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prototypist.Toolbox;
using Tac.Frontend;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticImplementationDefinitionMaker = AddElementMakers(
            () => new ImplementationDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> ImplementationDefinitionMaker = StaticImplementationDefinitionMaker;
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
        public IBox<IFrontendType> ContextTypeBox
        {
            get
            {
                return ContextDefinition;
            }
        }
        public IBox<IFrontendType> InputTypeBox
        {
            get
            {
                return ParameterDefinition;
            }
        }
        public IBox<IFrontendType> OutputType { get; }
        public IBox<IWeakMemberDefinition> ContextDefinition { get; }
        public IBox<IWeakMemberDefinition> ParameterDefinition { get; }
        public IBox<WeakScope> Scope { get; }
        public IReadOnlyList<IBox<IFrontendCodeElement>> MethodBody { get; }
        public IEnumerable<IFrontendCodeElement> StaticInitialzers { get; }

        public IBuildIntention<IImplementationDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ImplementationDefinition.Create();
            return new BuildIntention<IImplementationDefinition>(toBuild, () =>
            {
                maker.Build(
                    OutputType.GetValue().ConvertTypeOrThrow(context),
                    ContextDefinition.GetValue().Convert(context),
                    ParameterDefinition.GetValue().Convert(context),
                    Scope.GetValue().Convert(context),
                    MethodBody.Select(x => x.GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitialzers.Select(x => x.ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class ImplementationDefinitionMaker : IMaker<ISetUp<WeakImplementationDefinition, Tpn.IValue>>
    {
        public ImplementationDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<WeakImplementationDefinition, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> context= null, input = null, output = null;

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
                

                return TokenMatching<ISetUp<WeakImplementationDefinition, Tpn.IValue>>.MakeMatch(
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


            return TokenMatching<ISetUp<WeakImplementationDefinition, Tpn.IValue>>.MakeNotMatch(match.Context);
        }
        
        public static ISetUp<WeakImplementationDefinition, Tpn.IValue> PopulateScope(
                                ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> contextDefinition,
                ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> parameterDefinition,
                ISetUp<IConvertableFrontendCodeElement<ICodeElement>, Tpn.ITypeProblemNode>[] elements,
                ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> output,
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
        
        private class PopulateScopeImplementationDefinition : ISetUp<WeakImplementationDefinition, Tpn.IValue>
        {
            private readonly ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> contextDefinition;
            private readonly ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> parameterDefinition;
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;
            private readonly ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> output;
            private readonly string contextName;
            private readonly string parameterName;

            public PopulateScopeImplementationDefinition(
                ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> contextDefinition,
                ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> parameterDefinition,
                ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements,
                ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> output,
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

            public ISetUpResult<WeakImplementationDefinition,Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
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

                var innerBox = new Box<Tpn.TypeProblem2.Method>();
                var linesBox = new Box<IResolve<IFrontendCodeElement>[]>();
                var outer = context.TypeProblem.CreateMethod(scope, realizeContext.SetUpSideNode, outputTypeRef, contextName, new WeakImplementationDefinitionConverter(new Box<IResolve<IFrontendCodeElement>[]>(Array.Empty<IResolve<IFrontendCodeElement>>()), innerBox), new WeakMemberDefinitionConverter(false, new NameKey(parameterName)));

                var inner = context.TypeProblem.CreateMethod(outer, realizedInput.SetUpSideNode, realizedOutput.SetUpSideNode, parameterName, new WeakMethodDefinitionConverter(linesBox,false), new WeakMemberDefinitionConverter(false, new NameKey(parameterName)));
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

                return new SetUpResult<WeakImplementationDefinition, Tpn.IValue>(new ImplementationDefinitionResolveReferance(
                    outer), value);
            }
        }


        private class ImplementationDefinitionResolveReferance : IResolve<WeakImplementationDefinition>
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
