using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    internal class WeakBlockDefinition : WeakAbstractBlockDefinition, IBlockDefinition
    {
        public WeakBlockDefinition(
            ICodeElement[] body,
            IFinalizedScope scope,
            IEnumerable<ICodeElement> staticInitailizers) : 
            base(scope, body, staticInitailizers) { }

        #region IBlockDefinition

        IFinalizedScope IAbstractBlockDefinition.Scope => Scope;

        #endregion
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.BlockDefinition(this);
        }
    }

    internal class BlockDefinitionMaker : IMaker<WeakBlockDefinition>
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


    internal class BlockDefinitionPopulateScope : IPopulateScope<WeakBlockDefinition>
    {
        // TODO object??
        // is it worth adding another T?
        // this is the type the backend owns
        private IPopulateScope<ICodeElement>[] Elements { get; }
        private readonly Box<IVarifiableType> box = new Box<IVarifiableType>();

        public BlockDefinitionPopulateScope(IPopulateScope<ICodeElement>[] elements)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public IPopulateBoxes<WeakBlockDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            return new ResolveReferanceBlockDefinition(nextContext.GetResolvableScope(), Elements.Select(x => x.Run(nextContext)).ToArray(), box);
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }
    }

    internal class ResolveReferanceBlockDefinition : IPopulateBoxes<WeakBlockDefinition>
    {
        private IResolvableScope Scope { get; }
        private IPopulateBoxes<ICodeElement>[] ResolveReferance { get; }
        private readonly Box<IVarifiableType> box;

        public ResolveReferanceBlockDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<ICodeElement>[] resolveReferance,
            Box<IVarifiableType> box)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ResolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public WeakBlockDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(new WeakBlockDefinition(
                ResolveReferance.Select(x => x.Run(context)).ToArray(), 
                Scope.GetFinalized(), 
                new ICodeElement[0]));
        }
        
    }
    

}