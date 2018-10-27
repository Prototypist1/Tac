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

    public class BlockDefinitionMaker : IMaker<WeakBlockDefinition>
    {
        public BlockDefinitionMaker()
        {
        }
        

        public IResult<IPopulateScope<WeakBlockDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
               .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
               .Has(ElementMatcher.IsDone)
               .IsMatch)
            {
                var elements = matchingContext.ParseBlock(body);

                return ResultExtension.Good(new BlockDefinitionPopulateScope( elements));
            }

            return ResultExtension.Bad<IPopulateScope<WeakBlockDefinition>>();
        }
    }


    public class BlockDefinitionPopulateScope : IPopulateScope<WeakBlockDefinition>
    {
        // TODO object??
        // is it worth adding another T?
        // this is the type the backend owns
        private IPopulateScope<IWeakCodeElement>[] Elements { get; }
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public BlockDefinitionPopulateScope(IPopulateScope<IWeakCodeElement>[] elements)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public IPopulateBoxes<WeakBlockDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            return new ResolveReferanceBlockDefinition(nextContext.GetResolvableScope(), Elements.Select(x => x.Run(nextContext)).ToArray(), box);
        }

        public IBox<IWeakReturnable> GetReturnType()
        {
            return box;
        }
    }

    public class ResolveReferanceBlockDefinition : IPopulateBoxes<WeakBlockDefinition>
    {
        private IResolvableScope Scope { get; }
        private IPopulateBoxes<IWeakCodeElement>[] ResolveReferance { get; }
        private readonly Box<IWeakReturnable> box;

        public ResolveReferanceBlockDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<IWeakCodeElement>[] resolveReferance,
            Box<IWeakReturnable> box)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ResolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IOpenBoxes< WeakBlockDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(new WeakBlockDefinition(ResolveReferance.Select(x => x.Run(context).CodeElement).ToArray(), Scope.GetFinalized(), new IWeakCodeElement[0]));
            return new BlockDefinitionOpenBoxes(item);
        }
        
    }

    public class BlockDefinitionOpenBoxes : IOpenBoxes<WeakBlockDefinition>
    {
        public WeakBlockDefinition CodeElement { get; }

        public BlockDefinitionOpenBoxes(WeakBlockDefinition item)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.BlockDefinition(CodeElement);
        }
    }

}