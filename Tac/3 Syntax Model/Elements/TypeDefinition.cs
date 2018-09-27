using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public interface ITypeDefinition : ICodeElement, IScoped
    {

    }

    public class TypeDefinition : ITypeDefinition
    {
        public TypeDefinition(IScope scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IScope Scope { get; }
        
        public IBox<ITypeDefinition> ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.TypeType);
        }
    }

    public class NamedTypeDefinition : TypeDefinition
    {
        public NamedTypeDefinition(IKey key, IScope scope) : base(scope)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; }

    }

    public class TypeDefinitionMaker : IMaker<TypeDefinition>
    {
        public TypeDefinitionMaker(Func<IScope, TypeDefinition> make, Func<IScope,string, NamedTypeDefinition> makeWithName)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
            MakeWithName = makeWithName ?? throw new ArgumentNullException(nameof(makeWithName));
        }

        private Func<IScope, TypeDefinition> Make { get; }
        private Func<IScope, string, NamedTypeDefinition> MakeWithName { get; }

        private TypeDefinition Create(IScope scope, string name) {
            if (name == default)
            {
                return Make(scope);
            }
            else {
                return MakeWithName(scope, name);
            }
        }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<TypeDefinition> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("type"), out var _)
                            .OptionalHas(ElementMatcher.IsName, out AtomicToken typeName)
                            .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                            .IsMatch)
            {
                var scope = new ObjectScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(body);


                if (typeName != default)
                {
                    result = new TypeDefinitionPopulateScope(scope, elements, typeName,(s) => MakeWithName(s, typeName.Item));
                }
                else
                {
                    result = new TypeDefinitionPopulateScope(scope, elements, typeName, (s) => MakeWithName(s, typeName.Item));
                }

                return true;
            }

            result = default;
            return false;
        }

        private class TypeDefinitionPopulateScope : IPopulateScope<TypeDefinition>
        {
            private readonly ObjectScope scope;
            private readonly IPopulateScope<ICodeElement>[] elements;
            private readonly AtomicToken typeName;
            private readonly Func<IScope, TypeDefinition> make;

            public TypeDefinitionPopulateScope(ObjectScope scope, IPopulateScope<ICodeElement>[] elements, AtomicToken typeName, Func<IScope, TypeDefinition> make)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.typeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IResolveReferance<TypeDefinition> Run(IPopulateScopeContext context)
            {
                var box = new Box<TypeDefinition>();
                if (typeName != null)
                {
                    var encolsing = context.Tree.Scopes(scope).Skip(1).First();
                    encolsing.Cast<StaticScope>().TryAddStaticType(new NameKey(typeName.Item), box);
                }
                var nextContext = context.Child(this, scope);
                elements.Select(x => x.Run(nextContext)).ToArray();
                return new TypeDefinitionResolveReferance(scope, box,make);
            }

            public IResolveReferance<TypeDefinition> Run()
            {
                throw new NotImplementedException();
            }
        }

        private class TypeDefinitionResolveReferance : IResolveReferance<TypeDefinition>
        {
            private readonly ObjectScope scope;
            private readonly Box<TypeDefinition> box;
            private readonly Func<IScope, TypeDefinition> make;

            public TypeDefinitionResolveReferance(ObjectScope scope, Box<TypeDefinition> box, Func<IScope, TypeDefinition> make)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public TypeDefinition Run(IResolveReferanceContext context)
            {
                return box.Fill(make(scope));
            }
        }
        
    }


}
