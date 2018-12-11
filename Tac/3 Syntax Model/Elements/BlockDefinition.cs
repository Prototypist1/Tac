using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    internal class WeakBlockDefinition : WeakAbstractBlockDefinition
    {
        public WeakBlockDefinition(
            IIsPossibly<IFrontendCodeElement>[] body,
            IFinalizedScope scope,
            IEnumerable<IIsPossibly<IFrontendCodeElement>> staticInitailizers) : 
            base(scope, body, staticInitailizers) { }
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
    }
    
    internal class BlockDefinitionPopulateScope : IPopulateScope<WeakBlockDefinition>
    {
        // TODO object??
        // is it worth adding another T?
        // this is the type the backend owns
        private IPopulateScope<IFrontendCodeElement>[] Elements { get; }
        private readonly Box<IIsPossibly<IFrontendType>> box = new Box<IIsPossibly<IFrontendType>>();

        public BlockDefinitionPopulateScope(IPopulateScope<IFrontendCodeElement>[] elements)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public IPopulateBoxes<WeakBlockDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            return new ResolveReferanceBlockDefinition(
                nextContext.GetResolvableScope(), 
                Elements.Select(x => x.Run(nextContext)).ToArray(), 
                box);
        }

        public IBox<IIsPossibly<IFrontendType>> GetReturnType()
        {
            return box;
        }
    }

    internal class ResolveReferanceBlockDefinition : IPopulateBoxes<WeakBlockDefinition>
    {
        private IResolvableScope Scope { get; }
        private IPopulateBoxes<IFrontendCodeElement>[] ResolveReferance { get; }
        private readonly Box<IIsPossibly<IFrontendType>> box;

        public ResolveReferanceBlockDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<IFrontendCodeElement>[] resolveReferance,
            Box<IIsPossibly<IFrontendType>> box)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ResolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IIsPossibly<WeakBlockDefinition> Run(IResolveReferenceContext context)
        {
            return 
                box.Fill(
                    Possibly.Is(
                        new WeakBlockDefinition(
                            ResolveReferance.Select(x => x.Run(context)).ToArray(), 
                            Scope.GetFinalized(), 
                            new IIsPossibly<IFrontendCodeElement>[0])));
        }
        
    }
    

}