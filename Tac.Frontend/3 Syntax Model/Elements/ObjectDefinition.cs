﻿using Prototypist.LeftToRight;
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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticObjectDefinitionMaker = AddElementMakers(
            () => new ObjectDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> ObjectDefinitionMaker = StaticObjectDefinitionMaker;
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

    internal class ObjectDefinitionMaker : IMaker<IPopulateScope<WeakObjectDefinition, Tpn.IValue>>
    {
        public ObjectDefinitionMaker()
        {
        }

        public ITokenMatching<IPopulateScope<WeakObjectDefinition, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("object"), out var _)
                .Has(new BodyMaker(), out var block);
            if (matching is IMatchedTokenMatching matched)
            {

                var elements = tokenMatching.Context.ParseBlock(block);
                
                return TokenMatching<IPopulateScope<WeakObjectDefinition, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new ObjectDefinitionPopulateScope(elements));
            }
            return TokenMatching<IPopulateScope<WeakObjectDefinition, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }

        public static IPopulateScope<WeakObjectDefinition, Tpn.IValue> PopulateScope(IPopulateScope<IConvertableFrontendCodeElement<ICodeElement>, Tpn.ITypeProblemNode>[] elements)
        {
            return new ObjectDefinitionPopulateScope(elements);
        }
        public static IPopulateBoxes<WeakObjectDefinition> PopulateBoxes(IResolvableScope scope,
                IPopulateBoxes<IConvertableFrontendCodeElement<ICodeElement>>[] elements)
        {
            return new ResolveReferanceObjectDefinition(scope,
                elements);
        }

        private class ObjectDefinitionPopulateScope : IPopulateScope<WeakObjectDefinition,Tpn.IValue>
        {
            private readonly IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;

            public ObjectDefinitionPopulateScope(IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements)
            {
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public IResolvelizeScope<WeakObjectDefinition,Tpn.IValue> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {
                var key = new ImplicitKey();

                var myScope = context.TypeProblem.CreateObject(scope, key);

                var value = context.TypeProblem.CreateValue(scope, key);
                // ugh! an object is a type
                //

                return new FinalizeScopeObjectDefinition(
                    value,
                    elements.Select(x => x.Run(myScope, context)).ToArray()
                    );
            }
        }

        private class FinalizeScopeObjectDefinition : IResolvelizeScope<WeakObjectDefinition,Tpn.IValue>
        {
            private readonly IResolvelizeScope<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements;

            public FinalizeScopeObjectDefinition(
                Tpn.IValue scope,
                IResolvelizeScope<IFrontendCodeElement, Tpn.ITypeProblemNode>[] elements)
            {
                SetUpSideNode = scope ?? throw new ArgumentNullException(nameof(scope));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public Tpn.IValue SetUpSideNode
            {
                get;
            }

            public IPopulateBoxes<WeakObjectDefinition> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                var finalScope = scope.FinalizeScope(parent);

                return new ResolveReferanceObjectDefinition(finalScope, elements.Select(x => x.Run(finalScope,context)).ToArray());
            }
        }

        private class ResolveReferanceObjectDefinition : IPopulateBoxes<WeakObjectDefinition>
        {
            private readonly IResolvableScope scope;
            private readonly IPopulateBoxes<IFrontendCodeElement>[] elements;

            public ResolveReferanceObjectDefinition(
                IResolvableScope scope,
                IPopulateBoxes<IFrontendCodeElement>[] elements)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public IIsPossibly<WeakObjectDefinition> Run(IResolvableScope _, IResolveReferenceContext context)
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
