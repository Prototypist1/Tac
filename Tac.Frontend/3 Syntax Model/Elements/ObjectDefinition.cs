using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Tac.SemanticModel;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Prototypist.Toolbox;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.Parser;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticObjectDefinitionMaker = AddElementMakers(
            () => new ObjectDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> ObjectDefinitionMaker = StaticObjectDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}


namespace Tac.SemanticModel
{

    internal class WeakObjectDefinition: IConvertableFrontendCodeElement<IObjectDefiniton>, IScoped, IFrontendType
    {
        public WeakObjectDefinition(IBox<WeakScope> scope, IReadOnlyList<IOrType<IBox<WeakAssignOperation>,IError>> assigns) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assigns.ToArray();
        }

        public IBox<WeakScope> Scope { get; }
        public IReadOnlyList<IOrType<IBox<WeakAssignOperation>, IError>> Assignments { get; }

        public IBuildIntention<IObjectDefiniton> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ObjectDefiniton.Create();
            return new BuildIntention<IObjectDefiniton>(toBuild, () =>
            {
                maker.Build(
                    Scope.GetValue().Convert(context), 
                    Assignments.Select(x => x.TransformInner(y=>y.GetValue().Convert(context))).ToArray());
            });
        }
    }

    internal class ObjectDefinitionMaker : IMaker<ISetUp<WeakObjectDefinition, Tpn.IValue>>
    {
        public ObjectDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<WeakObjectDefinition, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .Has(new KeyWordMaker("object"), out var _)
                .Has(new BodyMaker())
                .ConvertIfMatched(block => new ObjectDefinitionPopulateScope(tokenMatching.Context.ParseBlock(block)));
        }

        private class ObjectDefinitionPopulateScope : ISetUp<WeakObjectDefinition, Tpn.IValue>
        {
            private readonly IReadOnlyList<IOrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError>> elements;

            public ObjectDefinitionPopulateScope(IReadOnlyList<IOrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError>> elements)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public ISetUpResult<WeakObjectDefinition, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var key = new ImplicitKey(Guid.NewGuid());

                var box = new Box<IReadOnlyList< IOrType<IResolve<IFrontendCodeElement>,IError>>>();
                var myScope = context.TypeProblem.CreateObjectOrModule(scope, key, new WeakObjectConverter(box));
                box.Fill(elements.Select(x => x.TransformInner(y=>y.Run(myScope, context).Resolve)).ToArray());

                var value = context.TypeProblem.CreateValue(scope, key, new PlaceholderValueConverter());
                // ugh! an object is a type
                //

                return new SetUpResult<WeakObjectDefinition, Tpn.IValue>(new ResolveReferanceObjectDefinition(myScope), OrType.Make<Tpn.IValue, IError>(value));
            }
        }

        private class ResolveReferanceObjectDefinition : IResolve<WeakObjectDefinition>
        {
            private readonly Tpn.TypeProblem2.Object myScope;

            public ResolveReferanceObjectDefinition(Tpn.TypeProblem2.Object myScope)
            {
                this.myScope = myScope ?? throw new ArgumentNullException(nameof(myScope));
            }

            // do these really need to be IBox? they seeme to generally be filled...
            // mayble IPossibly...
            public IBox<WeakObjectDefinition> Run(Tpn.ITypeSolution context)
            {
                var objectOr = context.GetObject(myScope);
                if (objectOr.GetValue().Is1(out var v1))
                {
                    return new Box<WeakObjectDefinition>(v1);
                }
                throw new Exception("wrong or");
            }
        }
    }
}
