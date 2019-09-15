using System;
using System.Collections.Generic;
using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement>> StaticBlockDefinitionMaker = AddElementMakers(
            () => new BlockDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement>> BlockDefinitionMaker = StaticBlockDefinitionMaker;
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

    internal class BlockDefinitionMaker : IMaker<IPopulateScope<WeakBlockDefinition>>
    {
        public BlockDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakBlockDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
               .Has(new BodyMaker(), out var body);

            if (match is IMatchedTokenMatching
               matched)
            {
                var elements = tokenMatching.Context.ParseBlock(body);

                return TokenMatching<IPopulateScope<WeakBlockDefinition>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, 
                    new BlockDefinitionPopulateScope(elements));
            }

            return TokenMatching<IPopulateScope<WeakBlockDefinition>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakBlockDefinition> PopulateScope(IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements)
        {
            return new BlockDefinitionPopulateScope(elements);
        }
        public static IPopulateBoxes<WeakBlockDefinition> PopulateBoxes(IResolvableScope scope,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] resolveReferance)
        {
            return new ResolveReferanceBlockDefinition(scope, resolveReferance);
        }

        private class BlockDefinitionPopulateScope : IPopulateScope<WeakBlockDefinition>
        {
            // TODO object??
            // is it worth adding another T?
            // this is the type the backend owns
            private IPopulateScope<IFrontendCodeElement>[] Elements { get; }

            public BlockDefinitionPopulateScope(IPopulateScope<IFrontendCodeElement>[] elements)
            {
                Elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public IResolvelizeScope<WeakBlockDefinition> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {
                var myScope = scope.AddChild();
                return new FinalizeScopeBlockDefinition(
                    myScope.GetResolvelizableScope(),
                    Elements.Select(x => x.Run(myScope, context)).ToArray());
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateBlockType()));
            }

        }

        private class FinalizeScopeBlockDefinition : IResolvelizeScope<WeakBlockDefinition>
        {
            private readonly IResolvelizableScope finalizableScope;
            private readonly IResolvelizeScope<IFrontendCodeElement>[] finalizeScope;

            public FinalizeScopeBlockDefinition(IResolvelizableScope finalizableScope, IResolvelizeScope<IFrontendCodeElement>[] finalizeScope)
            {
                this.finalizableScope = finalizableScope ?? throw new ArgumentNullException(nameof(finalizableScope));
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