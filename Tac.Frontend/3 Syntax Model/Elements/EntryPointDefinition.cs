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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticEntryPointDefinitionMaker = AddElementMakers(
            () => new EntryPointDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> EntryPointDefinitionMaker = StaticEntryPointDefinitionMaker;
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
                    Body.Select(x => x.Convert(x=>x.GetValue().ConvertElementOrThrow(context))).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class EntryPointDefinitionMaker : IMaker<ISetUp<WeakEntryPointDefinition, Tpn.IScope>>
    {
        public EntryPointDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<WeakEntryPointDefinition, Tpn.IScope>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            {
                var matching = tokenMatching
                    .Has(new KeyWordMaker("entry-point"), out var _)
                    .Has(new BodyMaker(), out var body);
                if (matching
                     is IMatchedTokenMatching matched)
                {
                    var elements = matching.Context.ParseBlock(body!);


                    return TokenMatching<ISetUp<WeakEntryPointDefinition, Tpn.IScope>>.MakeMatch(
                        matched.Tokens,
                        matched.Context,
                        new EntryPointDefinitionPopulateScope(elements)
                        );
                }

                return TokenMatching<ISetUp<WeakEntryPointDefinition, Tpn.IScope>>.MakeNotMatch(
                        matching.Context);
            }

        }


        private class EntryPointDefinitionPopulateScope : ISetUp<WeakEntryPointDefinition, Tpn.IScope>
        {
            private readonly IReadOnlyList<IOrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError>> elements;

            public EntryPointDefinitionPopulateScope(
                IReadOnlyList<IOrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError>> elements
                )
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public ISetUpResult<WeakEntryPointDefinition, Tpn.IScope> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var box = new Box<IOrType<IResolve<IFrontendCodeElement>, IError>[]>();
                var innerScope = context.TypeProblem.CreateScope(scope, new WeakEntryPointConverter(box));
                context.TypeProblem.HasEntryPoint(scope, innerScope);

                box.Fill(elements.Select(x => 
                    x.SwitchReturns(
                        y=> new OrType<IResolve<IFrontendCodeElement>, IError>(y.Run(innerScope, context).Resolve),
                        y=> new OrType<IResolve<IFrontendCodeElement>, IError>(y)))
                .ToArray());

                return new SetUpResult<WeakEntryPointDefinition, Tpn.IScope>(new EntryPointDefinitionResolveReferance(innerScope), new OrType<Tpn.IScope, IError>(innerScope));
            }
        }

        private class EntryPointDefinitionResolveReferance : IResolve<WeakEntryPointDefinition>
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


