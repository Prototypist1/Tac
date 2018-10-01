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

    public interface ITypeDefinition : ICodeElement, IScoped
    {
        IKey Key { get; }
    }

    public class TypeDefinition : ITypeDefinition
    {
        public TypeDefinition(IScope scope, IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IKey Key { get; }
        public IScope Scope { get; }
        
        public IBox<ITypeDefinition> ReturnType(IScope root)
        {
            if (scope.Root.TryGetType(RootScope.TypeType, out var res)) {
                return res;
            }
            throw new Exception("types types should be found");
        }
    }
    
    public class TypeDefinitionMaker : IMaker<TypeDefinition>
    {
        public TypeDefinitionMaker(Func<IScope,IKey, TypeDefinition> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<IScope, IKey, TypeDefinition> Make { get; }
        
        public IResult<IPopulateScope<TypeDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("type"), out var _)
                            .OptionalHas(ElementMatcher.IsName, out AtomicToken typeName)
                            .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                            .IsMatch)
            {
                var scope = new ObjectScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(body);

                
               return ResultExtension.Good(new TypeDefinitionPopulateScope(scope, elements, typeName != default ? new NameKey(typeName.Item).Cast<IKey>(): new ImplicitKey() , Make));
            }

            return ResultExtension.Bad<IPopulateScope<TypeDefinition>>(); ;
        }
    }
    
    public class TypeDefinitionPopulateScope : IPopulateScope<TypeDefinition>
    {
        private readonly ObjectScope scope;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly IKey key;
        private readonly Func<IScope, IKey, TypeDefinition> make;

        public TypeDefinitionPopulateScope(ObjectScope scope, IPopulateScope<ICodeElement>[] elements, IKey typeName, Func<IScope, IKey, TypeDefinition> make)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<TypeDefinition> Run(IPopulateScopeContext context)
        {
            var box = new Box<TypeDefinition>();
            var encolsing = context.Tree.Scopes(scope).Skip(1).First();
            encolsing.Cast<StaticScope>().TryAddStaticType(key, box);
            var nextContext = context.Child(this, scope);
            elements.Select(x => x.Run(nextContext)).ToArray();
            return new TypeDefinitionResolveReference(scope, box, make, key);
        }
    }

    public class TypeDefinitionResolveReference : IResolveReference<TypeDefinition>
    {
        private readonly ObjectScope scope;
        private readonly Box<TypeDefinition> box;
        private readonly IKey key;
        private readonly Func<IScope, IKey, TypeDefinition> make;

        public TypeDefinitionResolveReference(ObjectScope scope, Box<TypeDefinition> box, Func<IScope, IKey, TypeDefinition> make, IKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return box;
        }

        public TypeDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(scope, key));
        }
    }
}
