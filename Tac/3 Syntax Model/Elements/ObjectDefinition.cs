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
    public interface IObjectDefiniton : ICodeElement, IReturnable
    {
        IFinalizedScope Scope { get; }
        IEnumerable<IAssignOperation> Assignments { get; }
        // why does this know it own key?
        IKey Key { get; }
    }

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

        public IWeakReturnable Returns(IElementBuilders elementBuilders) {
            return this;
        }
    }
    
    public class ObjectDefinitionMaker<T> : IMaker<T, WeakObjectDefinition>
    {
        public ObjectDefinitionMaker(Func<WeakObjectDefinition,T> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<WeakObjectDefinition,T> Make { get; }

        public IResult<IPopulateScope<T, WeakObjectDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                           .Has(ElementMatcher.KeyWord("object"), out var keyword)
                           .Has(ElementMatcher.IsBody, out CurleyBracketToken block)
                           .Has(ElementMatcher.IsDone)
                           .IsMatch)
            {

                var elements = matchingContext.ParseBlock(block);
                
                return ResultExtension.Good(new ObjectDefinitionPopulateScope<T>(elements, Make));
            }
            return ResultExtension.Bad<IPopulateScope<T, WeakObjectDefinition>>();
        }
        
    }
    
    public class ObjectDefinitionPopulateScope<T> : IPopulateScope<T, WeakObjectDefinition>
    {
        private readonly IPopulateScope<,IWeakCodeElement>[] elements;
        private readonly Func<WeakObjectDefinition,T> make;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ObjectDefinitionPopulateScope(IPopulateScope<,IWeakCodeElement>[] elements, Func<WeakObjectDefinition,T> make)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<T, WeakObjectDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child();
            var key = new ImplicitKey();
            nextContext.Scope.TryAddType(key, box);
            return new ResolveReferanceObjectDefinition<T>(
                nextContext.GetResolvableScope(),
                elements.Select(x => x.Run(nextContext)).ToArray(),
                make, 
                box,
                key);
        }
    }

    public class ResolveReferanceObjectDefinition<T> : IPopulateBoxes<T, WeakObjectDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IPopulateBoxes<,IWeakCodeElement>[] elements;
        private readonly Func<WeakObjectDefinition,T> make;
        private readonly Box<IWeakReturnable> box;
        private readonly ImplicitKey key;

        public ResolveReferanceObjectDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<,IWeakCodeElement>[] elements, 
            Func<WeakObjectDefinition,T> make, 
            Box<IWeakReturnable> box, 
            ImplicitKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IOpenBoxes<T, WeakObjectDefinition> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(new WeakObjectDefinition(scope.GetFinalized(), elements.Select(x => x.Run(context).Cast<WeakAssignOperation>()).ToArray(), key));
            return new ObjectDefinitionOpenBoxes<T>(item, make);
        }
    }

    internal class ObjectDefinitionOpenBoxes<T> : IOpenBoxes<T, WeakObjectDefinition>
    {
        public WeakObjectDefinition CodeElement { get; }
        private readonly Func<WeakObjectDefinition, T> make;

        public ObjectDefinitionOpenBoxes(WeakObjectDefinition item, Func<WeakObjectDefinition, T> make)
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
