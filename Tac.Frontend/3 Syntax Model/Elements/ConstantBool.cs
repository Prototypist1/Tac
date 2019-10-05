using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> StaticConstantBoolMaker = AddElementMakers(
            () => new ConstantBoolMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> ConstantBoolMaker = StaticConstantBoolMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model.Operations
{

    internal class WeakConstantBool : IConvertableFrontendCodeElement<IConstantBool>
    {
        public WeakConstantBool(IIsPossibly<bool> value)
        {
            Value = value;
        }

        public IIsPossibly<bool> Value { get; }

        public IBuildIntention<IConstantBool> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ConstantBool.Create();
            return new BuildIntention<IConstantBool>(toBuild, () =>
            {
                maker.Build(
                    Value.GetOrThrow());
            });
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IFrontendType>(PrimitiveTypes.CreateNumberType());
        }
    }

    internal class ConstantBoolMaker : IMaker<IPopulateScope<WeakConstantBool, ISetUpValue>>
    {
        public ConstantBoolMaker() { }


        internal class BoolMaker : IMaker<bool>
        {
            public ITokenMatching<bool> TryMake(IMatchedTokenMatching self)
            {
                if (self.Tokens.Any() &&
                    self.Tokens.First() is AtomicToken first)
                {
                    if (first.Item == "true") {
                        return TokenMatching<bool>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, true);

                    }
                    else if (first.Item == "false"){
                        return TokenMatching<bool>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, false);

                    }

                }

                return TokenMatching<bool>.MakeNotMatch(self.Context);
            }
        }


        public ITokenMatching<IPopulateScope<WeakConstantBool, ISetUpValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new BoolMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakConstantBool, ISetUpValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantBoolPopulateScope(dub));
            }
            return TokenMatching<IPopulateScope<WeakConstantBool, ISetUpValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakConstantBool, ISetUpValue> PopulateScope(bool dub)
        {
            return new ConstantBoolPopulateScope(dub);
        }
        public static IPopulateBoxes<WeakConstantBool> PopulateBoxes(bool dub)
        {
            return new ConstantBoolResolveReferance(dub);
        }

        private class ConstantBoolPopulateScope : IPopulateScope<WeakConstantBool, ISetUpValue>
        {
            private readonly bool dub;

            public ConstantBoolPopulateScope(bool dub)
            {
                this.dub = dub;
            }

            public IResolvelizeScope<WeakConstantBool, ISetUpValue> Run(IDefineMembers scope, IPopulateScopeContext context)
            {
                var boolType= context.TypeProblem.CreateTypeReference(new NameKey("bool"));
                var value = context.TypeProblem.CreateValue(boolType);
                return new ConstantBoolFinalizeScope(dub,value);
            }
        }

        private class ConstantBoolFinalizeScope : IResolvelizeScope<WeakConstantBool, ISetUpValue>
        {
            private readonly bool dub;

            public ConstantBoolFinalizeScope(bool dub, ISetUpValue setUpSideNode)
            {
                this.dub = dub;
                SetUpSideNode = setUpSideNode ?? throw new System.ArgumentNullException(nameof(setUpSideNode));
            }

            public ISetUpValue SetUpSideNode  {get;}

            public IPopulateBoxes<WeakConstantBool> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new ConstantBoolResolveReferance(dub);
            }
        }


        private class ConstantBoolResolveReferance : IPopulateBoxes<WeakConstantBool>
        {
            private readonly bool dub;

            public ConstantBoolResolveReferance(
                bool dub)
            {
                this.dub = dub;
            }

            public IIsPossibly<WeakConstantBool> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                return Possibly.Is(new WeakConstantBool(Possibly.Is(dub)));
            }
        }
    }
}
