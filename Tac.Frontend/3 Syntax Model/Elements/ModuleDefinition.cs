﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticModuleDefinitionMaker = AddElementMakers(
            () => new ModuleDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> ModuleDefinitionMaker = StaticModuleDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model
{


    internal class WeakModuleDefinition : IScoped, IConvertableFrontendCodeElement<IModuleDefinition>, IFrontendType
    {
        public WeakModuleDefinition(IResolvableScope scope, IEnumerable<IIsPossibly<IFrontendCodeElement>> staticInitialization, IKey Key)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
        }
        
        public IResolvableScope Scope { get; }
        public IEnumerable<IIsPossibly<IFrontendCodeElement>> StaticInitialization { get; }

        public IKey Key
        {
            get;
        }

        public IBuildIntention<IModuleDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ModuleDefinition.Create();
            return new BuildIntention<IModuleDefinition>(toBuild, () =>
            {
                maker.Build(
                    Scope.Convert(context), 
                    StaticInitialization
                        .Select(x=>x.GetOrThrow().PossiblyConvert(context))
                        .OfType<IIsDefinately<ICodeElement>>()
                        .Select(x=>x.Value)
                        .ToArray(),
                    Key);
            });
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is(this);
        }
    }
    
    // modules are not really objects tho
    // they have very constrained syntax
    // they only can contain constants, methods and implementations 
    internal class ModuleDefinitionMaker : IMaker<ISetUp<WeakModuleDefinition, Tpn.IObject>>
    {
        public ModuleDefinitionMaker()
        {
        }
        

        public ITokenMatching<ISetUp<WeakModuleDefinition, Tpn.IObject>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("module"), out _)
                .Has(new NameMaker(), out var name)
                .Has(new BodyMaker(), out var third);
            if (matching is IMatchedTokenMatching matched)
            {
                var elements = matching.Context.ParseBlock(third);
                var nameKey = new NameKey(name.Item);

                return TokenMatching<ISetUp<WeakModuleDefinition, Tpn.IObject>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new ModuleDefinitionPopulateScope(elements, nameKey));

            }
            return TokenMatching<ISetUp<WeakModuleDefinition, Tpn.IObject>>.MakeNotMatch(
                    matching.Context);
        }


        public static ISetUp<WeakModuleDefinition, Tpn.IObject> PopulateScope(ISetUp<IConvertableFrontendCodeElement<ICodeElement>, Tpn.ITypeProblemNode>[] elements,
                NameKey nameKey)
        {
            return new ModuleDefinitionPopulateScope(elements,
                nameKey);
        }


        private class ModuleDefinitionPopulateScope : ISetUp<WeakModuleDefinition, Tpn.IObject>
        {
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;
            private readonly NameKey nameKey;

            public ModuleDefinitionPopulateScope(
                ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements,
                NameKey nameKey)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            }

            public ISetUpResult<WeakModuleDefinition, Tpn.IObject> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var myScope= context.TypeProblem.CreateObject(scope, nameKey);

                return new SetUpResult<WeakModuleDefinition, Tpn.IObject>(new ModuleDefinitionResolveReferance(
                    elements.Select(x => x.Run(myScope, context).Resolve).ToArray(),
                    nameKey),myScope);
            }
        }

        private class ModuleDefinitionResolveReferance : IResolve<WeakModuleDefinition>
        {
            private readonly IResolve<IFrontendCodeElement>[] resolveReferance;
            private readonly NameKey nameKey;

            public ModuleDefinitionResolveReferance(
                IResolve<IFrontendCodeElement>[] resolveReferance,
                NameKey nameKey)
            {
                this.resolveReferance = resolveReferance ?? throw new ArgumentNullException(nameof(resolveReferance));
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            }

            public IIsPossibly<WeakModuleDefinition> Run(IResolvableScope _, IResolveContext context)
            {
                var innerRes = new WeakModuleDefinition(
                        scope,
                        resolveReferance.Select(x => x.Run(scope,context)).ToArray(),
                        nameKey);

                var res = Possibly.Is(innerRes);

                return res;
            }
        }
    }
}
