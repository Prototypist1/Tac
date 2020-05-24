using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticEntryPointDefinitionMaker = AddElementMakers(
            () => new EntryPointDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> EntryPointDefinitionMaker = StaticEntryPointDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel
{

    internal class WeakEntryPointDefinition :
        WeakAbstractBlockDefinition<IEntryPointDefinition>
    {
        public WeakEntryPointDefinition(
            IOrType<IBox<IFrontendCodeElement>,IError>[] body,
            IBox<WeakScope> scope,
            IReadOnlyList<IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>>> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            
        }

        public override IBuildIntention<IEntryPointDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = EntryPointDefinition.Create();
            return new BuildIntention<IEntryPointDefinition>(toBuild, () =>
            {
                maker.Build(
                    Scope.GetValue().Convert(context),
                    Body.Select(x => x.Is1OrThrow().GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class EntryPointDefinitionMaker : IMaker<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>>
    {
        public EntryPointDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                    .Has(new KeyWordMaker("entry-point"), out var _)
                    .Has(new BodyMaker());
            return matching.ConvertIfMatched(block => new EntryPointDefinitionPopulateScope(matching.Context.ParseBlock(block)));
        }


        private class EntryPointDefinitionPopulateScope : ISetUp<IBox<WeakEntryPointDefinition>, Tpn.IScope>
        {
            private readonly IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements;

            public EntryPointDefinitionPopulateScope(
                IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements
                )
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public ISetUpResult<IBox<WeakEntryPointDefinition>, Tpn.IScope> Run(Tpn.IStaticScope scope, ISetUpContext context)
            {


                var box = new Box<IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[]>();
                var innerScope = context.TypeProblem.CreateScope(scope, new WeakEntryPointConverter(box));
                context.TypeProblem.HasEntryPoint(scope, innerScope);

                box.Fill(elements.Select(x => 
                    x.SwitchReturns(
                        y=> OrType.Make<IResolve<IBox<IFrontendCodeElement>>, IError>(y.Run(innerScope, context).Resolve),
                        y=> OrType.Make<IResolve<IBox<IFrontendCodeElement>>, IError>(y)))
                .ToArray());

                return new SetUpResult<IBox<WeakEntryPointDefinition>, Tpn.IScope>(new EntryPointDefinitionResolveReferance(innerScope), OrType.Make<Tpn.IScope, IError>(innerScope));
            }
        }

        private class EntryPointDefinitionResolveReferance : IResolve<IBox<WeakEntryPointDefinition>>
        {
            private readonly Tpn.TypeProblem2.Scope scope;

            public EntryPointDefinitionResolveReferance(Tpn.TypeProblem2.Scope scope)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            }

            public IBox<WeakEntryPointDefinition> Run(Tpn.ITypeSolution context)
            {
                var res = context.GetScope(scope);
                if (res.GetValue().Is3(out var v3))
                {
                    return new Box<WeakEntryPointDefinition>(v3);
                }
                throw new Exception("wrong!");
            }
        }
    }
}


