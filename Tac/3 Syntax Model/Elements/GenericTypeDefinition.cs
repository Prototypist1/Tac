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
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model
{

    internal class OverlayGenericTypeDefinition: IWeakGenericTypeDefinition
    {
        private readonly IWeakGenericTypeDefinition backing;

        public OverlayGenericTypeDefinition(IWeakGenericTypeDefinition backing, Overlay overlay)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            Scope = new OverlayedScope(backing.Scope, overlay);
        }
        
        public IResolvableScope Scope { get; }

        public IIsPossibly<IKey> Key => backing.Key;
        public IIsPossibly<Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder>[] TypeParameterDefinitions=> backing.TypeParameterDefinitions;
        public IBuildIntention<IGenericInterfaceDefinition> GetBuildIntention(ConversionContext context) => backing.Cast<IFrontendCodeElement<IGenericInterfaceDefinition>>().GetBuildIntention(context);
        public OrType<IFrontendGenericType, IFrontendType<IVarifiableType>> Overlay(TypeParameter[] typeParameters) => backing.Overlay(typeParameters);
        IBuildIntention<IVarifiableType> IConvertable<IVarifiableType>.GetBuildIntention(ConversionContext context) => backing.Cast<IFrontendType<IVarifiableType>>().GetBuildIntention(context); 
        IIsPossibly<IFrontendType<IVarifiableType>> IFrontendCodeElement<IGenericInterfaceDefinition>.Returns() => Possibly.Is(this);
    }

    internal interface IWeakGenericTypeDefinition: IFrontendCodeElement<IGenericInterfaceDefinition>, IScoped, IFrontendType<IVarifiableType>, IFrontendGenericType
    {
        IIsPossibly<IKey> Key { get; }
    }

    internal class WeakGenericTypeDefinition : IWeakGenericTypeDefinition
    {
        public WeakGenericTypeDefinition(
            IIsPossibly<NameKey> key,
            IResolvableScope scope,
            IIsPossibly<Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder>[] TypeParameterDefinitions)
        {
            this.TypeParameterDefinitions = TypeParameterDefinitions ?? throw new ArgumentNullException(nameof(TypeParameterDefinitions));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IIsPossibly<Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
        public IIsPossibly<IKey> Key { get; }
        public IResolvableScope Scope { get; }

        public IBuildIntention<IGenericInterfaceDefinition> GetBuildIntention(ConversionContext context)
        {
            var (toBuild, maker) = GenericInterfaceDefinition.Create();
            return new BuildIntention<IGenericInterfaceDefinition>(toBuild, () =>
            {
                maker.Build(
                    Scope.Convert(context),
                    TypeParameterDefinitions.Select(x=>new Tac.Model.Elements.GenericTypeParameterDefinition(x.GetOrThrow().Key).Cast<IGenericTypeParameterDefinition>()).ToArray());
            });
        }

        public OrType<IFrontendGenericType, IFrontendType<IVarifiableType>> Overlay(TypeParameter[] typeParameters)
        {
            var overlay =  new Overlay(typeParameters.ToDictionary(x=>x.parameterDefinition,x=>x.frontendType));
            if (typeParameters.All(x => !(x is IFrontendGenericType)))
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVarifiableType>>(new OverlayTypeDefinition(
                    new WeakTypeDefinition(Scope,Key),overlay));
            }
            else {
                return new OrType<IFrontendGenericType, IFrontendType<IVarifiableType>>(new OverlayGenericTypeDefinition(
                    this, overlay).Cast<IFrontendGenericType>());
            }
        }

        IBuildIntention<IVarifiableType> IConvertable<IVarifiableType>.GetBuildIntention(ConversionContext context) => GetBuildIntention(context);

        IIsPossibly<IFrontendType<IVarifiableType>> IFrontendCodeElement<IGenericInterfaceDefinition>.Returns()
        {
            return Possibly.Is(this);
        }
    }
    
    //internal class GenericTypeParameterDefinition : IGenericTypeParameterDefinition
    //{
    //    public GenericTypeParameterDefinition(string name)
    //    {
    //        Name = name ?? throw new ArgumentNullException(nameof(name));
    //    }

    //    public IKey Key
    //    {
    //        get
    //        {
    //            return new NameKey(Name);
    //        }
    //    }

    //    public string Name { get; }

    //    public override bool Equals(object obj)
    //    {
    //        return obj is GenericTypeParameterDefinition definition &&
    //               Name == definition.Name;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
    //    }

    //    internal bool Accepts(IVarifiableType b)
    //    {
    //        // TODO generic constraints
    //        return true;
    //    }
    //}

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
                        new Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder(new NameKey(x))).ToArray()));
            }

            return TokenMatching<IPopulateScope<WeakGenericTypeDefinition>>.MakeNotMatch(
                    matching.Context);
        }



    }

    internal class GenericTypeDefinitionPopulateScope : IPopulateScope<WeakGenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly IEnumerable<IPopulateScope<IFrontendCodeElement< ICodeElement>>> lines;
        private readonly Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters;
        private readonly Box<IIsPossibly<IFrontendType<IVarifiableType>>> box = new Box<IIsPossibly<IFrontendType<IVarifiableType>>>();

        public GenericTypeDefinitionPopulateScope(
            NameKey nameKey, 
            IEnumerable<IPopulateScope<IFrontendCodeElement<ICodeElement>>> lines,
            Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters)
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

        public IBox<IIsPossibly<IFrontendType<IVarifiableType>>> GetReturnType()
        {
            return box;
        }
    }

    internal class GenericTypeDefinitionResolveReferance : IPopulateBoxes<WeakGenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly IResolvableScope scope;
        private readonly Box<IIsPossibly<IFrontendType<IVarifiableType>>> box;
        private readonly Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters;

        public GenericTypeDefinitionResolveReferance(
            NameKey nameKey, 
            IResolvableScope scope, 
            Box<IIsPossibly<IFrontendType<IVarifiableType>>> box,
            Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters)
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
                scope,
                genericParameters.Select(x=>Possibly.Is(x)).ToArray())));
        }
    }
}
