
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using static Tac.SyntaxModel.Elements.AtomicTypes.PrimitiveTypes;
using Prototypist.TaskChain;
using Prototypist.Fluent;
using Tac.Frontend._3_Syntax_Model.Operations;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticGenericTypeDefinitionMaker = AddElementMakers(
            () => new GenericTypeDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> GenericTypeDefinitionMaker = StaticGenericTypeDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}



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
        public IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions=> backing.TypeParameterDefinitions;
        public OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters) => backing.Overlay(typeParameters);

        public IIsPossibly<IFrontendType> Returns() => Possibly.Is(this);
    }

    internal interface IWeakGenericTypeDefinition: IFrontendCodeElement, IScoped, IFrontendType, IFrontendGenericType
    {
        IIsPossibly<IKey> Key { get; }
    }

    internal class WeakGenericTypeDefinition : IWeakGenericTypeDefinition
    {
        private readonly ConcurrentIndexed<Overlay, Prototypist.Fluent.OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>> typeCache = new ConcurrentIndexed<Overlay, OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>>();

        public WeakGenericTypeDefinition(
            IIsPossibly<NameKey> key,
            IBox<WeakScope> scope,
            IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions)
        {
            this.TypeParameterDefinitions = TypeParameterDefinitions ?? throw new ArgumentNullException(nameof(TypeParameterDefinitions));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
        public IIsPossibly<IKey> Key { get; }
        public IBox<WeakScope> Scope { get; }

        //public IBuildIntention<IGenericType> GetBuildIntention(ConversionContext context)
        //{
        //    var (toBuild, maker) = GenericInterfaceDefinition.Create();
        //    return new BuildIntention<IGenericInterfaceDefinition>(toBuild, () =>
        //    {
        //        maker.Build(
        //            Scope.Convert(context),
        //            TypeParameterDefinitions.Select(x=>x.GetOrThrow().Convert(context)).ToArray());
        //    });
        //}

        public OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
        {
            // I kept reusing this code..
            var overlay =  new Overlay(typeParameters.ToDictionary(x=>x.parameterDefinition,x=>x.frontendType));

            return typeCache.GetOrAdd(overlay, Help());
            
            OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Help()
            {
                if (typeParameters.All(x => !(x.frontendType is IGenericTypeParameterPlacholder)))
                {
                    return new OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new OverlayTypeDefinition(
                        new WeakTypeDefinition(Scope, Key), overlay));
                }
                else
                {
                    return new OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new OverlayGenericTypeDefinition(
                        this, overlay).Cast<IFrontendGenericType>());
                }
            }
        }


        //IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(ConversionContext context) => GetBuildIntention(context);

        IIsPossibly<IFrontendType> IFrontendCodeElement.Returns()
        {
            return Possibly.Is(this);
        }
    }
    
    internal class GenericTypeDefinitionMaker : IMaker<ISetUp<WeakGenericTypeDefinition, LocalTpn.IExplicitType>>
    {

        public GenericTypeDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<WeakGenericTypeDefinition, LocalTpn.IExplicitType>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .Has(new DefineGenericNMaker(), out var genericTypes)
                .Has(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakGenericTypeDefinition, LocalTpn.IExplicitType>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new GenericTypeDefinitionPopulateScope(
                        new NameKey(typeName.Item),
                        tokenMatching.Context.ParseBlock(body),
                        genericTypes.Select(x =>
                        PrimitiveTypes.CreateGenericTypeParameterPlacholder(new NameKey(x))).ToArray()));
            }

            return TokenMatching<ISetUp<WeakGenericTypeDefinition, LocalTpn.IExplicitType>>.MakeNotMatch(
                    matching.Context);
        }

        public static ISetUp<WeakGenericTypeDefinition, LocalTpn.IExplicitType> PopulateScope(
                NameKey nameKey,
                IEnumerable<ISetUp<IConvertableFrontendCodeElement<ICodeElement>, LocalTpn.ITypeProblemNode>> lines,
                IGenericTypeParameterPlacholder[] genericParameters)
        {
            return new GenericTypeDefinitionPopulateScope(
                nameKey,
                lines,
                genericParameters);
        }

        private class GenericTypeDefinitionPopulateScope : ISetUp<WeakGenericTypeDefinition, LocalTpn.IExplicitType>
        {
            private readonly NameKey nameKey;
            private readonly IEnumerable<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> lines;
            private readonly IGenericTypeParameterPlacholder[] genericParameters;
            private readonly Box<IIsPossibly<IFrontendGenericType>> box = new Box<IIsPossibly<IFrontendGenericType>>();

            public GenericTypeDefinitionPopulateScope(
                NameKey nameKey,
                IEnumerable<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> lines,
                IGenericTypeParameterPlacholder[] genericParameters)
            {
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
                this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
                this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
            }

            public ISetUpResult<WeakGenericTypeDefinition, LocalTpn.IExplicitType> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                // oh geez here is a mountain.
                // I generic types are erased 
                // what on earth does this return?
                var myScope = context.TypeProblem.CreateGenericType(scope, nameKey, genericParameters.Select(x=>new LocalTpn.TypeAndConverter(x.Key, new WeakTypeDefinitionConverter())).ToArray(),new WeakGenericTypeDefinitionConverter(nameKey, genericParameters));
                var nextLines = lines.Select(x => x.Run(myScope, context).Resolve).ToArray();
                return new SetUpResult<WeakGenericTypeDefinition, LocalTpn.IExplicitType>(new GenericTypeDefinitionResolveReferance(myScope, nextLines), myScope);
            }
        }

        private class GenericTypeDefinitionResolveReferance : IResolve<WeakGenericTypeDefinition>
        {
            private Tpn<WeakBlockDefinition, Prototypist.Fluent.OrType<WeakTypeDefinition, WeakGenericTypeDefinition>, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Type myScope;
            private IResolve<IFrontendCodeElement>[] nextLines;

            public GenericTypeDefinitionResolveReferance(Tpn<WeakBlockDefinition, Prototypist.Fluent.OrType<WeakTypeDefinition, WeakGenericTypeDefinition>, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.TypeProblem2.Type myScope, IResolve<IFrontendCodeElement>[] nextLines)
            {
                this.myScope = myScope;
                this.nextLines = nextLines;
            }


            public IBox<WeakGenericTypeDefinition> Run(LocalTpn.ITypeSolution context)
            {
                // uhhh it is werid that I have to do this
                nextLines.Select(x => x.Run(context)).ToArray();
                if (context.GetExplicitType(myScope).GetValue().Is2(out var v2)) {
                    return new Box<WeakGenericTypeDefinition>( v2);
                }

                throw new Exception("well that is not good");
            }
        }
    }
}
