using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    internal class WeakTypeDefinition : IInterfaceType, ICodeElement, IScoped
    {
        public WeakTypeDefinition(IFinalizedScope scope, IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IKey Key { get; }
        public IFinalizedScope Scope { get; }
        
        
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.TypeDefinition(this);
        }
        
        public IVarifiableType Returns()
        {
            return this;
        }
    }


    internal class TypeDefinitionMaker : IMaker<IPopulateScope<WeakTypeReferance>>
    {
        public TypeDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakTypeReferance>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .OptionalHas(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);

            if (matching is IMatchedTokenMatching matched)
            {
               var elements = tokenMatching.Context.ParseBlock(body);
                
               return TokenMatching<IPopulateScope<WeakTypeReferance>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TypeDefinitionPopulateScope(
                       elements, 
                       typeName != default ? new NameKey(typeName.Item).Cast<IKey>(): new ImplicitKey()));
            }

            return TokenMatching<IPopulateScope<WeakTypeReferance>>.MakeNotMatch(
                    matching.Context);
        }
    }

    internal class TypeDefinitionPopulateScope : IPopulateScope<WeakTypeReferance>
    {
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly IKey key;
        private readonly Box<IInterfaceType> definitionBox = new Box<IInterfaceType>();
        private readonly WeakTypeReferance typeReferance;
        private readonly Box<WeakTypeReferance> box;

        public TypeDefinitionPopulateScope(IPopulateScope<ICodeElement>[] elements, IKey typeName)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            typeReferance = new WeakTypeReferance(definitionBox);
            box = new Box<WeakTypeReferance>(typeReferance);
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakTypeReferance> Run(IPopulateScopeContext context)
        {
            var encolsing = context.Scope.TryAddType(key, box);
            var nextContext = context.Child();
            elements.Select(x => x.Run(nextContext)).ToArray();
            return new TypeDefinitionResolveReference(
                nextContext.GetResolvableScope(),
                definitionBox,
                typeReferance,
                key);
        }
    }

    internal class TypeDefinitionResolveReference : IPopulateBoxes<WeakTypeReferance>
    {
        private readonly IResolvableScope scope;
        private readonly Box<IInterfaceType> definitionBox;
        private readonly WeakTypeReferance typeReferance;
        private readonly IKey key;

        public TypeDefinitionResolveReference(IResolvableScope scope, Box<IInterfaceType> definitionBox, WeakTypeReferance typeReferance, IKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.definitionBox = definitionBox ?? throw new ArgumentNullException(nameof(definitionBox));
            this.typeReferance = typeReferance ?? throw new ArgumentNullException(nameof(typeReferance));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public WeakTypeReferance Run(IResolveReferanceContext context)
        {
            definitionBox.Fill(new WeakTypeDefinition(scope.GetFinalized(), key));
            return typeReferance;
        }
    }
}
