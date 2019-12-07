using Prototypist.LeftToRight;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Semantic_Model;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticObjectDefinitionMaker = AddElementMakers(
            () => new ObjectDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> ObjectDefinitionMaker = StaticObjectDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model
{

    internal class WeakObjectDefinition: IConvertableFrontendCodeElement<IObjectDefiniton>,  IScoped, IFrontendType
    {
        public WeakObjectDefinition(IResolvableScope scope, IEnumerable<IIsPossibly<WeakAssignOperation>> assigns) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assigns.ToArray();
        }

        public IResolvableScope Scope { get; }
        public IIsPossibly<WeakAssignOperation>[] Assignments { get; }

        public IBuildIntention<IObjectDefiniton> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ObjectDefiniton.Create();
            return new BuildIntention<IObjectDefiniton>(toBuild, () =>
            {
                maker.Build(Scope.Convert(context), 
                    Assignments.Select(x => x.GetOrThrow().Convert(context)).ToArray());
            });
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is(this);
        }
    }

    internal class ObjectDefinitionMaker : IMaker<ISetUp<WeakObjectDefinition, LocalTpn.IValue>>
    {
        public ObjectDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<WeakObjectDefinition, LocalTpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("object"), out var _)
                .Has(new BodyMaker(), out var block);
            if (matching is IMatchedTokenMatching matched)
            {

                var elements = tokenMatching.Context.ParseBlock(block);
                
                return TokenMatching<ISetUp<WeakObjectDefinition, LocalTpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new ObjectDefinitionPopulateScope(elements));
            }
            return TokenMatching<ISetUp<WeakObjectDefinition, LocalTpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }

        public static ISetUp<WeakObjectDefinition, LocalTpn.IValue> PopulateScope(ISetUp<IConvertableFrontendCodeElement<ICodeElement>, LocalTpn.ITypeProblemNode>[] elements)
        {
            return new ObjectDefinitionPopulateScope(elements);
        }

        private class ObjectDefinitionPopulateScope : ISetUp<WeakObjectDefinition, LocalTpn.IValue>
        {
            private readonly ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements;

            public ObjectDefinitionPopulateScope(ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>[] elements)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public ISetUpResult<WeakObjectDefinition, LocalTpn.IValue> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                var key = new ImplicitKey();

                var myScope = context.TypeProblem.CreateObject(scope, key);

                var value = context.TypeProblem.CreateValue(scope, key);
                // ugh! an object is a type
                //

                return new SetUpResult<WeakObjectDefinition, LocalTpn.IValue>(new ResolveReferanceObjectDefinition(
                    elements.Select(x => x.Run(myScope, context).Resolve).ToArray()
                    ),value);
            }
        }

        private class ResolveReferanceObjectDefinition : IResolve<WeakObjectDefinition>
        {
            private readonly IResolve<IFrontendCodeElement>[] elements;

            public ResolveReferanceObjectDefinition(
                IResolve<IFrontendCodeElement>[] elements)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public IIsPossibly<WeakObjectDefinition> Run(IResolvableScope _, IResolveContext context)
            {
                var innerRes = new WeakObjectDefinition(
                            scope,
                            elements.Select(x => x.Run(scope,context).Cast<IIsPossibly<WeakAssignOperation>>()).ToArray());
                var res = Possibly.Is(innerRes);

                return res;
            }
        }
    }
}
