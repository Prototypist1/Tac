using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.SemanticModel;

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


namespace Tac.SemanticModel
{


    internal class WeakModuleDefinition : IScoped, IConvertableFrontendCodeElement<IModuleDefinition>, IFrontendType
    {
        public WeakModuleDefinition(IBox<WeakScope> scope, IEnumerable<IBox<IFrontendCodeElement>> staticInitialization, IKey Key)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
        }
        
        public IBox<WeakScope> Scope { get; }
        public IEnumerable<IBox<IFrontendCodeElement>> StaticInitialization { get; }

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
                    Scope.GetValue().Convert(context), 
                    StaticInitialization
                        .Select(x=>x.GetValue().PossiblyConvert(context))
                        .OfType<IIsDefinately<ICodeElement>>()
                        .Select(x=>x.Value)
                        .ToArray(),
                    Key);
            });
        }
    }
    
    // modules are not really objects tho
    // they have very constrained syntax
    // they only can contain constants, methods and implementations 
    internal class ModuleDefinitionMaker : IMaker<ISetUp<WeakModuleDefinition, Tpn.TypeProblem2.Object>>
    {
        public ModuleDefinitionMaker()
        {
        }
        

        public ITokenMatching<ISetUp<WeakModuleDefinition, Tpn.TypeProblem2.Object>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("module"), out _)
                .Has(new NameMaker(), out var name)
                .Has(new BodyMaker(), out var third);
            if (matching is IMatchedTokenMatching matched)
            {
                var elements = matching.Context.ParseBlock(third!);
                var nameKey = new NameKey(name!.Item);

                return TokenMatching<ISetUp<WeakModuleDefinition, Tpn.TypeProblem2.Object>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new ModuleDefinitionPopulateScope(elements, nameKey));

            }
            return TokenMatching<ISetUp<WeakModuleDefinition, Tpn.TypeProblem2.Object>>.MakeNotMatch(
                    matching.Context);
        }


        public static ISetUp<WeakModuleDefinition, Tpn.TypeProblem2.Object> PopulateScope(ISetUp<IConvertableFrontendCodeElement<ICodeElement>, Tpn.ITypeProblemNode>[] elements,
                NameKey nameKey)
        {
            return new ModuleDefinitionPopulateScope(elements,
                nameKey);
        }


        private class ModuleDefinitionPopulateScope : ISetUp<WeakModuleDefinition, Tpn.TypeProblem2.Object>
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

            public ISetUpResult<WeakModuleDefinition, Tpn.TypeProblem2.Object> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var box = new Box<IResolve<IFrontendCodeElement>[]>();
                var myScope= context.TypeProblem.CreateObjectOrModule(scope, nameKey, new WeakModuleConverter(box, nameKey));
                box.Fill(elements.Select(x => x.Run(myScope, context).Resolve).ToArray());

                return new SetUpResult<WeakModuleDefinition, Tpn.TypeProblem2.Object>(new ModuleDefinitionResolveReferance(myScope),myScope);
            }
        }

        private class ModuleDefinitionResolveReferance : IResolve<WeakModuleDefinition>
        {
            private readonly Tpn.TypeProblem2.Object myScope;

            public ModuleDefinitionResolveReferance(Tpn.TypeProblem2.Object myScope)
            {
                this.myScope = myScope;
            }

            public IBox<WeakModuleDefinition> Run(Tpn.ITypeSolution context)
            {
                var moduleOr = context.GetObject(myScope);
                if (moduleOr.GetValue().Is2(out var v2)) {
                    return new Box<WeakModuleDefinition>(v2);
                }
                throw new Exception("wrong or");
            }
        }
    }
}
