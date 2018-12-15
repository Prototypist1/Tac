using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{


    internal class WeakModuleDefinition : IScoped, IFrontendCodeElement<IModuleDefinition>, IFrontendType
    {
        public WeakModuleDefinition(IFinalizedScope scope, IEnumerable<IIsPossibly<IFrontendCodeElement<ICodeElement>>> staticInitialization, NameKey Key)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
        }
        
        public IFinalizedScope Scope { get; }
        public IEnumerable<IIsPossibly<IFrontendCodeElement<ICodeElement>>> StaticInitialization { get; }

        public IKey Key
        {
            get;
        }

        public IBuildIntention<IModuleDefinition> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = ModuleDefinition.Create();
            return new BuildIntention<IModuleDefinition>(toBuild, () =>
            {
                maker.Build(Scope,StaticInitialization.Select(x=>x.GetOrThrow().Convert(context)));
            });
        }

        IIsPossibly<IFrontendType> IFrontendCodeElement<IModuleDefinition>.Returns()
        {
            return Possibly.Is(this);
        }
    }
    
    internal class ModuleDefinitionMaker : IMaker<IPopulateScope<WeakModuleDefinition>>
    {
        public ModuleDefinitionMaker()
        {
        }
        

        public ITokenMatching<IPopulateScope<WeakModuleDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("module"), out var frist)
                .Has(new NameMaker(), out var name)
                .Has(new BodyMaker(), out var third);
            if (matching is IMatchedTokenMatching matched)
            {
                var elements = matching.Context.ParseBlock(third);
                var nameKey = new NameKey(name.Item);

                return TokenMatching<IPopulateScope<WeakModuleDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new ModuleDefinitionPopulateScope(elements, nameKey));

            }
            return TokenMatching<IPopulateScope<WeakModuleDefinition>>.MakeNotMatch(
                    matching.Context);
        }
    }

    internal class ModuleDefinitionPopulateScope : IPopulateScope<WeakModuleDefinition>
    {
        private readonly IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements;
        private readonly NameKey nameKey;
        private readonly Box<IIsPossibly<IFrontendType>> box = new Box<IIsPossibly<IFrontendType>>();

        public ModuleDefinitionPopulateScope(
            IPopulateScope<IFrontendCodeElement<ICodeElement>>[] elements,
            NameKey nameKey)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public IBox<IIsPossibly<IFrontendType>> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakModuleDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            return new ModuleDefinitionResolveReferance(
                nextContext.GetResolvableScope(),
                elements.Select(x => x.Run(nextContext)).ToArray(),
                nameKey,
                box);
        }

    }

    internal class ModuleDefinitionResolveReferance : IPopulateBoxes<WeakModuleDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] resolveReferance;
        private readonly NameKey nameKey;
        private readonly Box<IIsPossibly<IFrontendType>> box;

        public ModuleDefinitionResolveReferance(
            IResolvableScope scope, 
            IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] resolveReferance,
            NameKey nameKey,
            Box<IIsPossibly<IFrontendType>> box)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.resolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IIsPossibly<WeakModuleDefinition> Run(IResolveReferenceContext context)
        {
            return box.Fill(
                Possibly.Is(
                    new WeakModuleDefinition(
                    scope.GetFinalized(), 
                    resolveReferance.Select(x => x.Run(context)).ToArray(),
                    nameKey)));
        }
    }
}
