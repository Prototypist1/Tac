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
        public delegate WeakObjectDefinition Make(IWeakFinalizedScope scope, IEnumerable<WeakAssignOperation> assigns, ImplicitKey key);

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
    
    public class ObjectDefinitionMaker : IMaker<WeakObjectDefinition>
    {
        public ObjectDefinitionMaker(WeakObjectDefinition.Make make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private WeakObjectDefinition.Make Make { get; }

        public IResult<IPopulateScope<WeakObjectDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                           .Has(ElementMatcher.KeyWord("object"), out var keyword)
                           .Has(ElementMatcher.IsBody, out CurleyBracketToken block)
                           .Has(ElementMatcher.IsDone)
                           .IsMatch)
            {

                var elements = matchingContext.ParseBlock(block);
                
                return ResultExtension.Good(new ObjectDefinitionPopulateScope(elements, Make));
            }
            return ResultExtension.Bad<IPopulateScope<WeakObjectDefinition>>();
        }
        
    }
    
    public class ObjectDefinitionPopulateScope : IPopulateScope<WeakObjectDefinition>
    {
        private readonly IPopulateScope<IWeakCodeElement>[] elements;
        private readonly WeakObjectDefinition.Make make;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ObjectDefinitionPopulateScope(IPopulateScope<IWeakCodeElement>[] elements, WeakObjectDefinition.Make make)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
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
                make, 
                box,
                key);
        }
    }

    public class ResolveReferanceObjectDefinition : IPopulateBoxes<WeakObjectDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IPopulateBoxes<IWeakCodeElement>[] elements;
        private readonly WeakObjectDefinition.Make make;
        private readonly Box<IWeakReturnable> box;
        private readonly ImplicitKey key;

        public ResolveReferanceObjectDefinition(
            IResolvableScope scope, 
            IPopulateBoxes<IWeakCodeElement>[] elements, 
            WeakObjectDefinition.Make make, 
            Box<IWeakReturnable> box, 
            ImplicitKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public WeakObjectDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(scope.GetFinalized(), elements.Select(x => x.Run(context).Cast<WeakAssignOperation>()).ToArray(), key));
        }
    }
}
