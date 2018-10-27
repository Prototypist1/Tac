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


    public class WeakModuleDefinition : IScoped, IWeakCodeElement, IWeakReturnable
    {
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

        public IWeakReturnable Returns()
        {
            return this;
        }
    }


    public class ModuleDefinitionMaker : IMaker<WeakModuleDefinition>
    {
        public ModuleDefinitionMaker()
        {
        }
        

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
                
                return ResultExtension.Good(new ModuleDefinitionPopulateScope(elements, nameKey));

            }
            return ResultExtension.Bad<IPopulateScope<WeakModuleDefinition>>();
        }
    }
    
    public class ModuleDefinitionPopulateScope : IPopulateScope<WeakModuleDefinition>
    {
        private readonly IPopulateScope<IWeakCodeElement>[] elements;
        private readonly NameKey nameKey;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ModuleDefinitionPopulateScope(
            IPopulateScope<IWeakCodeElement>[] elements,
            NameKey nameKey)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public IBox<IWeakReturnable> GetReturnType()
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

    public class ModuleDefinitionResolveReferance : IPopulateBoxes<WeakModuleDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IPopulateBoxes<IWeakCodeElement>[] resolveReferance;
        private readonly NameKey nameKey;
        private readonly Box<IWeakReturnable> box;

        public ModuleDefinitionResolveReferance(
            IResolvableScope scope, 
            IPopulateBoxes<IWeakCodeElement>[] resolveReferance,
            NameKey nameKey,
            Box<IWeakReturnable> box)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.resolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IOpenBoxes<WeakModuleDefinition> Run(IResolveReferanceContext context)
        {
            var item =  box.Fill(new WeakModuleDefinition(
                scope.GetFinalized(), 
                resolveReferance.Select(x => x.Run(context).CodeElement).ToArray(),
                nameKey));
            return new MuldieDefinitionOpenBoxes(item);
        }
    }

    internal class MuldieDefinitionOpenBoxes : IOpenBoxes<WeakModuleDefinition>
    {
        public WeakModuleDefinition CodeElement { get; }

        public MuldieDefinitionOpenBoxes(WeakModuleDefinition item)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.ModuleDefinition(CodeElement);
        }
    }
}
