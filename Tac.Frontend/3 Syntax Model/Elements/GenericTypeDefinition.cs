
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
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.Parser;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticGenericTypeDefinitionMaker = AddElementMakers(
            () => new GenericTypeDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> GenericTypeDefinitionMaker = StaticGenericTypeDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
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
    
    internal class GenericTypeDefinitionMaker : IMaker<ISetUp<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType>>
    {

        public GenericTypeDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .Has(new DefineGenericNMaker())
                .Has(new NameMaker())
                .Has(new BodyMaker())
                .ConvertIfMatched((generics, name, lines) =>
                    new GenericTypeDefinitionPopulateScope(
                        new NameKey(name.Item),
                        tokenMatching.Context.ParseBlock(lines),
                        generics.Select(x => 
                            new GenericTypeParameterPlacholder(new NameKey(x)) as IGenericTypeParameterPlacholder).ToArray()));
        }

        public static ISetUp<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType> PopulateScope(
                NameKey nameKey,
                IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> lines,
                IGenericTypeParameterPlacholder[] genericParameters)
        {
            return new GenericTypeDefinitionPopulateScope(
                nameKey,
                lines,
                genericParameters);
        }

        private class GenericTypeDefinitionPopulateScope : ISetUp<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType>
        {
            private readonly NameKey nameKey;
            private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> lines;
            private readonly IGenericTypeParameterPlacholder[] genericParameters;

            public GenericTypeDefinitionPopulateScope(
                NameKey nameKey,
                IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError>> lines,
                IGenericTypeParameterPlacholder[] genericParameters)
            {
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
                this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
                this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
            }

            public ISetUpResult<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType> Run(Tpn.IScope scope, ISetUpContext context)
            {
                // oh geez here is a mountain.
                // I generic types are erased 
                // what on earth does this return?
                var myScope = context.TypeProblem.CreateGenericType(scope, nameKey, genericParameters.Select(x=>new Tpn.TypeAndConverter(x.Key, new WeakTypeDefinitionConverter())).ToArray(),new WeakGenericTypeDefinitionConverter(nameKey, genericParameters));
                var nextLines = lines.Select(x => x.TransformInner(y=>y.Run(myScope, context).Resolve)).ToArray();
                return new SetUpResult<IBox<WeakGenericTypeDefinition>, Tpn.IExplicitType>(new GenericTypeDefinitionResolveReferance(myScope, nextLines), OrType.Make<Tpn.IExplicitType, IError>(myScope));
            }
        }

        private class GenericTypeDefinitionResolveReferance : IResolve<IBox<WeakGenericTypeDefinition>>
        {
            private readonly Tpn.TypeProblem2.Type myScope;
            private readonly IOrType< IResolve<IBox<IFrontendCodeElement>>,IError>[] nextLines;

            public GenericTypeDefinitionResolveReferance(Tpn.TypeProblem2.Type myScope, IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextLines)
            {
                this.myScope = myScope;
                this.nextLines = nextLines;
            }


            public IBox<WeakGenericTypeDefinition> Run(Tpn.ITypeSolution context)
            {
                // uhhh it is werid that I have to do this
                nextLines.Select(x => x.TransformInner(y=>y.Run(context))).ToArray();
                if (context.GetExplicitType(myScope).GetValue().Is2(out var v2)) {
                    return new Box<WeakGenericTypeDefinition>( v2);
                }

                throw new Exception("well that is not good");
            }
        }
    }
}
