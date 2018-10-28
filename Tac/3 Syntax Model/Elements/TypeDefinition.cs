using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class WeakTypeDefinition : IWeakReturnable, IWeakCodeElement, IScoped
    {
        public WeakTypeDefinition(IWeakFinalizedScope scope, IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IKey Key { get; }
        public IWeakFinalizedScope Scope { get; }
        
        public IWeakReturnable Returns()
        {
            return this;
        }
    }
    
    public class TypeDefinitionMaker : IMaker<WeakTypeDefinition>
    {
        public TypeDefinitionMaker()
        {
        }
        
        
        public IResult<IPopulateScope<WeakTypeDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("type"), out var _)
                            .OptionalHas(ElementMatcher.IsName, out AtomicToken typeName)
                            .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                            .IsMatch)
            {

                var elements = matchingContext.ParseBlock(body);

                
               return ResultExtension.Good(new TypeDefinitionPopulateScope(
                   elements, 
                   typeName != default ? new NameKey(typeName.Item).Cast<IKey>(): new ImplicitKey()));
            }

            return ResultExtension.Bad<IPopulateScope<WeakTypeDefinition>>(); ;
        }
    }
    
    public class TypeDefinitionPopulateScope : IPopulateScope< WeakTypeDefinition>
    {
        private readonly IPopulateScope<IWeakCodeElement>[] elements;
        private readonly IKey key;
        private readonly Box<WeakTypeDefinition> box = new Box<WeakTypeDefinition>();

        public TypeDefinitionPopulateScope(IPopulateScope<IWeakCodeElement>[] elements, IKey typeName)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            key = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public IBox<IWeakReturnable> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakTypeDefinition> Run(IPopulateScopeContext context)
        {
            var encolsing = context.Scope.TryAddType(key, box);
            var nextContext = context.Child();
            elements.Select(x => x.Run(nextContext)).ToArray();
            return new TypeDefinitionResolveReference(
                nextContext.GetResolvableScope(), 
                box, 
                key);
        }
    }

    public class TypeDefinitionResolveReference : IPopulateBoxes<WeakTypeDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly Box<WeakTypeDefinition> box;
        private readonly IKey key;

        public TypeDefinitionResolveReference(IResolvableScope scope, Box<WeakTypeDefinition> box, IKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IOpenBoxes<WeakTypeDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(new WeakTypeDefinition(scope.GetFinalized(), key));
            return new TypeDefinitionOpenBoxes(item);
        }
    }

    internal class TypeDefinitionOpenBoxes : IOpenBoxes<WeakTypeDefinition>
    {
        public WeakTypeDefinition CodeElement { get; }

        public TypeDefinitionOpenBoxes(WeakTypeDefinition item)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.TypeDefinition(CodeElement);
        }
    }
}
