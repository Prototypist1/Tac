using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticModuleDefinitionMaker = AddElementMakers(
            () => new ModuleDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> ModuleDefinitionMaker = StaticModuleDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}


namespace Tac.SemanticModel
{

    // honestly these being types is wierd
    // espially since this is probably the same type as an object
    // I think this returns a WeakTypeDefinition or maybe there should be a class for that
    // I think there should be a class for that
    internal class WeakModuleDefinition : IScoped, IConvertableFrontendCodeElement<IModuleDefinition>, IReturn
    {
        public WeakModuleDefinition(IBox<WeakScope> scope, IReadOnlyList<IOrType<IBox<IFrontendCodeElement>,IError>> staticInitialization, IKey Key, IBox<WeakEntryPointDefinition> entryPoint)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            this.Key = Key ?? throw new ArgumentNullException(nameof(Key));
            EntryPoint = entryPoint ?? throw new ArgumentNullException(nameof(entryPoint));
        }
        
        public IBox<WeakScope> Scope { get; }
        IBox<WeakEntryPointDefinition> EntryPoint { get; }
        public IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>> StaticInitialization { get; }

        public IKey Key
        {
            get;
        }

        public IBuildIntention<IModuleDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ModuleDefinition.Create();
            return new BuildIntention<IModuleDefinition>(toBuild, () =>
            {
                var staticInit = new List<IOrType<ICodeElement, IError>>();

                foreach (var item in StaticInitialization)
                {
                    item.Switch(x1=> {
                        var converted = x1.GetValue().PossiblyConvert(context);
                        if (converted is IIsDefinately<ICodeElement> isCodeElement) {
                            staticInit.Add(OrType.Make<ICodeElement, IError>(isCodeElement.Value));
                        }
                    },x2=> {
                        staticInit.Add(OrType.Make<ICodeElement, IError>(x2));
                    });
                }


                maker.Build(
                    Scope.GetValue().Convert(context),
                    staticInit.Select(x=>x.Is1OrThrow()).ToArray(),
                    Key,
                    EntryPoint.GetValue().Convert(context));
            });
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in Scope.GetValue().Validate())
            {
                yield return error;
            }
            foreach (var error in EntryPoint.GetValue().Validate())
            {
                yield return error;
            }
            foreach (var line in StaticInitialization)
            {
                foreach (var error in line.SwitchReturns(x=>x.GetValue().Validate(),x=>new[] { x}))
                {
                    yield return error;
                }
            }
        }

        public IFrontendType AssuredReturns()
        {
            return new HasMembersType(Scope.GetValue());
        }

        public IOrType<IFrontendType, IError> Returns()
        {
            return OrType.Make< IFrontendType, IError > (AssuredReturns());
        }
    }
    
    // modules are not really objects tho
    // they have very constrained syntax
    // they only can contain constants, methods and implementations 
    internal class ModuleDefinitionMaker : IMaker<ISetUp<IBox<WeakModuleDefinition>, Tpn.TypeProblem2.Object>>
    {
        public ModuleDefinitionMaker()
        {
        }
        

        public ITokenMatching<ISetUp<IBox<WeakModuleDefinition>, Tpn.TypeProblem2.Object>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("module"), out var _)
                .Has(new NameMaker())
                .Has(new BodyMaker());

            return matching.ConvertIfMatched((name,third)=> new ModuleDefinitionPopulateScope(matching.Context.ParseBlock(third), new NameKey(name.Item)));
        }

        private class ModuleDefinitionPopulateScope : ISetUp<IBox<WeakModuleDefinition>, Tpn.TypeProblem2.Object>
        {
            private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;
            private readonly NameKey nameKey;

            public ModuleDefinitionPopulateScope(
                IReadOnlyList<IOrType< ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError>> elements,
                NameKey nameKey)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            }

            public ISetUpResult<IBox<WeakModuleDefinition>, Tpn.TypeProblem2.Object> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var box = new Box<IReadOnlyList<IOrType< IResolve<IBox<IFrontendCodeElement>>,IError>>>();
                var myScope= context.TypeProblem.CreateObjectOrModule(scope, nameKey, new WeakModuleConverter(box, nameKey));
                box.Fill(elements.Select(x => x.TransformInner(y=>y.Run(myScope, context).Resolve)).ToArray());

                return new SetUpResult<IBox<WeakModuleDefinition>, Tpn.TypeProblem2.Object>(new ModuleDefinitionResolveReferance(myScope), OrType.Make<Tpn.TypeProblem2.Object, IError>(myScope));
            }
        }

        private class ModuleDefinitionResolveReferance : IResolve<IBox<WeakModuleDefinition>>
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
