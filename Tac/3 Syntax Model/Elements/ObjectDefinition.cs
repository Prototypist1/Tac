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
    public class ObjectDefinition: ITypeDefinition, ICodeElement, IReturnable
    {
        public ObjectDefinition(IResolvableScope scope, IEnumerable<AssignOperation> assigns, ImplicitKey key) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Assignments = assigns.ToArray();
        }

        public IResolvableScope Scope { get; }
        public AssignOperation[] Assignments { get; }

        public IKey Key
        {
            get;
        }

        public IReturnable ReturnType(RootScope rootScope) {
            return this;
        }
    }
    
    public class ObjectDefinitionMaker : IMaker<ObjectDefinition>
    {
        public ObjectDefinitionMaker(Func<IResolvableScope, IEnumerable<AssignOperation>, ImplicitKey, ObjectDefinition> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<IResolvableScope, IEnumerable<AssignOperation>, ImplicitKey, ObjectDefinition> Make { get; }

        public IResult<IPopulateScope<ObjectDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                           .Has(ElementMatcher.KeyWord("object"), out var keyword)
                           .Has(ElementMatcher.IsBody, out CurleyBracketToken block)
                           .Has(ElementMatcher.IsDone)
                           .IsMatch)
            {
                var scope = Scope.LocalStaticScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(block);
                
                return ResultExtension.Good(new ObjectDefinitionPopulateScope(scope, elements, Make));
            }
            return ResultExtension.Bad<IPopulateScope<ObjectDefinition>>();
        }
        
    }
    
    public class ObjectDefinitionPopulateScope : IPopulateScope<ObjectDefinition>
    {
        private readonly ILocalStaticScope scope;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly Func<IResolvableScope, IEnumerable<AssignOperation>, ImplicitKey, ObjectDefinition> make;

        public ObjectDefinitionPopulateScope(ILocalStaticScope scope, IPopulateScope<ICodeElement>[] elements, Func<IResolvableScope, IEnumerable<AssignOperation>, ImplicitKey, ObjectDefinition> make)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<ObjectDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child(this, scope);
            var box = new Box<ITypeDefinition>();
            var key = new ImplicitKey();
            scope.TryAddStaticType(key, box);
            return new ResolveReferanceObjectDefinition(scope.ToResolvable(), elements.Select(x => x.Run(nextContext)).ToArray(), make, box,key);
        }
    }

    public class ResolveReferanceObjectDefinition : IResolveReference<ObjectDefinition>
    {
        private readonly IResolvableScope scope;
        private readonly IResolveReference<ICodeElement>[] elements;
        private readonly Func<IResolvableScope, IEnumerable<AssignOperation>, ImplicitKey, ObjectDefinition> make;
        private readonly Box<ITypeDefinition> box;
        private readonly ImplicitKey key;

        public ResolveReferanceObjectDefinition(IResolvableScope scope, IResolveReference<ICodeElement>[] elements, Func<IResolvableScope, IEnumerable<AssignOperation>, ImplicitKey, ObjectDefinition> make, Box<ITypeDefinition> box, ImplicitKey key)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public ObjectDefinition Run(IResolveReferanceContext context)
        {
            var nextContext = context.Child(this, scope);
            return box.Fill(make(scope, elements.Select(x => x.Run(nextContext).Cast<AssignOperation>()).ToArray(), key));
        }
        
        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return box;
        }
    }
}
