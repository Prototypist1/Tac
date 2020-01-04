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
using Prototypist.Toolbox;
using Tac.Frontend._3_Syntax_Model.Operations;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticObjectDefinitionMaker = AddElementMakers(
            () => new ObjectDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> ObjectDefinitionMaker = StaticObjectDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model
{

    internal class WeakObjectDefinition: IConvertableFrontendCodeElement<IObjectDefiniton>,  IScoped, IFrontendType
    {
        public WeakObjectDefinition(IBox<WeakScope> scope, IEnumerable<IBox<WeakAssignOperation>> assigns) {
            if (assigns == null)
            {
                throw new ArgumentNullException(nameof(assigns));
            }

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assigns.ToArray();
        }

        public IBox<WeakScope> Scope { get; }
        public IBox<WeakAssignOperation>[] Assignments { get; }

        public IBuildIntention<IObjectDefiniton> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ObjectDefiniton.Create();
            return new BuildIntention<IObjectDefiniton>(toBuild, () =>
            {
                maker.Build(Scope.GetValue().Convert(context), 
                    Assignments.Select(x => x.GetValue().Convert(context)).ToArray());
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
            var matching = tokenMatching
                .Has(new KeyWordMaker("object"), out var _)
                .Has(new BodyMaker(), out var block);
            if (matching is IMatchedTokenMatching matched)
            {

                var elements = tokenMatching.Context.ParseBlock(block);
                
                return TokenMatching<ISetUp<WeakObjectDefinition, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new ObjectDefinitionPopulateScope(elements));
            }
            return TokenMatching<ISetUp<WeakObjectDefinition, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }

        public static ISetUp<WeakObjectDefinition, Tpn.IValue> PopulateScope(ISetUp<IConvertableFrontendCodeElement<ICodeElement>, Tpn.ITypeProblemNode>[] elements)
        {
            return new ObjectDefinitionPopulateScope(elements);
        }

        private class ObjectDefinitionPopulateScope : ISetUp<WeakObjectDefinition, Tpn.IValue>
        {
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;

            public ObjectDefinitionPopulateScope(ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public ISetUpResult<WeakObjectDefinition, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var key = new ImplicitKey(Guid.NewGuid());

                var box = new Box<IResolve<IFrontendCodeElement>[]>();
                var myScope = context.TypeProblem.CreateObject(scope, key, new WeakObjectConverter(box));
                box.Fill(elements.Select(x => x.Run(myScope, context).Resolve).ToArray());

                var value = context.TypeProblem.CreateValue(scope, key, new PlaceholderValueConverter());
                // ugh! an object is a type
                //

                return new SetUpResult<WeakObjectDefinition, Tpn.IValue>(new ResolveReferanceObjectDefinition(myScope),value);
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
