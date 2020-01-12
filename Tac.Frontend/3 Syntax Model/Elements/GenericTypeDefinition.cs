
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.SemanticModel;
using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.Parser;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticGenericTypeDefinitionMaker = AddElementMakers(
            () => new GenericTypeDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> GenericTypeDefinitionMaker = StaticGenericTypeDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}



namespace Tac.SemanticModel
{

    internal interface IWeakGenericTypeDefinition: IFrontendCodeElement, IScoped, IFrontendType, IFrontendGenericType
    {
        IIsPossibly<IKey> Key { get; }
    }

    internal class WeakGenericTypeDefinition : IWeakGenericTypeDefinition
    {
        public WeakGenericTypeDefinition(
            IIsPossibly<NameKey> key,
            IBox<WeakScope> scope,
            IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions)
        {
            this.TypeParameterDefinitions = TypeParameterDefinitions ?? throw new ArgumentNullException(nameof(TypeParameterDefinitions));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
        public IIsPossibly<IKey> Key { get; }
        public IBox<WeakScope> Scope { get; }
    }
    
    internal class GenericTypeDefinitionMaker : IMaker<ISetUp<WeakGenericTypeDefinition, Tpn.IExplicitType>>
    {

        public GenericTypeDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<WeakGenericTypeDefinition, Tpn.IExplicitType>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .Has(new DefineGenericNMaker(), out var genericTypes)
                .Has(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakGenericTypeDefinition, Tpn.IExplicitType>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new GenericTypeDefinitionPopulateScope(
                        new NameKey(typeName.Item),
                        tokenMatching.Context.ParseBlock(body),
                        genericTypes.Select(x =>
                        new GenericTypeParameterPlacholder(new NameKey(x)) as IGenericTypeParameterPlacholder).ToArray()));
            }

            return TokenMatching<ISetUp<WeakGenericTypeDefinition, Tpn.IExplicitType>>.MakeNotMatch(
                    matching.Context);
        }

        public static ISetUp<WeakGenericTypeDefinition, Tpn.IExplicitType> PopulateScope(
                NameKey nameKey,
                IEnumerable<ISetUp<IConvertableFrontendCodeElement<ICodeElement>, Tpn.ITypeProblemNode>> lines,
                IGenericTypeParameterPlacholder[] genericParameters)
        {
            return new GenericTypeDefinitionPopulateScope(
                nameKey,
                lines,
                genericParameters);
        }

        private class GenericTypeDefinitionPopulateScope : ISetUp<WeakGenericTypeDefinition, Tpn.IExplicitType>
        {
            private readonly NameKey nameKey;
            private readonly IEnumerable<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> lines;
            private readonly IGenericTypeParameterPlacholder[] genericParameters;

            public GenericTypeDefinitionPopulateScope(
                NameKey nameKey,
                IEnumerable<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> lines,
                IGenericTypeParameterPlacholder[] genericParameters)
            {
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
                this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
                this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
            }

            public ISetUpResult<WeakGenericTypeDefinition, Tpn.IExplicitType> Run(Tpn.IScope scope, ISetUpContext context)
            {
                // oh geez here is a mountain.
                // I generic types are erased 
                // what on earth does this return?
                var myScope = context.TypeProblem.CreateGenericType(scope, nameKey, genericParameters.Select(x=>new Tpn.TypeAndConverter(x.Key, new WeakTypeDefinitionConverter())).ToArray(),new WeakGenericTypeDefinitionConverter(nameKey, genericParameters));
                var nextLines = lines.Select(x => x.Run(myScope, context).Resolve).ToArray();
                return new SetUpResult<WeakGenericTypeDefinition, Tpn.IExplicitType>(new GenericTypeDefinitionResolveReferance(myScope, nextLines), myScope);
            }
        }

        private class GenericTypeDefinitionResolveReferance : IResolve<WeakGenericTypeDefinition>
        {
            private readonly Tpn.TypeProblem2.Type myScope;
            private readonly IResolve<IFrontendCodeElement>[] nextLines;

            public GenericTypeDefinitionResolveReferance(Tpn.TypeProblem2.Type myScope, IResolve<IFrontendCodeElement>[] nextLines)
            {
                this.myScope = myScope;
                this.nextLines = nextLines;
            }


            public IBox<WeakGenericTypeDefinition> Run(Tpn.ITypeSolution context)
            {
                // uhhh it is werid that I have to do this
                nextLines.Select(x => x.Run(context)).ToArray();
                if (context.GetExplicitType(myScope).GetValue().Is2(out var v2)) {
                    return new Box<WeakGenericTypeDefinition>( v2);
                }

                throw new Exception("well that is not good");
            }
        }
    }
}
