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

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> StaticEmptyInstanceMaker = AddElementMakers(
            () => new EmptyInstanceMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>>(typeof(MemberMaker)));
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> EmptyInstanceMaker = StaticEmptyInstanceMaker;
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

    internal class EmptyInstanceMaker : IMaker<IPopulateScope<WeakEmptyInstance,ISetUpValue>>
    {
        public EmptyInstanceMaker() { }

        public ITokenMatching<IPopulateScope<WeakEmptyInstance, ISetUpValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            // change key word to nothing?
            var match = tokenMatching
                .Has(new KeyWordMaker("new-empty"), out var _);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakEmptyInstance, ISetUpValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new EmptyInstancePopulateScope());
            }
            return TokenMatching<IPopulateScope<WeakEmptyInstance, ISetUpValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakEmptyInstance, ISetUpValue> PopulateScope()
        {
            return new EmptyInstancePopulateScope();
        }
        public static IPopulateBoxes<WeakEmptyInstance> PopulateBoxes()
        {
            return new EmptyInstanceResolveReferance();
        }

        private class EmptyInstancePopulateScope : IPopulateScope<WeakEmptyInstance, ISetUpValue>
        {

            public EmptyInstancePopulateScope() { }

            public IResolvelizeScope<WeakEmptyInstance, ISetUpValue> Run(IDefineMembers scope, IPopulateScopeContext context)
            {
                var emptyType = context.TypeProblem.CreateTypeReference(new NameKey("empty"));
                var value = context.TypeProblem.CreateValue(emptyType);
                return new EmptyInstanceFinalizeScope(value);
            }
        }

        private class EmptyInstanceFinalizeScope : IResolvelizeScope<WeakEmptyInstance, ISetUpValue>
        {

            public EmptyInstanceFinalizeScope(ISetUpValue value) { SetUpSideNode = value ?? throw new ArgumentNullException(nameof(value)); }

            public ISetUpValue SetUpSideNode
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
