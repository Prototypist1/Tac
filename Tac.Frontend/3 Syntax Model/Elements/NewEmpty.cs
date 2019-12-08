using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend._2_Parser;
using Tac.Model.Elements;
using Tac.Model.Instantiated.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Frontend;
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.Model;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticEmptyInstanceMaker = AddElementMakers(
            () => new EmptyInstanceMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> EmptyInstanceMaker = StaticEmptyInstanceMaker;
    }
}



namespace Tac.Frontend._3_Syntax_Model.Elements
{
    // what is the empty instance...
    // I though empty was a type
    // more analgous to something like a bool with a single value
    // than the null reference 
    // yeah but you still need to be able to create one...
    internal class WeakEmptyInstance : IConvertableFrontendCodeElement<IEmptyInstance>
    {
        public WeakEmptyInstance()
        {
        }

        public IBuildIntention<IEmptyInstance> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = EmptyInstance.Create();
            return new BuildIntention<IEmptyInstance>(toBuild, () =>
            {
                maker.Build();
            });
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IFrontendType>(PrimitiveTypes.CreateEmptyType());
        }
    }

    internal class EmptyInstanceMaker : IMaker<ISetUp<WeakEmptyInstance, LocalTpn.IValue>>
    {
        public EmptyInstanceMaker() { }

        public ITokenMatching<ISetUp<WeakEmptyInstance, LocalTpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            // change key word to nothing?
            var match = tokenMatching
                .Has(new KeyWordMaker("new-empty"), out var _);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakEmptyInstance, LocalTpn.IValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new EmptyInstancePopulateScope());
            }
            return TokenMatching<ISetUp<WeakEmptyInstance, LocalTpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static ISetUp<WeakEmptyInstance, LocalTpn.IValue> PopulateScope()
        {
            return new EmptyInstancePopulateScope();
        }
        public static IResolve<WeakEmptyInstance> PopulateBoxes()
        {
            return new EmptyInstanceResolveReferance();
        }

        private class EmptyInstancePopulateScope : ISetUp<WeakEmptyInstance, LocalTpn.IValue>
        {

            public EmptyInstancePopulateScope() { }

            public ISetUpResult<WeakEmptyInstance, LocalTpn.IValue> Run(LocalTpn.IScope scope, ISetUpContext context)
            {
                var value = context.TypeProblem.CreateValue(scope,new NameKey("empty"),new PlaceholderValueConverter());
                return new SetUpResult<WeakEmptyInstance, LocalTpn.IValue>(new EmptyInstanceResolveReferance(),value);
            }
        }

        private class EmptyInstanceResolveReferance : IResolve<WeakEmptyInstance>
        {
            public EmptyInstanceResolveReferance()
            {
            }

            public IBox<WeakEmptyInstance> Run(LocalTpn.ITypeSolution context)
            {
                return new Box<WeakEmptyInstance>(new WeakEmptyInstance());
            }
        }
    }
}
