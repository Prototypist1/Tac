using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticEmptyInstanceMaker = AddElementMakers(
            () => new EmptyInstanceMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> EmptyInstanceMaker = StaticEmptyInstanceMaker;
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

    internal class EmptyInstanceMaker : IMaker<ISetUp<WeakEmptyInstance,Tpn.IValue>>
    {
        public EmptyInstanceMaker() { }

        public ITokenMatching<ISetUp<WeakEmptyInstance, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            // change key word to nothing?
            var match = tokenMatching
                .Has(new KeyWordMaker("new-empty"), out var _);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakEmptyInstance, Tpn.IValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new EmptyInstancePopulateScope());
            }
            return TokenMatching<ISetUp<WeakEmptyInstance, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static ISetUp<WeakEmptyInstance, Tpn.IValue> PopulateScope()
        {
            return new EmptyInstancePopulateScope();
        }
        public static IResolve<WeakEmptyInstance> PopulateBoxes()
        {
            return new EmptyInstanceResolveReferance();
        }

        private class EmptyInstancePopulateScope : ISetUp<WeakEmptyInstance, Tpn.IValue>
        {

            public EmptyInstancePopulateScope() { }

            public ISetUpResult<WeakEmptyInstance, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var value = context.TypeProblem.CreateValue(scope,new NameKey("empty"));
                return new SetUpResult<WeakEmptyInstance, Tpn.IValue>(new EmptyInstanceResolveReferance(),value);
            }
        }

        private class EmptyInstanceResolveReferance : IResolve<WeakEmptyInstance>
        {
            public EmptyInstanceResolveReferance()
            {
            }

            public IIsPossibly<WeakEmptyInstance> Run(IResolvableScope _, IResolveContext context)
            {
                return Possibly.Is(new WeakEmptyInstance());
            }
        }
    }
}
