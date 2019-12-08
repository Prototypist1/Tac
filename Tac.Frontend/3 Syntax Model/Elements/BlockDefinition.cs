using System;
using System.Collections.Generic;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Frontend._3_Syntax_Model.Operations;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticBlockDefinitionMaker = AddElementMakers(
            () => new BlockDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> BlockDefinitionMaker = StaticBlockDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model
{

    internal class WeakBlockDefinition : WeakAbstractBlockDefinition<IBlockDefinition>
    {
        public WeakBlockDefinition(
            IIsPossibly<IFrontendCodeElement>[] body,
            IBox<WeakScope> scope,
            IEnumerable<IIsPossibly<IFrontendCodeElement>> staticInitailizers) :
            base(scope, body, staticInitailizers)
        { }

        public override IBuildIntention<IBlockDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = BlockDefinition.Create();
            return new BuildIntention<IBlockDefinition>(toBuild, () =>
            {
                maker.Build(
                    Scope.GetValue().Convert(context),
                    Body.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }

        public override IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IFrontendType>(PrimitiveTypes.CreateBlockType());
        }
    }

    internal class BlockDefinitionMaker : IMaker<ISetUp<WeakBlockDefinition, LocalTpn.IScope>>
    {
        public BlockDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<WeakBlockDefinition, LocalTpn.IScope>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
               .Has(new BodyMaker(), out var body);

            if (match is IMatchedTokenMatching
               matched)
            {
                var elements = tokenMatching.Context.ParseBlock(body);

                return TokenMatching<ISetUp<WeakBlockDefinition, LocalTpn.IScope>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context,
                    new BlockDefinitionPopulateScope(elements));
            }

            return TokenMatching<ISetUp<WeakBlockDefinition, LocalTpn.IScope>>.MakeNotMatch(tokenMatching.Context);
        }

        public static ISetUp<WeakBlockDefinition, LocalTpn.IScope> PopulateScope(ISetUp<IConvertableFrontendCodeElement<ICodeElement>, LocalTpn.ITypeProblemNode>[] elements)
        {
            return new BlockDefinitionPopulateScope(elements);
        }
        private class BlockDefinitionPopulateScope : ISetUp<WeakBlockDefinition, LocalTpn.IScope>
        {
            // TODO object??
            // is it worth adding another T?
            // this is the type the backend owns
            private ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] Elements { get; }

            public BlockDefinitionPopulateScope(ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements)
            {
                Elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public ISetUpResult<WeakBlockDefinition, LocalTpn.IScope> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                var myScope = context.TypeProblem.CreateScope(scope, new WeakScopeConverter());
                return new SetUpResult<WeakBlockDefinition, LocalTpn.IScope>(new ResolveReferanceBlockDefinition(myScope,
                    Elements.Select(x => x.Run(myScope, context).Resolve).ToArray()), myScope);
            }
        }

        private class ResolveReferanceBlockDefinition : IResolve<WeakBlockDefinition>
        {
            private readonly Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Scope myScope;
            private readonly IResolve<IFrontendCodeElement>[] resolveReferance;

            public ResolveReferanceBlockDefinition(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Scope myScope, IResolve<IFrontendCodeElement>[] resolveReferance)
            {
                this.myScope = myScope;
                this.resolveReferance = resolveReferance;
            }

            public IIsPossibly<WeakBlockDefinition> Run(LocalTpn.ITypeSolution context)
            {
                return
                        Possibly.Is(
                            new WeakBlockDefinition(
                                resolveReferance.Select(x => x.Run(context)).ToArray(),
                                context.GetScope(myScope),
                                Array.Empty<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>>()));
            }
        }
    }
}