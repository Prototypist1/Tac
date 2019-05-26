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
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model
{

    internal class WeakBlockDefinition : WeakAbstractBlockDefinition<IBlockDefinition>
    {
        public WeakBlockDefinition(
            IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>[] body,
            IResolvableScope scope,
            IEnumerable<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitailizers) : 
            base(scope, body, staticInitailizers) { }

        public override IBuildIntention<IBlockDefinition> GetBuildIntention(ConversionContext context)
        {
            var (toBuild, maker) = BlockDefinition.Create();
            return new BuildIntention<IBlockDefinition>(toBuild, () =>
            {
                maker.Build(
                    Scope.Convert(context),
                    Body.Select(x => x.GetOrThrow().Convert(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().Convert(context)).ToArray());
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

                return TokenMatching<IPopulateScope<WeakBlockDefinition>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new BlockDefinitionPopulateScope(elements));
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
            private IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] Elements { get; }

            public BlockDefinitionPopulateScope(IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements)
            {
                Elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public IPopulateBoxes<WeakBlockDefinition> Run(IPopulateScopeContext context)
            {
                var nextContext = context.Child();
                return new ResolveReferanceBlockDefinition(
                    nextContext.GetResolvableScope(),
                    Elements.Select(x => x.Run(nextContext)).ToArray());
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return new Box<IIsPossibly<IFrontendType>>(Possibly.Is<IFrontendType>(PrimitiveTypes.CreateBlockType()));
            }

        }

        private class ResolveReferanceBlockDefinition : IPopulateBoxes<WeakBlockDefinition>
        {
            private IResolvableScope Scope { get; }
            private IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] ResolveReferance { get; }

            public ResolveReferanceBlockDefinition(
                IResolvableScope scope,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] resolveReferance)
            {
                Scope = scope ?? throw new ArgumentNullException(nameof(scope));
                ResolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            }

            public IIsPossibly<WeakBlockDefinition> Run(IResolveReferenceContext context)
            {
                return
                        Possibly.Is(
                            new WeakBlockDefinition(
                                ResolveReferance.Select(x => x.Run(context)).ToArray(),
                                Scope,
                                new IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>[0]));
            }

        }


    }

}