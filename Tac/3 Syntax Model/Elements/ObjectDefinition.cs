using Prototypist.LeftToRight;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{

    internal class WeakObjectDefinition: IFrontendCodeElement<IObjectDefiniton>,  IScoped, IFrontendType<IVarifiableType>
    {
        public WeakObjectDefinition(IResolvableScope scope, IEnumerable<IIsPossibly<WeakAssignOperation>> assigns, ImplicitKey key) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Assignments = assigns.ToArray();
        }

        public IResolvableScope Scope { get; }
        public IIsPossibly<WeakAssignOperation>[] Assignments { get; }

        public IKey Key
        {
            get;
        }

        public IBuildIntention<IObjectDefiniton> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = ObjectDefiniton.Create();
            return new BuildIntention<IObjectDefiniton>(toBuild, () =>
            {
                maker.Build(Scope.Convert(context), 
                    Assignments.Select(x => x.GetOrThrow().Convert(context)).ToArray());
            });
        }

        IBuildIntention<IVarifiableType> IConvertable<IVarifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);

        IIsPossibly<IFrontendType<IVarifiableType>> IFrontendCodeElement<IObjectDefiniton>.Returns()
        {
            return Possibly.Is(this);
        }
    }

    internal class ObjectDefinitionMaker : IMaker<IPopulateScope<WeakObjectDefinition>>
    {
        public ObjectDefinitionMaker()
        {
        }

        public ITokenMatching<IPopulateScope<WeakObjectDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("object"), out var keyword)
                .Has(new BodyMaker(), out var block);
            if (matching is IMatchedTokenMatching matched)
            {

                var elements = tokenMatching.Context.ParseBlock(block);
                
                return TokenMatching<IPopulateScope<WeakObjectDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new ObjectDefinitionPopulateScope(elements));
            }
            return TokenMatching<IPopulateScope<WeakObjectDefinition>>.MakeNotMatch(
                    matching.Context);
        }
    }

    internal class ObjectDefinitionPopulateScope : IPopulateScope<WeakObjectDefinition>
    {
        private readonly IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements;
        private readonly Box<IIsPossibly<IFrontendType<IVarifiableType>>> box = new Box<IIsPossibly<IFrontendType<IVarifiableType>>>();

        public ObjectDefinitionPopulateScope(IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public IBox<IIsPossibly<IFrontendType<IVarifiableType>>> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakObjectDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            var key = new ImplicitKey();
            nextContext.Scope.TryAddType(key, box);
            return new ResolveReferanceObjectDefinition(
                nextContext.GetResolvableScope(),
                elements.Select(x => x.Run(nextContext)).ToArray(),
                box,
                key);
        }
    }

    internal class ResolveReferanceObjectDefinition : IPopulateBoxes<WeakObjectDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] elements;
        private readonly Box<IIsPossibly<IFrontendType<IVarifiableType>>> box;
        private readonly ImplicitKey key;

        public ResolveReferanceObjectDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] elements, 
            Box<IIsPossibly<IFrontendType<IVarifiableType>>> box, 
            ImplicitKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IIsPossibly<WeakObjectDefinition> Run(IResolveReferenceContext context)
        {
            return box.Fill(
                Possibly.Is(
                    new WeakObjectDefinition(
                        scope, 
                        elements.Select(x => x.Run(context).Cast<IIsPossibly<WeakAssignOperation>>()).ToArray(), 
                        key)));
        }
    }
}
