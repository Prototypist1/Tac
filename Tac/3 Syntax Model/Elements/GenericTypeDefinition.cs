using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    //internal interface IFinalizedScopeTemplate
    //{
    //    IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    //    IFinalizedScope CreateScope(GenericTypeParameter[] parameters);
    //}

    //internal static class FinalizedScopeTemplateExtensions {
    //    public static bool Accepts(this IFinalizedScopeTemplate self, GenericTypeParameter[] parameters) {
    //        return parameters.Select(x => x.Parameter).SetEqual(self.TypeParameterDefinitions);
    //    }
    //}

    internal class WeakGenericTypeDefinition : WeakTypeDefinition, IGenericType
    {
        public WeakGenericTypeDefinition(NameKey key, IFinalizedScope scope, IGenericTypeParameterDefinition[] TypeParameterDefinitions):base(scope,key)
        {
            this.TypeParameterDefinitions = TypeParameterDefinitions ?? throw new ArgumentNullException(nameof(TypeParameterDefinitions));
        }

        public IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
        
        //public IVarifiableType GetConcreteType(GenericTypeParameter[] parameters)
        //{

        //    // TOOD populate?
        //    // overlay?
        //    // fill boxes? -- you can't fill the boxes unless you copy everything
            
        //    // the scope already holds all the members
        //    // just the types are not populated
        //    // 

        //    // man generics are killer 
            
        //    // clearly a scope intention,
        //    // not a scope
        //    // that is a problem for existing code
            
        //    // is it?
        //    // method are a scope intention too
        //    // do method work in parallel? or do they trip all over each other?
            
        //    // no they take a scope template 
        //    // this clearly should feature a scope template too

        //    // now the problem is members
        //    // they add them selve to the current scope
        //    // there is no reason they should not just add them selves to the template 

        //    return new WeakTypeDefinition(Scope.CreateScope(parameters), Key); ;
        //}

        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.GenericTypeDefinition(this);
        }

        public override IVarifiableType Returns()
        {
            return this;
        }
    }


    internal class GenericTypeParameterDefinition: IGenericTypeParameterDefinition
    {
        public GenericTypeParameterDefinition(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IKey Key
        {
            get
            {
                return new NameKey(Name);
            }
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is GenericTypeParameterDefinition definition &&
                   Name == definition.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        internal bool Accepts(IVarifiableType b)
        {
            // TODO generic constraints
            return true;
        }
    }
    
    internal class GenericTypeDefinitionMaker : IMaker<IPopulateScope<WeakGenericTypeDefinition>>
    {

        public GenericTypeDefinitionMaker()
        {
        }

        public ITokenMatching<IPopulateScope<WeakGenericTypeDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .Has(new DefineGenericNMaker(), out AtomicToken[] genericTypes)
                .Has(new NameMaker(), out AtomicToken typeName)
                .Has(new BodyMaker(), out CurleyBracketToken body);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakGenericTypeDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new GenericTypeDefinitionPopulateScope(
                        new NameKey(typeName.Item),
                        tokenMatching.Context.ParseBlock(body),
                        genericTypes.Select(x => 
                        new GenericTypeParameterDefinition(x.Item)).ToArray()));
            }

            return TokenMatching<IPopulateScope<WeakGenericTypeDefinition>>.MakeNotMatch(
                    matching.Context);
        }



    }

    internal class GenericTypeDefinitionPopulateScope : IPopulateScope<WeakGenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly IEnumerable<IPopulateScope<ICodeElement>> lines;
        private readonly IGenericTypeParameterDefinition[] genericParameters;
        private readonly Box<IVarifiableType> box = new Box<IVarifiableType>();

        public GenericTypeDefinitionPopulateScope(
            NameKey nameKey, 
            IEnumerable<IPopulateScope<ICodeElement>> lines,
            IGenericTypeParameterDefinition[] genericParameters)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
        }

        public IPopulateBoxes<WeakGenericTypeDefinition> Run(IPopulateScopeContext context)
        {
            var encolsing = context.Scope.TryAddType(nameKey, box);
            
            var nextContext = context.TemplateChild(genericParameters);
            lines.Select(x => x.Run(nextContext)).ToArray();
            return new GenericTypeDefinitionResolveReferance(nameKey, nextContext.GetResolvableScope(), box, genericParameters);
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }
    }

    internal class GenericTypeDefinitionResolveReferance : IPopulateBoxes<WeakGenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly IResolvableScope scope;
        private readonly Box<IVarifiableType> box;
        private readonly IGenericTypeParameterDefinition[] genericParameters;

        public GenericTypeDefinitionResolveReferance(
            NameKey nameKey, 
            IResolvableScope scope, 
            Box<IVarifiableType> box,
            IGenericTypeParameterDefinition[] genericParameters)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
        }
        
        public WeakGenericTypeDefinition Run(IResolveReferanceContext context)
        {
            // hmm getting the template down here is hard
            // scope mostly comes from context
            // why is that?
            return box.Fill(new WeakGenericTypeDefinition(nameKey, scope.GetFinalized(), genericParameters));
        }
    }
}
