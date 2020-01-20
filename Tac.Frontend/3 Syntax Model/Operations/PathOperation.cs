using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticPathSymbol = StaticSymbolsRegistry.AddOrThrow(".");
        public readonly string PathSymbol = StaticPathSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticPathMaker = AddOperationMatcher(() => new PathOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> PathMaker = StaticPathMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.SemanticModel.Operations
{
    internal class WeakPathOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IPathOperation>
    {
        public WeakPathOperation(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<IPathOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = PathOperation.Create();
            return new BuildIntention<IPathOperation>(toBuild, () =>
            {
                maker.Build(Left.GetValue().ConvertElementOrThrow(context), Right.GetValue().ConvertElementOrThrow(context));
            });
        }
    }


    internal class PathOperationMaker : IMaker<ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>>
    {

        public PathOperationMaker()
        {
        }


        public ITokenMatching<ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .HasStruct(new BinaryOperationMatcher(SymbolsRegistry.StaticPathSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching)
            {
                if (tokenMatching.Tokens[tokenMatching.Tokens.Count - 1] is ElementToken elementToken && 
                    elementToken.Tokens.Single() is AtomicToken atomic ) {
                    var left = matching.Context.ParseLine(match.perface);
                    //var right = matching.Context.ExpectPathPart(box).ParseParenthesisOrElement(match.rhs);

                    return TokenMatching<ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>>.MakeMatch(
                        Array.Empty<IToken>(),
                        matching.Context,
                        new WeakPathOperationPopulateScope(left, atomic.Item));
                }
            }

            return TokenMatching<ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakPathOperationPopulateScope : ISetUp<WeakPathOperation, Tpn.TypeProblem2.Member>
        {
            private readonly ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> left;
            private readonly string name;

            public WeakPathOperationPopulateScope(ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode> left,
                string name)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public ISetUpResult<WeakPathOperation, Tpn.TypeProblem2.Member> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var nextLeft = left.Run(scope, context);
                var member = context.TypeProblem.CreateHopefulMember(nextLeft.SetUpSideNode.CastTo<Tpn.IValue>(), new NameKey(name), new WeakMemberDefinitionConverter(false,new NameKey(name)));

                return new SetUpResult<WeakPathOperation, Tpn.TypeProblem2.Member>(new WeakPathOperationResolveReference(
                    left.Run(scope, context).Resolve,
                     member),member);
            }
        }

        private class WeakPathOperationResolveReference : IResolve<WeakPathOperation>
        {
            readonly IResolve<IFrontendCodeElement> left;
            readonly Tpn.TypeProblem2.Member member;

            public WeakPathOperationResolveReference(
                IResolve<IFrontendCodeElement> resolveReference, 
                Tpn.TypeProblem2.Member member)
            {
                left = resolveReference ?? throw new ArgumentNullException(nameof(resolveReference));
                this.member = member ?? throw new ArgumentNullException(nameof(member));
            }

            public IBox<WeakPathOperation> Run(Tpn.ITypeSolution context)
            {
                var convertedMember = context.GetMember(member);

                var res = new Box<WeakPathOperation>(new WeakPathOperation(
                    left.Run(context),
                    new Box<WeakMemberReference>(new WeakMemberReference(convertedMember))));
                return res;
            }
        }
    }
}
