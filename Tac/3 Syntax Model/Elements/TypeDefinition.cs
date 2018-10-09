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
    public class TypeDefinition : IReturnable
    {
        public TypeDefinition(IResolvableScope scope, IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IKey Key { get; }
        public IResolvableScope Scope { get; }
        
        public IReturnable ReturnType()
        {
            return this;
        }
    }
    
    public class TypeDefinitionMaker : IMaker<TypeDefinition>
    {
        public TypeDefinitionMaker(Func<IResolvableScope, IKey, TypeDefinition> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<IResolvableScope, IKey, TypeDefinition> Make { get; }
        
        public IResult<IPopulateScope<TypeDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("type"), out var _)
                            .OptionalHas(ElementMatcher.IsName, out AtomicToken typeName)
                            .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                            .IsMatch)
            {
                var scope = Scope.LocalStaticScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(body);

                
               return ResultExtension.Good(new TypeDefinitionPopulateScope(scope, elements, typeName != default ? new NameKey(typeName.Item).Cast<IKey>(): new ImplicitKey() , Make));
            }

            return ResultExtension.Bad<IPopulateScope<TypeDefinition>>(); ;
        }
    }
    
    public class TypeDefinitionPopulateScope : IPopulateScope<TypeDefinition>
    {
        private readonly ILocalStaticScope scope;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly IKey key;
        private readonly Func<IResolvableScope, IKey, TypeDefinition> make;

        public TypeDefinitionPopulateScope(ILocalStaticScope scope, IPopulateScope<ICodeElement>[] elements, IKey typeName, Func<IResolvableScope, IKey, TypeDefinition> make)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<TypeDefinition> Run(IPopulateScopeContext context)
        {
            var box = new Box<TypeDefinition>();
            var encolsing = context.TryAddType(key, box);
            var nextContext = context.Child(this, scope);
            elements.Select(x => x.Run(nextContext)).ToArray();
            return new TypeDefinitionResolveReference(scope.ToResolvable(), box, make, key);
        }
    }

    public class TypeDefinitionResolveReference : IResolveReference<TypeDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly Box<TypeDefinition> box;
        private readonly IKey key;
        private readonly Func<IResolvableScope, IKey, TypeDefinition> make;

        public TypeDefinitionResolveReference(IResolvableScope scope, Box<TypeDefinition> box, Func<IResolvableScope, IKey, TypeDefinition> make, IKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IReturnable> GetReturnType(IResolveReferanceContext context)
        {
            return box;
        }

        public TypeDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(scope, key));
        }
    }
}
