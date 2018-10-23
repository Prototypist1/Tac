﻿using Prototypist.LeftToRight;
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
    public class ObjectDefinition: ICodeElement, IReturnable, IScoped
    {
        public delegate ObjectDefinition Make(IFinalizedScope scope, IEnumerable<AssignOperation> assigns, ImplicitKey key);

        public ObjectDefinition(IFinalizedScope scope, IEnumerable<AssignOperation> assigns, ImplicitKey key) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Assignments = assigns.ToArray();
        }

        public IFinalizedScope Scope { get; }
        public AssignOperation[] Assignments { get; }

        public IKey Key
        {
            get;
        }

        public IReturnable Returns(IElementBuilders elementBuilders) {
            return this;
        }
    }
    
    public class ObjectDefinitionMaker : IMaker<ObjectDefinition>
    {
        public ObjectDefinitionMaker(ObjectDefinition.Make make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private ObjectDefinition.Make Make { get; }

        public IResult<IPopulateScope<ObjectDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
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
            return ResultExtension.Bad<IPopulateScope<ObjectDefinition>>();
        }
        
    }
    
    public class ObjectDefinitionPopulateScope : IPopulateScope<ObjectDefinition>
    {
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly ObjectDefinition.Make make;
        private readonly Box<IReturnable> box = new Box<IReturnable>();

        public ObjectDefinitionPopulateScope(IPopulateScope<ICodeElement>[] elements, ObjectDefinition.Make make)
        {
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<ObjectDefinition> Run(IPopulateScopeContext context)
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

    public class ResolveReferanceObjectDefinition : IResolveReference<ObjectDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IResolveReference<ICodeElement>[] elements;
        private readonly ObjectDefinition.Make make;
        private readonly Box<IReturnable> box;
        private readonly ImplicitKey key;

        public ResolveReferanceObjectDefinition(
            IResolvableScope scope, 
            IResolveReference<ICodeElement>[] elements, 
            ObjectDefinition.Make make, 
            Box<IReturnable> box, 
            ImplicitKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public ObjectDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(scope.GetFinalized(), elements.Select(x => x.Run(context).Cast<AssignOperation>()).ToArray(), key));
        }
    }
}
