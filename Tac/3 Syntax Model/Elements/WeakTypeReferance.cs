using System;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    internal class WeakTypeReferance : ICodeElement, ITypeReferance
    {
        public WeakTypeReferance(IBox<IVarifiableType> typeDefinition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
        }

        public IBox<IVarifiableType> TypeDefinition { get; }

        #region ITypeReferance

        IVarifiableType ITypeReferance.TypeDefinition => TypeDefinition.GetValue();

        #endregion


        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.TypeReferance(this);
        }

        public IVarifiableType Returns()
        {
            return TypeDefinition.GetValue();
        }
    }
    
    internal class TypeReferanceMaker : IMaker<IPopulateScope<WeakTypeReferance>>
    {
        public TypeReferanceMaker()
        {
        }

        public ITokenMatching<IPopulateScope<WeakTypeReferance>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var typeName);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakTypeReferance>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TypeReferancePopulateScope(new NameKey(typeName.Item)));
            }

            return TokenMatching<IPopulateScope<WeakTypeReferance>>.MakeNotMatch(matching.Context);
        }
    }

    internal class TypeReferancePopulateScope : IPopulateScope<WeakTypeReferance>
    {
        private readonly IKey key;
        private readonly Box<WeakTypeReferance> box = new Box<WeakTypeReferance>();

        public TypeReferancePopulateScope(IKey typeName)
        {
            key = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakTypeReferance> Run(IPopulateScopeContext context)
        {
            return new TypeReferanceResolveReference(
                context.GetResolvableScope(),
                box,
                key);
        }
    }

    internal class TypeReferanceResolveReference : IPopulateBoxes<WeakTypeReferance>
    {
        private readonly IResolvableScope scope;
        private readonly Box<WeakTypeReferance> box;
        private readonly IKey key;

        public TypeReferanceResolveReference(IResolvableScope scope, Box<WeakTypeReferance> box, IKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public WeakTypeReferance Run(IResolveReferanceContext context)
        {
            return box.Fill(new WeakTypeReferance(scope.GetTypeOrThrow(key)));
        }
    }

}
