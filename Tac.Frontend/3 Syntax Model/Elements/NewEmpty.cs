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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticEmptyInstanceMaker = AddElementMakers(
            () => new EmptyInstanceMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> EmptyInstanceMaker = StaticEmptyInstanceMaker;
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

    internal class EmptyInstanceMaker : IMaker<IPopulateScope<WeakEmptyInstance,Tpn.IValue>>
    {
        public EmptyInstanceMaker() { }

        public ITokenMatching<IPopulateScope<WeakEmptyInstance, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            // change key word to nothing?
            var match = tokenMatching
                .Has(new KeyWordMaker("new-empty"), out var _);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakEmptyInstance, Tpn.IValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new EmptyInstancePopulateScope());
            }
            return TokenMatching<IPopulateScope<WeakEmptyInstance, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakEmptyInstance, Tpn.IValue> PopulateScope()
        {
            return new EmptyInstancePopulateScope();
        }
        public static IPopulateBoxes<WeakEmptyInstance> PopulateBoxes()
        {
            return new EmptyInstanceResolveReferance();
        }

        private class EmptyInstancePopulateScope : IPopulateScope<WeakEmptyInstance, Tpn.IValue>
        {

            public EmptyInstancePopulateScope() { }

            public IResolvelizeScope<WeakEmptyInstance, Tpn.IValue> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {
                var value = context.TypeProblem.CreateValue(scope,new NameKey("empty"));
                return new EmptyInstanceFinalizeScope(value);
            }
        }

        private class EmptyInstanceFinalizeScope : IResolvelizeScope<WeakEmptyInstance, Tpn.IValue>
        {

            public EmptyInstanceFinalizeScope(Tpn.IValue value) { SetUpSideNode = value ?? throw new ArgumentNullException(nameof(value)); }

            public Tpn.IValue SetUpSideNode
            {
                get;
            }

            public IPopulateBoxes<WeakEmptyInstance> Run(IResolvableScope scope, IFinalizeScopeContext context)
            {
                return new EmptyInstanceResolveReferance();
            }

        }

        private class EmptyInstanceResolveReferance : IPopulateBoxes<WeakEmptyInstance>
        {
            public EmptyInstanceResolveReferance()
            {
            }

            public IIsPossibly<WeakEmptyInstance> Run(IResolvableScope _, IResolveReferenceContext context)
            {
                return Possibly.Is(new WeakEmptyInstance());
            }
        }
    }
}
