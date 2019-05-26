using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;

namespace Tac.Semantic_Model
{


    internal class WeakModuleDefinition : IScoped, IConvertableFrontendCodeElement<IModuleDefinition>, IFrontendType
    {
        public WeakModuleDefinition(IResolvableScope scope, IEnumerable<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitialization, IKey Key)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
        }
        
        public IResolvableScope Scope { get; }
        public IEnumerable<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> StaticInitialization { get; }

        public IKey Key
        {
            get;
        }

        public IBuildIntention<IModuleDefinition> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = ModuleDefinition.Create();
            return new BuildIntention<IModuleDefinition>(toBuild, () =>
            {
                maker.Build(Scope.Convert(context), StaticInitialization.Select(x=>x.GetOrThrow().Convert(context)).ToArray(),Key);
            });
        }

        public IIsPossibly<IFrontendType> Returns()
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
                .Has(new KeyWordMaker("module"), out _)
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
        
        private class ModuleDefinitionPopulateScope : IPopulateScope<WeakModuleDefinition>
        {
            private readonly IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements;
            private readonly NameKey nameKey;
            private readonly Box<IIsPossibly<IFrontendType>> box = new Box<IIsPossibly<IFrontendType>>();

            public ModuleDefinitionPopulateScope(
                IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements,
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

        public static IPopulateScope<WeakModuleDefinition> PopulateScope(IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>>[] elements,
                NameKey nameKey)
        {
            return new ModuleDefinitionPopulateScope(elements,
                nameKey);
        }
        public static IPopulateBoxes<WeakModuleDefinition> PopulateBoxes(IResolvableScope scope,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] resolveReferance,
                NameKey nameKey,
                Box<IIsPossibly<IFrontendType>> box)
        {
            return new ModuleDefinitionResolveReferance(scope,
               resolveReferance,
               nameKey,
               box);
        }

        private class ModuleDefinitionResolveReferance : IPopulateBoxes<WeakModuleDefinition>
        {
            private readonly IResolvableScope scope;
            private readonly IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] resolveReferance;
            private readonly NameKey nameKey;
            private readonly Box<IIsPossibly<IFrontendType>> box;

            public ModuleDefinitionResolveReferance(
                IResolvableScope scope,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] resolveReferance,
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
                var innerRes = new WeakModuleDefinition(
                        scope,
                        resolveReferance.Select(x => x.Run(context)).ToArray(),
                        nameKey);

                var res = Possibly.Is(innerRes);

                box.Fill(innerRes.Returns());

                return res;
            }
        }
    }
}
