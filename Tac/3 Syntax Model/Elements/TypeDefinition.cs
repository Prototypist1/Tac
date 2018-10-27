using Prototypist.LeftToRight;
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
    public interface ITypeDefinition : ICodeElement, IReturnable
    {
        // why does this know it own key?
        IKey Key { get; }
        IFinalizedScope Scope { get; }
    }

    public class WeakTypeDefinition : IWeakReturnable, IWeakCodeElement, IScoped
    {
        public WeakTypeDefinition(IWeakFinalizedScope scope, IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IKey Key { get; }
        public IWeakFinalizedScope Scope { get; }
        
        public IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return this;
        }
    }
    
    public class TypeDefinitionMaker<T> : IMaker<T, WeakTypeDefinition>
    {
        public TypeDefinitionMaker(Func<WeakTypeDefinition,T> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<WeakTypeDefinition,T> Make { get; }
        
        public IResult<IPopulateScope<T, WeakTypeDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
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
                   typeName != default ? new NameKey(typeName.Item).Cast<IKey>(): new ImplicitKey() , 
                   Make));
            }

            return ResultExtension.Bad<IPopulateScope<T, WeakTypeDefinition>>(); ;
        }
    }
    
    public class TypeDefinitionPopulateScope<T> : IPopulateScope<T, WeakTypeDefinition>
    {
        private readonly IPopulateScope<,IWeakCodeElement>[] elements;
        private readonly IKey key;
        private readonly Func<WeakTypeDefinition,T> make;
        private readonly Box<WeakTypeDefinition> box = new Box<WeakTypeDefinition>();

        public TypeDefinitionPopulateScope(IPopulateScope<,IWeakCodeElement>[] elements, IKey typeName, Func<WeakTypeDefinition,T> make)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<T, WeakTypeDefinition> Run(IPopulateScopeContext context)
        {
            var encolsing = context.Scope.TryAddType(key, box);
            var nextContext = context.Child();
            elements.Select(x => x.Run(nextContext)).ToArray();
            return new TypeDefinitionResolveReference<T>(
                nextContext.GetResolvableScope(), 
                box, 
                make, 
                key);
        }
    }

    public class TypeDefinitionResolveReference<T> : IPopulateBoxes<T, WeakTypeDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly Box<WeakTypeDefinition> box;
        private readonly IKey key;
        private readonly Func<WeakTypeDefinition,T> make;

        public TypeDefinitionResolveReference(IResolvableScope scope, Box<WeakTypeDefinition> box, Func<WeakTypeDefinition,T> make, IKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IOpenBoxes<T, WeakTypeDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(new WeakTypeDefinition(scope.GetFinalized(), key));
            return new TypeDefinitionOpenBoxes<T>(item, make);
        }
    }

    internal class TypeDefinitionOpenBoxes<T> : IOpenBoxes<T, WeakTypeDefinition>
    {
        public WeakTypeDefinition CodeElement { get; }
        private readonly Func<WeakTypeDefinition, T> make;

        public TypeDefinitionOpenBoxes(WeakTypeDefinition item, Func<WeakTypeDefinition, T> make)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public T Run(IOpenBoxesContext context)
        {
            return make(CodeElement);
        }
    }
}
