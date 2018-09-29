using Prototypist.LeftToRight;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public class ObjectDefinition: ITypeDefinition, ICodeElement
    {
        public ObjectDefinition(ObjectScope scope, IEnumerable<AssignOperation> assigns) {
            Scope = scope;
            Assignments = assigns.ToArray();
        }

        public IScope Scope { get; }
        public AssignOperation[] Assignments { get; }
        
        public IBox<ITypeDefinition> ReturnType(ScopeTree scope) {
            return new Box<ITypeDefinition>(this);
        }
    }
    
    public class ObjectDefinitionMaker : IMaker<ObjectDefinition>
    {
        public ObjectDefinitionMaker(Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> Make { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<ObjectDefinition> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                           .Has(ElementMatcher.KeyWord("object"), out var keyword)
                           .Has(ElementMatcher.IsBody, out CurleyBacketToken block)
                           .Has(ElementMatcher.IsDone)
                           .IsMatch)
            {
                var scope = new ObjectScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(block);

                result = new ObjectDefinitionPopulateScope(scope, elements, Make);

                return true;
            }
            result = default;
            return false;
        }
        
    }
    
    public class ObjectDefinitionPopulateScope : IPopulateScope<ObjectDefinition>
    {
        private readonly ObjectScope scope;
        private readonly IPopulateScope<ICodeElement>[] elements;
        private readonly Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> make;

        public ObjectDefinitionPopulateScope(ObjectScope scope, IPopulateScope<ICodeElement>[] elements, Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> make)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReferance<ObjectDefinition> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child(this, scope);
            return new ResolveReferanceObjectDefinition(scope, elements.Select(x => x.Run(nextContext)).ToArray(), make);
        }

        public IResolveReferance<ObjectDefinition> Run()
        {
            throw new NotImplementedException();
        }
    }

    public class ResolveReferanceObjectDefinition : IResolveReferance<ObjectDefinition>
    {
        private readonly Box<ITypeDefinition> type = new Box<ITypeDefinition>();
        private readonly ObjectScope scope;
        private readonly IResolveReferance<ICodeElement>[] elements;
        private readonly Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> make;

        public ResolveReferanceObjectDefinition(ObjectScope scope, IResolveReferance<ICodeElement>[] elements, Func<IScope, IEnumerable<AssignOperation>, ObjectDefinition> make)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public ObjectDefinition Run(IResolveReferanceContext context)
        {
            var nextContext = context.Child(this, scope);
            return type.Fill(make(scope, elements.Select(x => x.Run(nextContext).Cast<AssignOperation>()).ToArray()));
        }
        
        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return type;
        }
    }
}
