using System;
using System.Collections.Generic;
using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> StaticBlockDefinitionMaker = AddElementMakers(
            () => new BlockDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> BlockDefinitionMaker = StaticBlockDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model
{

    internal class WeakBlockDefinition : WeakAbstractBlockDefinition<IBlockDefinition>
    {
        public WeakBlockDefinition(
            IIsPossibly<IFrontendCodeElement>[] body,
            IResolvableScope scope,
            IEnumerable<IIsPossibly<IFrontendCodeElement>> staticInitailizers) : 
            base(scope, body, staticInitailizers) { }

        public override IBuildIntention<IBlockDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = BlockDefinition.Create();
            return new BuildIntention<IBlockDefinition>(toBuild, () =>
            {
                maker.Build(
                    Scope.Convert(context),
                    Body.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }

        public override IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IFrontendType>(PrimitiveTypes.CreateBlockType());
        }
    }

    internal class BlockDefinitionMaker : IMaker<IPopulateScope<WeakBlockDefinition, ISetUpScope>>
    {
        public BlockDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakBlockDefinition, ISetUpScope>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
               .Has(new BodyMaker(), out var body);

            if (match is IMatchedTokenMatching
               matched)
            {
                var elements = tokenMatching.Context.ParseBlock(body);

                return TokenMatching<IPopulateScope<WeakBlockDefinition, ISetUpScope>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, 
                    new BlockDefinitionPopulateScope(elements));
            }

            return TokenMatching<IPopulateScope<WeakBlockDefinition, ISetUpScope>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakBlockDefinition, ISetUpScope> PopulateScope(IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>, ISetUpSideNode>[] elements)
        {
            return new BlockDefinitionPopulateScope(elements);
        }
        public static IPopulateBoxes<WeakBlockDefinition> PopulateBoxes(IResolvableScope scope,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] resolveReferance)
        {
            return new ResolveReferanceBlockDefinition(scope, resolveReferance);
        }

        private class BlockDefinitionPopulateScope : IPopulateScope<WeakBlockDefinition, ISetUpScope>
        {
            // TODO object??
            // is it worth adding another T?
            // this is the type the backend owns
            private IPopulateScope<IFrontendCodeElement,ISetUpSideNode>[] Elements { get; }

            public BlockDefinitionPopulateScope(IPopulateScope<IFrontendCodeElement, ISetUpSideNode>[] elements)
            {
                Elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public IResolvelizeScope<WeakBlockDefinition,ISetUpScope> Run(IDefineMembers scope, IPopulateScopeContext context)
            {
                var myScope = context.TypeProblem.CreateScope(scope);
                return new FinalizeScopeBlockDefinition(
                    myScope,
                    Elements.Select(x => x.Run(myScope, context)).ToArray());
            }
        }

        private class FinalizeScopeBlockDefinition : IResolvelizeScope<WeakBlockDefinition, ISetUpScope>
        {
            public ISetUpScope SetUpSideNode { get; }
            private readonly IResolvelizeScope<IFrontendCodeElement,ISetUpSideNode>[] finalizeScope;

            public FinalizeScopeBlockDefinition(ISetUpScope setUpSideNode, IResolvelizeScope<IFrontendCodeElement,ISetUpSideNode>[] finalizeScope)
            {
                SetUpSideNode = setUpSideNode ?? throw new ArgumentNullException(nameof(setUpSideNode));
                this.finalizeScope = finalizeScope ?? throw new ArgumentNullException(nameof(finalizeScope));
            }

            public IPopulateBoxes<WeakBlockDefinition> Run(IResolvableScope parent,IFinalizeScopeContext context)
            {
                var scope = this.finalizableScope.FinalizeScope(parent);
                return new ResolveReferanceBlockDefinition(scope, finalizeScope.Select(x => x.Run(scope, context)).ToArray());
            }
        }

        private class ResolveReferanceBlockDefinition : IPopulateBoxes<WeakBlockDefinition>
        {
            private readonly IResolvableScope scope;
            private readonly IPopulateBoxes<IFrontendCodeElement>[] resolveReferance;

            public ResolveReferanceBlockDefinition(
                IResolvableScope scope,
                IPopulateBoxes<IFrontendCodeElement>[] resolveReferance)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.resolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            }

            public IIsPossibly<WeakBlockDefinition> Run(IResolvableScope _, IResolveReferenceContext context)
            {
                return
                        Possibly.Is(
                            new WeakBlockDefinition(
                                resolveReferance.Select(x => x.Run(scope,context)).ToArray(),
                                scope,
                                new IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>[0]));
            }

        }

    }

}