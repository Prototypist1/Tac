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
    public class TypeDefinition : IReturnable, ICodeElement
    {
        public delegate TypeDefinition Make(IResolvableScope scope, IKey key);

        public TypeDefinition(IResolvableScope scope, IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IKey Key { get; }
        public IResolvableScope Scope { get; }
        
        public IReturnable Returns(IElementBuilders elementBuilders)
        {
            return this;
        }
    }
    
    public class TypeDefinitionMaker : IMaker<TypeDefinition>
    {
        public TypeDefinitionMaker(TypeDefinition.Make make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private TypeDefinition.Make Make { get; }
        
        public IResult<IPopulateScope<TypeDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("type"), out var _)
                            .OptionalHas(ElementMatcher.IsName, out AtomicToken typeName)
                            .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                            .IsMatch)
            {
                var (scope,stack) = matchingContext.ScopeStack.LocalStaticScope();

                var elementMatchingContext = matchingContext.Child(stack);
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
        private readonly TypeDefinition.Make make;
        private readonly Box<TypeDefinition> box = new Box<TypeDefinition>();

        public TypeDefinitionPopulateScope(ILocalStaticScope scope, IPopulateScope<ICodeElement>[] elements, IKey typeName, TypeDefinition.Make make)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<TypeDefinition> Run(IPopulateScopeContext context)
        {
            var encolsing = context.TryAddType(key, box);
            var nextContext = context.Child(scope);
            elements.Select(x => x.Run(nextContext)).ToArray();
            return new TypeDefinitionResolveReference(scope.ToResolvable(), box, make, key);
        }
    }

    public class TypeDefinitionResolveReference : IResolveReference<TypeDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly Box<TypeDefinition> box;
        private readonly IKey key;
        private readonly TypeDefinition.Make make;

        public TypeDefinitionResolveReference(IResolvableScope scope, Box<TypeDefinition> box, TypeDefinition.Make make, IKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public TypeDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(scope, key));
        }
    }
}
