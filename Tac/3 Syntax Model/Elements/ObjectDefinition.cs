using Prototypist.LeftToRight;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{

    internal class WeakObjectDefinition: ICodeElement, IObjectType, IScoped, IObjectDefiniton
    {
        public WeakObjectDefinition(IFinalizedScope scope, IEnumerable<WeakAssignOperation> assigns, ImplicitKey key) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Assignments = assigns.ToArray();
        }

        public IFinalizedScope Scope { get; }
        public WeakAssignOperation[] Assignments { get; }

        public IKey Key
        {
            get;
        }

        #region IObjectDefiniton

        IFinalizedScope IObjectDefiniton.Scope => Scope;
        IEnumerable<IAssignOperation> IObjectDefiniton.Assignments => Assignments;

        #endregion
        
        public IVarifiableType Returns() {
            return this;
        }
        
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ObjectDefinition(this);
        }

    }

    internal class ObjectDefinitionMaker : IMaker<IPopulateScope<WeakObjectDefinition>>
    {
        public ObjectDefinitionMaker()
        {
        }

        public ITokenMatching<IPopulateScope<WeakObjectDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("object"), out var keyword)
                .Has(new BodyMaker(), out var block);
            if (matching is IMatchedTokenMatching matched)
            {

                var elements = tokenMatching.Context.ParseBlock(block);
                
                return TokenMatching<IPopulateScope<WeakObjectDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new ObjectDefinitionPopulateScope(elements));
            }
            return TokenMatching<IPopulateScope<WeakObjectDefinition>>.MakeNotMatch(
                    matching.Context);
        }
    }

    internal class ObjectDefinitionPopulateScope : IPopulateScope<WeakObjectDefinition>
    {
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly Box<IVarifiableType> box = new Box<IVarifiableType>();

        public ObjectDefinitionPopulateScope(IPopulateScope<ICodeElement>[] elements)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakObjectDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            var key = new ImplicitKey();
            nextContext.Scope.TryAddType(key, box);
            return new ResolveReferanceObjectDefinition(
                nextContext.GetResolvableScope(),
                elements.Select(x => x.Run(nextContext)).ToArray(),
                box,
                key);
        }
    }

    internal class ResolveReferanceObjectDefinition : IPopulateBoxes<WeakObjectDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IPopulateBoxes<ICodeElement>[] elements;
        private readonly Box<IVarifiableType> box;
        private readonly ImplicitKey key;

        public ResolveReferanceObjectDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<ICodeElement>[] elements, 
            Box<IVarifiableType> box, 
            ImplicitKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public WeakObjectDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(new WeakObjectDefinition(scope.GetFinalized(), elements.Select(x => x.Run(context).Cast<WeakAssignOperation>()).ToArray(), key));
        }
    }
}
