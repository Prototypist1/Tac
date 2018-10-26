using System;
using System.Collections.Generic;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public interface IBlockDefinition: IAbstractBlockDefinition
    {
    }

    public class WeakBlockDefinition : WeakAbstractBlockDefinition
    {

        public delegate WeakBlockDefinition Make(IWeakCodeElement[] body,
            IWeakFinalizedScope scope,
            IEnumerable<IWeakCodeElement> staticInitailizers);

        public WeakBlockDefinition(
            IWeakCodeElement[] body,
            IWeakFinalizedScope scope,
            IEnumerable<IWeakCodeElement> staticInitailizers) : 
            base(scope, body, staticInitailizers) { }
        
    }

    public class BlockDefinitionMaker : IMaker<WeakBlockDefinition>
    {
        public BlockDefinitionMaker(WeakBlockDefinition.Make make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private WeakBlockDefinition.Make Make { get; }

        public IResult<IPopulateScope<WeakBlockDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
               .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
               .Has(ElementMatcher.IsDone)
               .IsMatch)
            {
                var elements = matchingContext.ParseBlock(body);

                return ResultExtension.Good(new BlockDefinitionPopulateScope( elements, Make));
            }

            return ResultExtension.Bad<IPopulateScope<WeakBlockDefinition>>();
        }
    }


    public class BlockDefinitionPopulateScope : IPopulateScope<WeakBlockDefinition>, IWeakReturnable
    {
        private IPopulateScope<IWeakCodeElement>[] Elements { get; }
        public WeakBlockDefinition.Make Make { get; }
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public BlockDefinitionPopulateScope(IPopulateScope<IWeakCodeElement>[] elements, WeakBlockDefinition.Make make)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IPopulateBoxes<WeakBlockDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            return new ResolveReferanceBlockDefinition(nextContext.GetResolvableScope(), Elements.Select(x => x.Run(nextContext)).ToArray(), Make,box);
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }
    }

    public class ResolveReferanceBlockDefinition : IPopulateBoxes<WeakBlockDefinition>
    {
        private IResolvableScope Scope { get; }
        private IPopulateBoxes<IWeakCodeElement>[] ResolveReferance { get; }
        private WeakBlockDefinition.Make Make { get; }
        private readonly Box<IWeakReturnable> box;

        public ResolveReferanceBlockDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<IWeakCodeElement>[] resolveReferance,
            WeakBlockDefinition.Make make,
            Box<IWeakReturnable> box)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ResolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public WeakBlockDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(Make(ResolveReferance.Select(x => x.Run(context)).ToArray(), Scope.GetFinalized(), new IWeakCodeElement[0]));
        }
    }
}