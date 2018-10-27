using Prototypist.LeftToRight;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{

    public class WeakObjectDefinition: IWeakCodeElement, IWeakReturnable, IScoped
    {
        public WeakObjectDefinition(IWeakFinalizedScope scope, IEnumerable<WeakAssignOperation> assigns, ImplicitKey key) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Assignments = assigns.ToArray();
        }

        public IWeakFinalizedScope Scope { get; }
        public WeakAssignOperation[] Assignments { get; }

        public IKey Key
        {
            get;
        }

        public IWeakReturnable Returns() {
            return this;
        }
    }
    
    public class ObjectDefinitionMaker : IMaker<WeakObjectDefinition>
    {
        public ObjectDefinitionMaker()
        {
        }

        public IResult<IPopulateScope<WeakObjectDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                           .Has(ElementMatcher.KeyWord("object"), out var keyword)
                           .Has(ElementMatcher.IsBody, out CurleyBracketToken block)
                           .Has(ElementMatcher.IsDone)
                           .IsMatch)
            {

                var elements = matchingContext.ParseBlock(block);
                
                return ResultExtension.Good(new ObjectDefinitionPopulateScope(elements));
            }
            return ResultExtension.Bad<IPopulateScope<WeakObjectDefinition>>();
        }
        
    }
    
    public class ObjectDefinitionPopulateScope : IPopulateScope<WeakObjectDefinition>
    {
        private readonly IPopulateScope<IWeakCodeElement>[] elements;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ObjectDefinitionPopulateScope(IPopulateScope<IWeakCodeElement>[] elements)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public IBox<IWeakReturnable> GetReturnType()
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

    public class ResolveReferanceObjectDefinition : IPopulateBoxes<WeakObjectDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IPopulateBoxes<IWeakCodeElement>[] elements;
        private readonly Box<IWeakReturnable> box;
        private readonly ImplicitKey key;

        public ResolveReferanceObjectDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<IWeakCodeElement>[] elements, 
            Box<IWeakReturnable> box, 
            ImplicitKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IOpenBoxes<WeakObjectDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(new WeakObjectDefinition(scope.GetFinalized(), elements.Select(x => x.Run(context).Cast<WeakAssignOperation>()).ToArray(), key));
            return new ObjectDefinitionOpenBoxes(item);
        }
    }

    internal class ObjectDefinitionOpenBoxes : IOpenBoxes< WeakObjectDefinition>
    {
        public WeakObjectDefinition CodeElement { get; }

        public ObjectDefinitionOpenBoxes(WeakObjectDefinition item)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return context.ObjectDefinition(CodeElement);
        }
    }
}
