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

    public class ModuleDefinition : IScoped, ICodeElement, IReturnable
    {
        public delegate ModuleDefinition Make(IResolvableScope scope, IEnumerable<ICodeElement> staticInitialization, NameKey Ke);

        public ModuleDefinition(IResolvableScope scope, IEnumerable<ICodeElement> staticInitialization, NameKey Key)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
        }
        
        public IResolvableScope Scope { get; }
        public IEnumerable<ICodeElement> StaticInitialization { get; }

        public IKey Key
        {
            get;
        }

        public IReturnable ReturnType(IElementBuilders elementBuilders)
        {
            return this;
        }
    }


    public class ModuleDefinitionMaker : IMaker<ModuleDefinition>
    {
        public ModuleDefinitionMaker(ModuleDefinition.Make make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private ModuleDefinition.Make Make { get; }

        public IResult<IPopulateScope<ModuleDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("module"), out var frist)
                            .Has(ElementMatcher.IsName, out AtomicToken name)
                            .Has(ElementMatcher.IsBody, out CurleyBracketToken third)
                            .Has(ElementMatcher.IsDone)
                            .IsMatch)
            {

                var (scope,stack) = matchingContext.ScopeStack.StaticScope();

                var elementMatchingContext = matchingContext.Child(stack);
                var elements = elementMatchingContext.ParseBlock(third);
                var nameKey = new NameKey(name.Item);


                return ResultExtension.Good(new ModuleDefinitionPopulateScope(scope, elements, Make, nameKey));

            }
            return ResultExtension.Bad<IPopulateScope<ModuleDefinition>>();
        }
    }
    
    public class ModuleDefinitionPopulateScope : IPopulateScope<ModuleDefinition>
    {
        private readonly IStaticScope scope;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly ModuleDefinition.Make make;
        private readonly NameKey nameKey;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public ModuleDefinitionPopulateScope(
            IStaticScope scope, 
            IPopulateScope<ICodeElement>[] elements,
            ModuleDefinition.Make make, 
            NameKey nameKey)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<ModuleDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child(this,scope);
            return new ModuleDefinitionResolveReferance(scope.ToResolvable(), elements.Select(x => x.Run(nextContext)).ToArray(), make,nameKey,box);
        }

    }

    public class ModuleDefinitionResolveReferance : IResolveReference<ModuleDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IResolveReference<ICodeElement>[] resolveReferance;
        private readonly ModuleDefinition.Make make;
        private readonly NameKey nameKey;
        private readonly Box<IReturnable> box;

        public ModuleDefinitionResolveReferance(
            IResolvableScope scope, 
            IResolveReference<ICodeElement>[] resolveReferance,
            ModuleDefinition.Make make, 
            NameKey nameKey,
            Box<IReturnable> box)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.resolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public ModuleDefinition Run(IResolveReferanceContext context)
        {
            var nextContext = context.Child(this, scope);
            return box.Fill(make(scope, resolveReferance.Select(x => x.Run(nextContext)).ToArray(),nameKey));
        }
    }
}
