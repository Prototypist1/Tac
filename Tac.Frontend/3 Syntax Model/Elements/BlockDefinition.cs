﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.Frontend.SyntaxModel.Operations;
using Prototypist.Toolbox;
using Tac.Frontend.Parser;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticBlockDefinitionMaker = AddElementMakers(
            () => new BlockDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> BlockDefinitionMaker = StaticBlockDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}

namespace Tac.SemanticModel
{

    internal class WeakBlockDefinition : WeakAbstractBlockDefinition<IBlockDefinition>
    {
        public WeakBlockDefinition(
            IBox<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> body,
            IOrType< WeakScope,IError> scope,
            IReadOnlyList<IIsPossibly<IFrontendCodeElement>> staticInitailizers) :
            base(scope, body, staticInitailizers)
        { }

        public override IBuildIntention<IBlockDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = BlockDefinition.Create();
            return new BuildIntention<IBlockDefinition>(toBuild, () =>
            {
                maker.Build(
                    Scope.Is1OrThrow().Convert(context),
                    Body.GetValue().Select(or => or.Is1OrThrow().GetValue().ConvertElementOrThrow(context)).ToArray(),
                    StaticInitailizers.Select(x => x.GetOrThrow().ConvertElementOrThrow(context)).ToArray());
            });
        }
    }

    internal class BlockDefinitionMaker : IMaker<ISetUp<IBox<WeakBlockDefinition>, Tpn.IScope>>
    {
        public BlockDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakBlockDefinition>, Tpn.IScope>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
               .Has(new BodyMaker(), out var body);

            if (match is IMatchedTokenMatching
               matched)
            {
                var elements = tokenMatching.Context.ParseBlock(body);

                return TokenMatching<ISetUp<IBox<WeakBlockDefinition>, Tpn.IScope>>.MakeMatch(tokenMatching,
                    new BlockDefinitionPopulateScope(elements), matched.EndIndex);
            }

            return TokenMatching<ISetUp<IBox<WeakBlockDefinition>, Tpn.IScope>>.MakeNotMatch(tokenMatching.Context);
        }


        
    }

    internal class BlockDefinitionPopulateScope : ISetUp<IBox<WeakBlockDefinition>, Tpn.IScope>
    {
        // TODO object??
        // is it worth adding another T?
        // this is the type the backend owns
        private IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> Elements { get; }

        public BlockDefinitionPopulateScope(IReadOnlyList<IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError>> elements)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public ISetUpResult<IBox<WeakBlockDefinition>, Tpn.IScope> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            var box = new Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>>();
            var myScope = context.TypeProblem.CreateScope(scope, new WeakBlockDefinitionConverter(box));
            var nextElements = Elements.Select(or => or.TransformInner(y => y.Run(scope, context.CreateChildContext(this)).Resolve)).ToArray();
            return new SetUpResult<IBox<WeakBlockDefinition>, Tpn.IScope>(new ResolveReferanceBlockDefinition(myScope, nextElements,box), OrType.Make<Tpn.IScope, IError>(myScope));
        }
    }

    internal class ResolveReferanceBlockDefinition : IResolve<IBox<WeakBlockDefinition>>
    {
        private readonly Tpn.TypeProblem2.Scope myScope;
        private readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextElements;
        private readonly Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> box;

        public ResolveReferanceBlockDefinition(Tpn.TypeProblem2.Scope myScope, IOrType<IResolve<IBox<IFrontendCodeElement>>, IError>[] nextElements, Box<IReadOnlyList<IOrType<IBox<IFrontendCodeElement>, IError>>> box)
        {
            this.myScope = myScope ?? throw new ArgumentNullException(nameof(myScope));
            this.nextElements = nextElements ?? throw new ArgumentNullException(nameof(nextElements));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IBox<WeakBlockDefinition> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {
            box.Fill(nextElements.Select(x => x.TransformInner(y => y.Run(context, stack.Add(myScope)))).ToArray());

            return new Box<WeakBlockDefinition>( myScope.Converter.Convert(context, myScope, stack).Is1OrThrow());
        }
    }

    public static class IEnumerableExtensions {
        public static IEnumerable<T> Add<T>(this IEnumerable<T> list, T toAdd) {
            foreach (var item in list)
            {
                yield return item;
            }
            yield return toAdd;
        }
    }
}