using System;
using System.Collections.Generic;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    public class BlockDefinition : AbstractBlockDefinition
    {

        public delegate BlockDefinition Make(ICodeElement[] body,
            IResolvableScope scope,
            IEnumerable<ICodeElement> staticInitailizers);

        public BlockDefinition(
            ICodeElement[] body,
            IResolvableScope scope,
            IEnumerable<ICodeElement> staticInitailizers) : 
            base(scope, body, staticInitailizers) { }
        
    }

    public class BlockDefinitionMaker : IMaker<BlockDefinition>
    {
        public BlockDefinitionMaker(BlockDefinition.Make make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private BlockDefinition.Make Make { get; }

        public IResult<IPopulateScope<BlockDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
               .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
               .Has(ElementMatcher.IsDone)
               .IsMatch)
            {
                var elements = matchingContext.ParseBlock(body);

                return ResultExtension.Good(new BlockDefinitionPopulateScope( elements, Make));
            }

            return ResultExtension.Bad<IPopulateScope<BlockDefinition>>();
        }
    }


    public class BlockDefinitionPopulateScope : IPopulateScope<BlockDefinition>, IReturnable
    {
        private IPopulateScope<ICodeElement>[] Elements { get; }
        public BlockDefinition.Make Make { get; }
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public BlockDefinitionPopulateScope(IPopulateScope<ICodeElement>[] elements, BlockDefinition.Make make)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<BlockDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            return new ResolveReferanceBlockDefinition(nextContext.GetResolvableScope(), Elements.Select(x => x.Run(nextContext)).ToArray(), Make,box);
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }
    }

    public class ResolveReferanceBlockDefinition : IResolveReference<BlockDefinition>
    {
        private IResolvableScope Scope { get; }
        private IResolveReference<ICodeElement>[] ResolveReferance { get; }
        private BlockDefinition.Make Make { get; }
        private readonly Box<IReturnable> box;

        public ResolveReferanceBlockDefinition(
            IResolvableScope scope, 
            IResolveReference<ICodeElement>[] resolveReferance,
            BlockDefinition.Make make,
            Box<IReturnable> box)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ResolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public BlockDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(Make(ResolveReferance.Select(x => x.Run(context)).ToArray(), Scope, new ICodeElement[0]));
        }
    }
}