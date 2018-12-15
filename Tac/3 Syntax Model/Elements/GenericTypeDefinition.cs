using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model
{
    
    internal class WeakGenericTypeDefinition : IFrontendCodeElement<IGenericInterfaceDefinition>, IScoped, IFrontendType
    {
        public WeakGenericTypeDefinition(
            IIsPossibly<NameKey> key, 
            IFinalizedScope scope,
            IIsPossibly<IGenericTypeParameterDefinition>[] TypeParameterDefinitions)
        {
            this.TypeParameterDefinitions = TypeParameterDefinitions ?? throw new ArgumentNullException(nameof(TypeParameterDefinitions));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IIsPossibly<IGenericTypeParameterDefinition>[] TypeParameterDefinitions { get; }
        public IIsPossibly<IKey> Key { get; }
        public IFinalizedScope Scope { get; }

        public IBuildIntention<IGenericInterfaceDefinition> GetBuildIntention(ConversionContext context)
        {
            var (toBuild, maker) = GenericInterfaceDefinition.Create();
            return new BuildIntention<IGenericInterfaceDefinition>(toBuild, () =>
            {
                maker.Build(Scope,
                    TypeParameterDefinitions.Select(x=>x.GetOrThrow()).ToArray());
            });
        }

        IIsPossibly<IFrontendType> IFrontendCodeElement<IGenericInterfaceDefinition>.Returns()
        {
            return Possibly.Is(this);
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
                .Has(new DefineGenericNMaker(), out var genericTypes)
                .Has(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakGenericTypeDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new GenericTypeDefinitionPopulateScope(
                        new NameKey(typeName.Item),
                        tokenMatching.Context.ParseBlock(body),
                        genericTypes.Select(x => 
                        new GenericTypeParameterDefinition(x)).ToArray()));
            }

            return TokenMatching<IPopulateScope<WeakGenericTypeDefinition>>.MakeNotMatch(
                    matching.Context);
        }



    }

    internal class GenericTypeDefinitionPopulateScope : IPopulateScope<WeakGenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly IEnumerable<IPopulateScope<IFrontendCodeElement< ICodeElement>>> lines;
        private readonly IGenericTypeParameterDefinition[] genericParameters;
        private readonly Box<IIsPossibly<IFrontendType>> box = new Box<IIsPossibly<IFrontendType>>();

        public GenericTypeDefinitionPopulateScope(
            NameKey nameKey, 
            IEnumerable<IPopulateScope<IFrontendCodeElement<ICodeElement>>> lines,
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

        public IBox<IIsPossibly<IFrontendType>> GetReturnType()
        {
            return box;
        }
    }

    internal class GenericTypeDefinitionResolveReferance : IPopulateBoxes<WeakGenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly IResolvableScope scope;
        private readonly Box<IIsPossibly<IFrontendType>> box;
        private readonly IGenericTypeParameterDefinition[] genericParameters;

        public GenericTypeDefinitionResolveReferance(
            NameKey nameKey, 
            IResolvableScope scope, 
            Box<IIsPossibly<IFrontendType>> box,
            IGenericTypeParameterDefinition[] genericParameters)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
        }
        
        public IIsPossibly<WeakGenericTypeDefinition> Run(IResolveReferenceContext context)
        {
            // hmm getting the template down here is hard
            // scope mostly comes from context
            // why is that?
            return box.Fill(Possibly.Is(new WeakGenericTypeDefinition(
                Possibly.Is(nameKey),
                scope.GetFinalized(),
                genericParameters.Select(x=>Possibly.Is(x)).ToArray())));
        }
    }
}
