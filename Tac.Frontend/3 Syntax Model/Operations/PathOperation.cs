﻿using Prototypist.Toolbox.Object;
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
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Tac.SemanticModel;

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
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticPathMaker = AddOperationMatcher(() => new PathOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> PathMaker = StaticPathMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}

namespace Tac.SemanticModel.Operations
{
    internal class WeakPathOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IPathOperation>, IReturn
    {
        public WeakPathOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }

        public override IBuildIntention<IPathOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = PathOperation.Create();
            return new BuildIntention<IPathOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context),
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public IOrType<IFrontendType, IError> Returns()
        {
            return Right.TransformAndFlatten(x =>
            {
                if (x is IReturn @return) {
                    return @return.Returns();
                }
                return OrType.Make<IFrontendType, IError>(Error.Other($"{Right} should return"));
            });
        }

        public override IEnumerable<IError> Validate()
        {
            foreach (var error in base.Validate())
            {
                yield return error;
            }

            // really not sure how to validate this
            // this will do for now
        }

    }


    internal class PathOperationMaker : IMaker<ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>>
    {

        public PathOperationMaker()
        {
        }


        public ITokenMatching<ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .HasStruct(new BinaryOperationMatcher(SymbolsRegistry.StaticPathSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching)
            {
                if (tokenMatching.Tokens[tokenMatching.Tokens.Count - 1] is ElementToken elementToken &&
                    elementToken.Tokens.Single() is AtomicToken atomic)
                {
                    var left = matching.Context.ParseLine(match.perface);
                    //var right = matching.Context.ExpectPathPart(box).ParseParenthesisOrElement(match.rhs);

                    return TokenMatching<ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>>.MakeMatch(
                        Array.Empty<IToken>(),
                        matching.Context,
                        new WeakPathOperationPopulateScope(left, atomic.Item));
                }
            }

            return TokenMatching<ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakPathOperationPopulateScope : ISetUp<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>
        {
            private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left;
            private readonly string name;

            public WeakPathOperationPopulateScope(IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left,
                string name)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public ISetUpResult<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
            {
                var nextLeft = left.TransformInner(x => x.Run(scope, context));

                var member = nextLeft.SwitchReturns(
                    good =>
                    {
                        if (good.SetUpSideNode.Is1(out var nodeLeft) && nodeLeft is Tpn.IValue value)
                        {
                            return OrType.Make<Tpn.TypeProblem2.Member, IError>(context.TypeProblem.CreateHopefulMember(
                                value,
                                new NameKey(name),
                                new WeakMemberDefinitionConverter(false, new NameKey(name))));
                        }
                        else
                        {
                            return OrType.Make<Tpn.TypeProblem2.Member, IError>(Error.Other(""));
                            // todo better error handling 
                            throw new NotImplementedException($"can not . off {good.SetUpSideNode}");
                        }
                    },
                    error => OrType.Make<Tpn.TypeProblem2.Member, IError>(Error.Cascaded("We needed ", error)));

                return new SetUpResult<IBox<WeakPathOperation>, Tpn.TypeProblem2.Member>(new WeakPathOperationResolveReference(
                    nextLeft.TransformInner(x=>x.Resolve),
                    member), 
                    member);
            }
        }

        private class WeakPathOperationResolveReference : IResolve<IBox<WeakPathOperation>>
        {
            readonly IOrType<IResolve<IBox<IFrontendCodeElement>>,IError> left;
            readonly IOrType<Tpn.TypeProblem2.Member,IError> member;

            public WeakPathOperationResolveReference(
                IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> resolveReference,
                IOrType<Tpn.TypeProblem2.Member, IError> member)
            {
                left = resolveReference ?? throw new ArgumentNullException(nameof(resolveReference));
                this.member = member ?? throw new ArgumentNullException(nameof(member));
            }

            public IBox<WeakPathOperation> Run(Tpn.TypeSolution context)
            {
                var res = new Box<WeakPathOperation>(new WeakPathOperation(
                    left.TransformInner(x => x.Run(context)),
                    member.SwitchReturns(
                        x => OrType.Make<IBox<IFrontendCodeElement>, IError>(new Box<WeakMemberReference>(new WeakMemberReference(context.GetMember(x)))),
                        y => OrType.Make<IBox<IFrontendCodeElement>, IError>(Error.Cascaded("", y)))));
                return res;
            }
        }
    }
}
