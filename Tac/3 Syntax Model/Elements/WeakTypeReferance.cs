using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    internal class WeakTypeReferance : IFrontendCodeElement, ITypeReferance
    {
        public WeakTypeReferance(IIsPossibly<IBox<IIsPossibly<IVarifiableType>>> typeDefinition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
        }

        public IIsPossibly<IBox<IIsPossibly<IVarifiableType>>> TypeDefinition { get; }

        #region ITypeReferance

        IVarifiableType ITypeReferance.TypeDefinition => TypeDefinition.IfIs(x=>x.GetValue()).GetOrThrow();

        #endregion


        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.TypeReferance(this);
        }

        public IIsPossibly<IVarifiableType> Returns()
        {
            return TypeDefinition.IfIs(x => x.GetValue());
        }

        IVarifiableType ICodeElement.Returns()
        {
            return Returns().GetOrThrow();
        }
    }

    internal class KeyMatcher : IMaker<IKey>
    {
        public ITokenMatching<IKey> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var matching = tokenMatching
                .Has(new NameMaker(), out var typeName);
            
            var list = new List<IKey>();
            var genericMatachig = matching
                .HasSquare(x => {
                    while (true)
                    {
                        // colin, why! w x y z
                        // you are an adult arn'y you?
                        var item = default(IKey);
                        var y = x.HasLine(z => z.Has(new KeyMatcher(), out item));
                        if (y is IMatchedTokenMatching w)
                        {
                            x = w;
                            list.Add(item);
                            if (w.Tokens.Any().Not()) {
                                return w;
                            }
                        }
                        else
                        {
                            return y;
                        }
                    }
                });

            if (genericMatachig is IMatchedTokenMatching genericMatched)
            {
                return TokenMatching<IKey>.MakeMatch(genericMatched.Tokens,genericMatched.Context, new GenericNameKey(new NameKey(typeName.Item),list.ToArray()));
            }

            if (matching is IMatchedTokenMatching matched) {
                return TokenMatching<IKey>.MakeMatch(matched.Tokens, matched.Context, new NameKey(typeName.Item));
            }

            return TokenMatching<IKey>.MakeNotMatch(matching.Context);
        }
    }

    internal class TypeReferanceMaker : IMaker<IPopulateScope<WeakTypeReferance>>
    {
        public ITokenMatching<IPopulateScope<WeakTypeReferance>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var list = new List<IPopulateScope<WeakTypeReferance>>();
            var matching = tokenMatching
                .Has(new TypeMaker(), out var type);
            
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakTypeReferance>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new TypeReferancePopulateScope(type));
            }

            return TokenMatching<IPopulateScope<WeakTypeReferance>>.MakeNotMatch(matching.Context);
        }
    }

    internal class TypeReferancePopulateScope : IPopulateScope<WeakTypeReferance>
    {
        private readonly IKey key;
        private readonly Box<IIsPossibly<WeakTypeReferance>> box = new Box<IIsPossibly<WeakTypeReferance>>();

        public TypeReferancePopulateScope(IKey typeName)
        {
            key = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public IBox<IIsPossibly<IVarifiableType>> GetReturnType()
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
        private readonly Box<IIsPossibly<WeakTypeReferance>> box;
        private readonly IKey key;

        public TypeReferanceResolveReference(IResolvableScope scope, Box<IIsPossibly<WeakTypeReferance>> box, IKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IIsPossibly<WeakTypeReferance> Run(IResolveReferenceContext context)
        {
                return box.Fill(Possibly.Is(new WeakTypeReferance(scope.PossiblyGetType(key))));
        }
    }

}
