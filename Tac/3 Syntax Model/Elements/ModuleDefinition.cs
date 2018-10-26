using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IModuleDefinition : ICodeElement, IReturnable
    {
        IFinalizedScope Scope { get; }
        IEnumerable<IWeakCodeElement> StaticInitialization { get; }
        // why does this know it's own key??
        IKey Key{get; }
    }


    public class WeakModuleDefinition : IScoped, IWeakCodeElement, IWeakReturnable
    {
        public delegate WeakModuleDefinition Make(IWeakFinalizedScope scope, IEnumerable<IWeakCodeElement> staticInitialization, NameKey Ke);

        public WeakModuleDefinition(IWeakFinalizedScope scope, IEnumerable<IWeakCodeElement> staticInitialization, NameKey Key)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
        }
        
        public IWeakFinalizedScope Scope { get; }
        public IEnumerable<IWeakCodeElement> StaticInitialization { get; }

        public IKey Key
        {
            get;
        }

        public IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return this;
        }
    }


    public class ModuleDefinitionMaker : IMaker<WeakModuleDefinition>
    {
        public ModuleDefinitionMaker(WeakModuleDefinition.Make make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private WeakModuleDefinition.Make Make { get; }

        public IResult<IPopulateScope<WeakModuleDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("module"), out var frist)
                            .Has(ElementMatcher.IsName, out AtomicToken name)
                            .Has(ElementMatcher.IsBody, out CurleyBracketToken third)
                            .Has(ElementMatcher.IsDone)
                            .IsMatch)
            {


                var elements = matchingContext.ParseBlock(third);
                var nameKey = new NameKey(name.Item);
                
                return ResultExtension.Good(new ModuleDefinitionPopulateScope(elements, Make, nameKey));

            }
            return ResultExtension.Bad<IPopulateScope<WeakModuleDefinition>>();
        }
    }
    
    public class ModuleDefinitionPopulateScope : IPopulateScope<WeakModuleDefinition>
    {
        private readonly IPopulateScope<IWeakCodeElement>[] elements;
        private readonly WeakModuleDefinition.Make make;
        private readonly NameKey nameKey;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ModuleDefinitionPopulateScope(
            IPopulateScope<IWeakCodeElement>[] elements,
            WeakModuleDefinition.Make make, 
            NameKey nameKey)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<WeakModuleDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            return new ModuleDefinitionResolveReferance(
                nextContext.GetResolvableScope(),
                elements.Select(x => x.Run(nextContext)).ToArray(),
                make,
                nameKey,
                box);
        }

    }

    public class ModuleDefinitionResolveReferance : IPopulateBoxes<WeakModuleDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IPopulateBoxes<IWeakCodeElement>[] resolveReferance;
        private readonly WeakModuleDefinition.Make make;
        private readonly NameKey nameKey;
        private readonly Box<IWeakReturnable> box;

        public ModuleDefinitionResolveReferance(
            IResolvableScope scope, 
            IPopulateBoxes<IWeakCodeElement>[] resolveReferance,
            WeakModuleDefinition.Make make, 
            NameKey nameKey,
            Box<IWeakReturnable> box)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.resolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public WeakModuleDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(scope.GetFinalized(), resolveReferance.Select(x => x.Run(context)).ToArray(),nameKey));
        }
    }
}
