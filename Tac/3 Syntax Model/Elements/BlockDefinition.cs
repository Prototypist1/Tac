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
        public WeakBlockDefinition(
            IWeakCodeElement[] body,
            IWeakFinalizedScope scope,
            IEnumerable<IWeakCodeElement> staticInitailizers) : 
            base(scope, body, staticInitailizers) { }
        
    }

    public class BlockDefinitionMaker<T> : IMaker<T, WeakBlockDefinition>
    {
        public BlockDefinitionMaker(Func<WeakBlockDefinition, T> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<WeakBlockDefinition, T> Make { get; }

        public IResult<IPopulateScope<T, WeakBlockDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
               .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
               .Has(ElementMatcher.IsDone)
               .IsMatch)
            {
                var elements = matchingContext.ParseBlock(body);

                return ResultExtension.Good(new BlockDefinitionPopulateScope<T>( elements, Make));
            }

            return ResultExtension.Bad<IPopulateScope<T, WeakBlockDefinition>>();
        }
    }


    public class BlockDefinitionPopulateScope<T> : IPopulateScope<T, WeakBlockDefinition>
    {
        // TODO object??
        // is it worth adding another T?
        // this is the type the backend owns
        private IPopulateScope<object,IWeakCodeElement>[] Elements { get; }
        public Func<WeakBlockDefinition, T> Make { get; }
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public BlockDefinitionPopulateScope(IPopulateScope<object,IWeakCodeElement>[] elements, Func<WeakBlockDefinition, T> make)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IPopulateBoxes<T, WeakBlockDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            return new ResolveReferanceBlockDefinition<T>(nextContext.GetResolvableScope(), Elements.Select(x => x.Run(nextContext)).ToArray(), Make,box);
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }
    }

    public class ResolveReferanceBlockDefinition<T> : IPopulateBoxes<T, WeakBlockDefinition>
    {
        private IResolvableScope Scope { get; }
        private IPopulateBoxes<object,IWeakCodeElement>[] ResolveReferance { get; }
        private Func<WeakBlockDefinition, T> Make { get; }
        private readonly Box<IWeakReturnable> box;

        public ResolveReferanceBlockDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<object,IWeakCodeElement>[] resolveReferance,
            Func<WeakBlockDefinition, T> make,
            Box<IWeakReturnable> box)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ResolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IOpenBoxes<T, WeakBlockDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(new WeakBlockDefinition(ResolveReferance.Select(x => x.Run(context).CodeElement).ToArray(), Scope.GetFinalized(), new IWeakCodeElement[0]));
            return new ResolveReferanceOpenBoxes<T>(item,Make);
        }
        
    }

    public class ResolveReferanceOpenBoxes<T> : IOpenBoxes<T, WeakBlockDefinition>
    {
        public WeakBlockDefinition CodeElement { get; }
        private readonly Func<WeakBlockDefinition, T> make;

        public ResolveReferanceOpenBoxes(WeakBlockDefinition item, Func<WeakBlockDefinition, T> make)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }
    }

}