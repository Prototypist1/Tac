using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{


    internal class WeakModuleDefinition : IScoped, ICodeElement, IType, IModuleDefinition
    {
        public WeakModuleDefinition(IWeakFinalizedScope scope, IEnumerable<ICodeElement> staticInitialization, NameKey Key)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
        }
        
        public IWeakFinalizedScope Scope { get; }
        public IEnumerable<ICodeElement> StaticInitialization { get; }

        public IKey Key
        {
            get;
        }

        #region IModuleDefinition
        
        IFinalizedScope IModuleDefinition.Scope => Scope;

        #endregion
        
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ModuleDefinition(this);
        }
        
        public IType Returns()
        {
            return this;
        }
    }


    internal class ModuleDefinitionMaker : IMaker<WeakModuleDefinition>
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

    internal class ModuleDefinitionPopulateScope : IPopulateScope<WeakModuleDefinition>
    {
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly NameKey nameKey;
        private readonly Box<IType> box = new Box<IType>();

        public ModuleDefinitionPopulateScope(
            IPopulateScope<ICodeElement>[] elements,
            NameKey nameKey)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public IBox<IType> GetReturnType()
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
        private readonly IPopulateBoxes<ICodeElement>[] resolveReferance;
        private readonly NameKey nameKey;
        private readonly Box<IType> box;

        public ModuleDefinitionResolveReferance(
            IResolvableScope scope, 
            IPopulateBoxes<ICodeElement>[] resolveReferance,
            NameKey nameKey,
            Box<IType> box)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.resolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public WeakModuleDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(new WeakModuleDefinition(
                scope.GetFinalized(), 
                resolveReferance.Select(x => x.Run(context)).ToArray(),
                nameKey));
        }
    }
}
